using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solo.Scripts.Entities.Units
{
    //整体逻辑 : 无事就到处乱逛, 每个单位都有一个敌对阵营列表和畏惧阵营列表, 剩余是中立阵营
    //看到敌对阵营列表中的单位就会发起追击, 攻击
    //看到畏惧阵营列表就会向反方向逃跑
    //若被中立阵营攻击了, 温顺的就会该阵营加入到自己的畏惧阵营列表里, 进入逃跑状态
    //强硬的就会把其加入到敌对阵营里, 进入攻击状态

    public enum UnitState
    {
        Idle, //闲置
        Patrol,//巡逻/闲逛
        Chase,//追击
        Atk,//攻击
        Escape,//逃跑
        Cd,//攻击后冷却
        Hurt,//受伤, 可以在这里添加击退等效果
        Death,//死亡
    }


    public partial class Unit : Area2D
    {
        public UnitType Type;
        private float _maxHp;
        private float _curHp;
        private float _patrolRadius;
        private float _moveSpeed;
        private float _atkRangeSq;
        private HashSet<UnitType> _hostileUnitTypeSet = new HashSet<UnitType>();//敌对单位类型
        private HashSet<UnitType> _fearUnitTypeSet = new HashSet<UnitType>();//畏惧单位类型
        private List<(ItemType, int, int)> _dropItemList;//掉落物类型, 最小掉落数量, 最大掉落数量

        private UnitState _curState;
        private Vector2 _curDir = Vector2.Right;
        [Export] private NavigationAgent2D _naviAgent;
        [Export] private Area2D _viewArea;
        [Export] private CollisionShape2D _viewCollisionShape;
        [Export] private Node2D _spriteRoot;
        [Export] private Node2D _animRoot;
        [Export] private Sprite2D _sprite;
        [Export] public PackedScene DropItemPs;
        [Export] private Label _debugLb;
        private ShaderMaterial _shaderMaterial;
        private Tween _animTween;

        public void Init(UnitType type, Vector2 worldPos)
        {
            GameManager.Instance.UnitList.Add(this);

            Type = type;
            GlobalPosition = worldPos;
            UnitData unitData = UnitDataManager.Instance.GetUnitData(Type);
            _maxHp = unitData.MaxHp;
            _curHp = _maxHp;
            _moveSpeed = unitData.MoveSpeed;
            _patrolRadius = unitData.PatrolRadius;
            _idleDuration = unitData.IdleDuration;
            _patrolDuration = unitData.PatrolDuration;
            _atkRangeSq = unitData.AtkRange * unitData.AtkRange;
            _cdDuration = unitData.CdDuration;
            _naviAgent.Radius = unitData.NaviAgentRadius;          // 根据你的单位大小调整
            _hostileUnitTypeSet = unitData.HostileUnitTypeList.ToHashSet();
            _fearUnitTypeSet = unitData.FearUnitTypeList.ToHashSet();
            _dropItemList = unitData.DropItemList;
            if (_viewCollisionShape.Shape is CircleShape2D circleShape)
            {
                circleShape.Radius = unitData.ViewCollisionShapeRadius;
            }

            ChangeState(UnitState.Idle);
            _viewArea.AreaEntered += _viewArea_AreaEntered;
            _viewArea.AreaExited += _viewArea_AreaExited;
            _viewArea.BodyEntered += _viewArea_BodyEntered;
            _viewArea.BodyExited += _viewArea_BodyExited;
            _naviAgent.VelocityComputed += OnVelocityComputed;


            if (_sprite.Material is ShaderMaterial shaderMat)
            {
                _shaderMaterial = (ShaderMaterial)shaderMat.Duplicate();// 关键：复制一份材质，确保每个实例的材质相互独立
                _sprite.Material = _shaderMaterial;// 记得把复制后的独立材质重新赋给当前的 Sprite2D
            }
            ShowOutline(false);
        }





        public override void _PhysicsProcess(double delta)
        {
            UpdataState((float)delta);
        }

        private void ChangeState(UnitState newState)
        {
            ExitState(_curState);
            EnterState(newState);
            _curState = newState;
            _debugLb.Text = _curState.ToString();
        }
        private void EnterState(UnitState state)
        {
            switch (state)
            {
                case UnitState.Idle:
                    EnterIdle();
                    break;
                case UnitState.Patrol:
                    EnterPatrol();
                    break;
                case UnitState.Chase:
                    EnterChase();
                    break;
                case UnitState.Atk:
                    EnterAtk();
                    break;
                case UnitState.Escape:
                    EnterEscape();
                    break;
                case UnitState.Cd:
                    EnterCd();
                    break;
                case UnitState.Hurt:
                    EnterHurt();
                    break;
                case UnitState.Death:
                    EnterDeath();
                    break;
            }
        }
        private void UpdataState(float delta)
        {
            switch (_curState)
            {
                case UnitState.Idle:
                    UpdateIdle(delta);
                    break;
                case UnitState.Patrol:
                    UpdatePatrol(delta);
                    break;
                case UnitState.Chase:
                    UpdateChase(delta);
                    break;
                case UnitState.Atk:
                    UpdateAtk(delta);
                    break;
                case UnitState.Escape:
                    UpdateEscape(delta);
                    break;
                case UnitState.Cd:
                    UpdateCd(delta);
                    break;
                case UnitState.Hurt:
                    UpdateHurt(delta);
                    break;
                case UnitState.Death:
                    UpdateDeath(delta);
                    break;
            }
        }
        private void ExitState(UnitState state)
        {
            switch (state)
            {
                case UnitState.Idle:
                    ExitIdle();
                    break;
                case UnitState.Patrol:
                    ExitPatrol();
                    break;
                case UnitState.Chase:
                    ExitChase();
                    break;
                case UnitState.Atk:
                    ExitAtk();
                    break;
                case UnitState.Escape:
                    ExitEscape();
                    break;
                case UnitState.Cd:
                    ExitCd();
                    break;
                case UnitState.Hurt:
                    ExitHurt();
                    break;
                case UnitState.Death:
                    ExitDeath();
                    break;
            }
        }

        #region Idle
        private float _idleDuration = 1;
        private float _idleTimer = 0;
        private void EnterIdle()
        {
            _naviAgent.TargetPosition = GlobalPosition;
            _naviAgent.Velocity = Vector2.Zero;
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(_animRoot, "scale", new Vector2(1.2f, 0.8f), 0.5f);
            _animTween.TweenProperty(_animRoot, "scale", new Vector2(1.0f, 1.0f), 0.5f);
        }
        private void UpdateIdle(float delta)
        {
            _idleTimer += delta;
            if (_idleTimer >= _idleDuration)
            {
                ChangeState(UnitState.Patrol);
                return;
            }


            RefreshFearNode();
            if (_curFearNode != null)
            {
                ChangeState(UnitState.Escape);
                return;
            }
            RefreshNearestHostileNode();
            if (_curNearestHostileNode != null)
            {
                ChangeState(UnitState.Chase);
                return;
            }
        }
        private void ExitIdle()
        {
            _idleTimer = 0;
        }
        #endregion

        #region Patrol
        private float _patrolDuration = 1;
        private float _patrolTimer = 0;
        private void EnterPatrol()
        {
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(_animRoot, "skew", 0.1f, 0.3f);// 走动效果：左右晃动或轻微拉伸
            _animTween.TweenProperty(_animRoot, "skew", -0.1f, 0.3f);
            _naviAgent.TargetPosition = GetRandomTargetPosition();
        }
        private void UpdatePatrol(float delta)
        {
            if (_naviAgent.IsNavigationFinished())
            {
                ChangeState(UnitState.Idle);
                return;
            }

            _patrolTimer += delta;
            if (_patrolTimer > _patrolDuration)
            {
                ChangeState(UnitState.Idle);
                return;
            }

            RefreshFearNode();
            if (_curFearNode != null)
            {
                ChangeState(UnitState.Escape);
                return;
            }
            RefreshNearestHostileNode();
            if (_curNearestHostileNode != null)
            {
                ChangeState(UnitState.Chase);
                return;
            }


            _curDir = (_naviAgent.GetNextPathPosition() - GlobalPosition).Normalized();
            if (_curDir.X > 0)
                _spriteRoot.Scale = new Vector2(1, 1);
            else
                _spriteRoot.Scale = new Vector2(-1, 1);
            _naviAgent.SetVelocity(_moveSpeed * _curDir);
        }
        private void ExitPatrol()
        {
            _patrolTimer = 0;
        }
        #endregion

        #region Chase
        private void EnterChase()
        {

        }
        private void UpdateChase(float delta)
        {
            RefreshFearNode();
            if (_curFearNode != null)
            {
                ChangeState(UnitState.Escape);
                return;
            }
            RefreshNearestHostileNode();
            if (_curNearestHostileNode == null)
            {
                ChangeState(UnitState.Idle);
                return;
            }

            if (GlobalPosition.DistanceSquaredTo(_curNearestHostileNode.GlobalPosition) <= _atkRangeSq)
            {
                ChangeState(UnitState.Atk);
                return;
            }

            _curDir = (_curNearestHostileNode.GlobalPosition - GlobalPosition).Normalized();
            if (_curDir.X > 0)
                _spriteRoot.Scale = new Vector2(1, 1);
            else
                _spriteRoot.Scale = new Vector2(-1, 1);
            _naviAgent.SetVelocity(_moveSpeed * _curDir);
        }
        private void ExitChase()
        {

        }
        #endregion

        #region Atk
        private void EnterAtk()
        {
            ResetAnim();

            _curDir = (_curNearestHostileNode.GlobalPosition - GlobalPosition).Normalized();
            if (_curDir.X > 0)
                _spriteRoot.Scale = new Vector2(1, 1);
            else
                _spriteRoot.Scale = new Vector2(-1, 1);

            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            _animTween.Parallel().TweenProperty(_animRoot, "scale", new Vector2(0.8f, 0.8f), 0.05f);//出手动画
            _animTween.TweenProperty(_animRoot, "rotation", 1.3f, 0.05f);
            _animTween.TweenCallback(Callable.From(() =>
            {
                foreach (Node2D viewNode in _viewNodeList)
                {
                    if (!IsInstanceValid(viewNode)) continue;
                    if (GlobalPosition.DistanceSquaredTo(viewNode.GlobalPosition) > _atkRangeSq) continue;
                    Vector2 toTarget = viewNode.GlobalPosition - GlobalPosition;
                    if (Mathf.Abs(_curDir.AngleTo(toTarget)) > Mathf.DegToRad(60f)) continue;// 60 度转换为弧度是 Mathf.DegToRad(60)
                    if (viewNode is Player player)
                    {
                        player.TakeDamage(10);
                    }
                    else if (viewNode is Unit unit)
                    {
                        unit.TakeDamage(null, 10);
                    }
                }
            }));

            _animTween.Parallel().TweenProperty(_animRoot, "scale", new Vector2(1f, 1f), 0.1f);
            _animTween.TweenProperty(_animRoot, "rotation", 0f, 0.1f);
            _animTween.Finished += () =>
            {
                ChangeState(UnitState.Cd);
                return;
            };
        }
        private void UpdateAtk(float delta)
        {

        }
        private void ExitAtk()
        {

        }
        #endregion

        #region Escape
        private void EnterEscape()
        {

        }
        private void UpdateEscape(float delta)
        {

        }
        private void ExitEscape()
        {

        }
        #endregion

        #region Cd
        private float _cdDuration = 1;
        private float CdTimer = 0;
        private void EnterCd()
        {
            _naviAgent.TargetPosition = GlobalPosition;
            _naviAgent.Velocity = Vector2.Zero;
        }
        private void UpdateCd(float delta)
        {
            CdTimer += delta;
            if (CdTimer >= _cdDuration)
            {
                ChangeState(UnitState.Idle);
                return;
            }
        }
        private void ExitCd()
        {
            CdTimer = 0;
        }
        #endregion

        #region Hurt
        private void EnterHurt()
        {

        }
        private void UpdateHurt(float delta)
        {

        }
        private void ExitHurt()
        {

        }
        #endregion

        #region Death
        private void EnterDeath()
        {

        }
        private void UpdateDeath(float delta)
        {

        }
        private void ExitDeath()
        {

        }
        #endregion




        private void ResetAnim()
        {
            if (_animTween != null && _animTween.IsRunning())
                _animTween.Kill();
            _animRoot.Scale = Vector2.One;
            _animRoot.Skew = 0f;
            _animRoot.Modulate = Colors.White;
            _animRoot.Rotation = 0f;
            _animRoot.Modulate = Colors.White;
        }

        public Vector2 GetRandomTargetPosition()
        {
            Rid mapRid = _naviAgent.GetNavigationMap();
            Vector2 bestPos = GlobalPosition;

            int maxAttempts = 10; // 可以适当增加尝试次数
            for (int i = 0; i < maxAttempts; i++)
            {
                float randomAngle = GD.Randf() * Mathf.Tau;
                Vector2 randomDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
                float randomDistance = (float)GD.RandRange(20.0f, _patrolRadius); // 设置最小距离，避免原地发呆
                Vector2 rawTargetPos = GlobalPosition + randomDirection * randomDistance;

                // 1. 获取网格上最近的合法点
                Vector2 validTargetPos = NavigationServer2D.MapGetClosestPoint(mapRid, rawTargetPos);

                // 2. 核心：测试从当前位置到目标点是否有完整的通畅路径
                // optimize: true 代表对路径进行平滑优化处理
                Vector2[] path = NavigationServer2D.MapGetPath(mapRid, GlobalPosition, validTargetPos, optimize: true);

                // 3. 验证路径有效性
                if (path != null && path.Length > 1)
                {
                    // 确保路径的实际终点离我们期望的目标点足够近 (例如小于 15 像素)
                    // 如果终点差得太远，说明中间被完全挡断，走不过去
                    if (path[path.Length - 1].DistanceTo(validTargetPos) < 15.0f)
                    {
                        bestPos = validTargetPos;
                        break;
                    }
                }
            }

            // 如果尝试了10次都被挡住，就返回当前位置，下一帧或下一个 Idle 周期再试
            return bestPos;
        }

        private void OnVelocityComputed(Vector2 safeVelocity)
        {
            //Velocity = safeVelocity;
            //MoveAndSlide();
            GlobalPosition += safeVelocity * (float)GetPhysicsProcessDeltaTime();
        }



        private void Die()
        {
            //GameManager.Instance.ChunkManager.RemoveItem(this, GlobalPosition);
            _curHp = 0;
            //掉落物品
            foreach ((ItemType, int, int) tuple in _dropItemList)
            {
                DropItem dropItem = DropItemPs.Instantiate<DropItem>();
                GetTree().CurrentScene.AddChild(dropItem);
                dropItem.Init(tuple.Item1, Random.Shared.Next(tuple.Item2, tuple.Item3 + 1), Position);
                dropItem.ApplyForce();
            }
            //GameManager.Instance.ChunkManager.RemoveItem(this, GlobalPosition);
            GameManager.Instance.UnitList.Remove(this);
            QueueFree();
        }


        public virtual void ShowOutline(bool isShow)
        {
            if (isShow)
            {
                _shaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1));//162, 38, 51
                _shaderMaterial.SetShaderParameter("outline_width", 1);
            }
            else
            {
                _shaderMaterial.SetShaderParameter("outline_width", 0.0f);
            }
        }
        public void TakeDamage(ItemType? dmgItemType, float damage)
        {
            damage = HandleDamage(dmgItemType, damage);
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(_animRoot, "skew", 0.2f, 0.1f);
            animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 1.0f, 0.1f);
            animTween.TweenProperty(_animRoot, "skew", -0.2f, 0.1f);
            animTween.Parallel().TweenProperty(_sprite.Material, "shader_parameter/flash_modifier", 0.0f, 0.1f);
            animTween.TweenProperty(_animRoot, "skew", 0f, 0.1f);
            FloatTextLb floatTextLb = GameManager.Instance.FloatTextLbPs.Instantiate<FloatTextLb>();
            GetTree().CurrentScene.AddChild(floatTextLb);
            floatTextLb.Init($"-{damage}", GlobalPosition);
            _curHp -= damage;
            //_damageCooldownTimer = 0f;
            //_healTimer = 0f;
            //RefreshHpBar();
            if (_curHp <= 0)
            {
                Die();

            }
        }

        private float HandleDamage(ItemType? dmgItemType, float damage)
        {
            float resDmg = damage;
            //switch (Type)
            //{
            //    case BuildingType.Tree:
            //        switch (dmgItemType)
            //        {
            //            case ItemType.WoodAxe:
            //                resDmg *= 2;
            //                break;
            //            case ItemType.IronAxe:
            //                resDmg *= 3;
            //                break;
            //            case ItemType.GoldAxe:
            //                resDmg *= 4;
            //                break;
            //            case ItemType.JadeAxe:
            //                resDmg *= 5;
            //                break;
            //            default:
            //                resDmg *= 1;
            //                break;
            //        }
            //        break;
            //    case BuildingType.Stone:
            //        switch (dmgItemType)
            //        {
            //            case ItemType.WoodPickaxe:
            //                resDmg *= 1;
            //                break;
            //            case ItemType.IronPickaxe:
            //                resDmg *= 2;
            //                break;
            //            case ItemType.GoldPickaxe:
            //                resDmg *= 3;
            //                break;
            //            case ItemType.JadePickaxe:
            //                resDmg *= 4;
            //                break;
            //            default:
            //                resDmg *= 0.1f;
            //                break;
            //        }
            //        break;
            //}
            return resDmg;
        }


        private Node2D _curFearNode = null;
        private Node2D _curNearestHostileNode = null;
        private List<Node2D> _viewNodeList = new List<Node2D>();

        private void _viewArea_AreaEntered(Area2D area)
        {
            if (area.GetParent() is Unit unit)
            {
                _viewNodeList.Add(unit);
            }
        }
        private void _viewArea_AreaExited(Area2D area)
        {
            if (area.GetParent() is Unit unit)
            {
                _viewNodeList.Remove(unit);
            }
        }
        private void _viewArea_BodyEntered(Node2D body)
        {
            if (body is Player player)
            {
                _viewNodeList.Add(player);
            }
        }
        private void _viewArea_BodyExited(Node2D body)
        {
            if (body is Player player)
            {
                _viewNodeList.Remove(player);
            }
        }
        private void RefreshFearNode()
        {
            //每帧拿到最近的node2d, 再根据该node的type决定切到追击/逃离
            //逃跑优先 : 只要viewnodelist内存在畏惧类型, 就直接逃跑
            //若无畏惧目标, 拿到最近可攻击目标, 进入追击
            //检查是否有追击/逃离目标, 若有切换对应状态
            _curFearNode = null;
            foreach (Node2D targetNode in _viewNodeList)
            {
                if (targetNode is Player player && _fearUnitTypeSet.Contains(player.Type))
                {
                    _curFearNode = player;
                    return;
                }
                else if (targetNode is Unit unit && _fearUnitTypeSet.Contains(unit.Type))
                {
                    _curFearNode = unit;
                    return;
                }
            }
        }
        private void RefreshNearestHostileNode()
        {
            _curNearestHostileNode = null;
            float curMinDistSq = float.MaxValue;
            foreach (Node2D targetNode in _viewNodeList)
            {
                float curDisq = GlobalPosition.DistanceSquaredTo(targetNode.GlobalPosition);
                if (curDisq < curMinDistSq)
                {
                    if (targetNode is Player player && _hostileUnitTypeSet.Contains(player.Type))
                    {
                        _curNearestHostileNode = targetNode;
                        curMinDistSq = curDisq;
                    }
                    else if (targetNode is Unit unit && _hostileUnitTypeSet.Contains(unit.Type))
                    {
                        _curNearestHostileNode = targetNode;
                        curMinDistSq = curDisq;
                    }
                }
            }
        }
    }

}
