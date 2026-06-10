using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
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
        Hurt,//受伤, 可以在这里添加击退等效果
        Death,//死亡
    }


    public partial class Unit : Node2D
    {
        public UnitType Type;
        private float _maxHp;
        private float _curHp;
        private float _patrolRadius;
        private float _moveSpeed;
        private HashSet<UnitType> _hostileUnitTypeSet = new HashSet<UnitType>();//敌对单位类型
        private HashSet<UnitType> _fearUnitTypeSet = new HashSet<UnitType>();//畏惧单位类型

        private UnitState _curState;
        private Vector2 _curDir = Vector2.Right;
        [Export] private NavigationAgent2D _naviAgent;
        [Export] private Area2D _viewArea;
        [Export] private Node2D _animRoot;
        private Tween _animTween;
        private List<Node2D> _targetNodeList = new List<Node2D>();

        public override void _Ready()
        {
            _viewArea.BodyEntered += _viewArea_BodyEntered;
            _viewArea.BodyExited += _viewArea_BodyExited;
        }



        private void _viewArea_BodyEntered(Node2D body)
        {
            if (body is Player player)
            {
                _targetNodeList.Add(player);
            }
            else if (body is Unit unit)
            {
                _targetNodeList.Add(unit);
            }
        }
        private void _viewArea_BodyExited(Node2D body)
        {
            if (body is Player player)
            {
                _targetNodeList.Remove(player);
            }
            else if (body is Unit unit)
            {
                _targetNodeList.Remove(unit);
            }
        }

        public void Init(UnitType type, Vector2 worldPos)
        {
            Type = type;
            GlobalPosition = worldPos;
            UnitData unitData = UnitDataManager.Instance.GetUnitData(Type);
            _maxHp = unitData.MaxHp;
            _curHp = _maxHp;
            _moveSpeed = unitData.MoveSpeed;
            _patrolRadius = unitData.PatrolRadius;
            _hostileUnitTypeSet = unitData.HostileUnitTypeList.ToHashSet();
            _fearUnitTypeSet = unitData.FearUnitTypeList.ToHashSet();
            ChangeState(UnitState.Idle);
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
                case UnitState.Hurt:
                    ExitHurt();
                    break;
                case UnitState.Death:
                    ExitDeath();
                    break;
            }
        }

        #region Idle
        private float _idleDuration = 3;
        private float _idleTimer = 0;

        private void EnterIdle()
        {
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
                _naviAgent.TargetPosition = GetRandomTargetPosition();
                ChangeState(UnitState.Patrol);
                return;
            }
        }
        private void ExitIdle()
        {
            _idleTimer = 0;
        }
        #endregion

        #region Patrol
        private void EnterPatrol()
        {
            ResetAnim();
            _animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out).SetLoops();
            _animTween.TweenProperty(_animRoot, "skew", 0.1f, 0.3f);// 走动效果：左右晃动或轻微拉伸
            _animTween.TweenProperty(_animRoot, "skew", -0.1f, 0.3f);
        }
        private void UpdatePatrol(float delta)
        {
            if (_naviAgent.IsNavigationFinished())
            {
                ChangeState(UnitState.Idle);
                return;
            }
            _curDir = (_naviAgent.GetNextPathPosition() - GlobalPosition).Normalized();
            if (_curDir.X > 0)
                Scale = new Vector2(1, 1);
            else
                Scale = new Vector2(-1, 1);
            GlobalPosition += delta * _moveSpeed * _curDir;
        }
        private void ExitPatrol()
        {

        }
        #endregion

        #region Chase
        private void EnterChase()
        {

        }
        private void UpdateChase(float delta)
        {

        }
        private void ExitChase()
        {

        }
        #endregion

        #region Atk
        private void EnterAtk()
        {

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
            float randomAngle = GD.Randf() * Mathf.Tau; // 1. 生成 360 度随机方向 // Tau 就是 2 * PI
            Vector2 randomDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle));
            float randomDistance = (float)GD.RandRange(0, _patrolRadius);// 2. 计算随机距离和原始目标点
            Vector2 rawTargetPos = GlobalPosition + randomDirection * randomDistance;
            Vector2 validTargetPos = NavigationServer2D.MapGetClosestPoint(mapRid, rawTargetPos);// 3. 使用 MapGetClosestPoint（这就是正确的 API）把点吸附到导航网格上
            return validTargetPos;
        }
    }

}
