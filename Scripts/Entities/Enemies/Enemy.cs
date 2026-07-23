using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.Projectiles;
namespace Solo.Scripts.Entities.Enemies
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Atk,
        Death,
    }

    public partial class Enemy : CharacterBody2D, IEntity, ISaveable
    {
        [Export] private AnimatedSprite2D _animSprite = null!;
        [Export] private NavigationAgent2D _naviAgent = null!;
        [Export] private Label _debugLb = null!;
        private EnemyState _curState;
        private Vector2 _curDir = Vector2.Right;
        private bool canMove = false;

        public EntityType Type;
        private EnemyData _data = null!;

        public EntityCore Core { get; private set; } = new();

        public void Init(EntityType type, Vector2 worldPosition, EntitySaveData? entitySaveData = null)
        {
            Type = type;
            _data = (EnemyData)EntityDataManager.Instance.GetData(Type);
            EnemySaveData? saveData = (EnemySaveData?)entitySaveData;

            if (saveData == null)
                GlobalPosition = worldPosition;
            else
                GlobalPosition = new Vector2(saveData.WorldX, saveData.WorldY);

            HpComponent hpComponent = new HpComponent(this);
            if (saveData == null)
                hpComponent.SetValue(_data.MaxHp, _data.MaxHp);
            else
                hpComponent.SetValue(saveData.CurHp, _data.MaxHp);
            hpComponent.OnValueChanged += (curValue, maxValue) =>
            {
                GD.Print($"{curValue} / {maxValue}");
            };
            Core.AddComponent(hpComponent);

            AtkComponent atkComponent = new AtkComponent(this);
            atkComponent.Refresh(_data.Atk);
            Core.AddComponent(atkComponent);

            DefComponent defComponent = new DefComponent(this);
            defComponent.Refresh(_data.Def);
            Core.AddComponent(defComponent);

            DynamicComponent dynamicComponent = new DynamicComponent(this);
            Core.AddComponent(dynamicComponent);

            _naviAgent.VelocityComputed += _naviAgent_VelocityComputed;

            ChangeState(EnemyState.Idle);
        }

        public EntitySaveData GetSaveData()
        {
            return new EnemySaveData()
            {
                Type = Type,
                WorldX = GlobalPosition.X,
                WorldY = GlobalPosition.Y,
                CurHp = Core.GetComponent<HpComponent>().CurValue
            };
        }

        public override void _PhysicsProcess(double delta)
        {
            UpdateState((float)delta);
            _debugLb.Text = _curState.ToString();
            if (Core.TryGetComponent<DynamicComponent>(out DynamicComponent dynamicComponent))
            {
                dynamicComponent.RefreshChunkPos(GlobalPosition);
            }
        }

        private void ChangeState(EnemyState newState)
        {
            ExitState(_curState);
            _curState = newState;
            EnterState(newState);
        }
        private void EnterState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    EnterIdle();
                    break;
                case EnemyState.Patrol:
                    EnterPatrol();
                    break;
                case EnemyState.Chase:
                    EnterChase();
                    break;
                case EnemyState.Atk:
                    EnterAtk();
                    break;
                case EnemyState.Death:
                    EnterDeath();
                    break;
            }
        }
        private void UpdateState(float delta)
        {
            switch (_curState)
            {
                case EnemyState.Idle:
                    UpdateIdle(delta);
                    break;
                case EnemyState.Patrol:
                    UpdatePatrol(delta);
                    break;
                case EnemyState.Chase:
                    UpdateChase(delta);
                    break;
                case EnemyState.Atk:
                    UpdateAtk(delta);
                    break;
                case EnemyState.Death:
                    UpdateDeath(delta);
                    break;
            }
        }
        private void ExitState(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Idle:
                    ExitIdle();
                    break;
                case EnemyState.Patrol:
                    ExitPatrol();
                    break;
                case EnemyState.Chase:
                    ExitChase();
                    break;
                case EnemyState.Atk:
                    ExitAtk();
                    break;
                case EnemyState.Death:
                    ExitDeath();
                    break;
            }
        }

        #region Idle
        private float _idleTimer;
        private void EnterIdle()
        {
            canMove = false;
            _animSprite.Play("Idle");
        }
        private void UpdateIdle(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurValue <= 0)
            {
                ChangeState(EnemyState.Death);
                return;
            }

            Player player = GameManager.Instance.Player;
            if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) >= 500 * 500)
            {
                return;
            }

            if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= _data.ViewRangeSq)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            _idleTimer += delta;
            if (_idleTimer >= _data.IdleDuration)
            {
                ChangeState(EnemyState.Patrol);
                return;
            }
        }
        private void ExitIdle()
        {
            _idleTimer = 0;
        }
        #endregion

        #region Patrol
        //private Vector2 _curPatrolTarget;
        private void EnterPatrol()
        {
            _animSprite.Play("Move");
            canMove = true;
            _naviAgent.TargetPosition = GetRandomValidTarget(GlobalPosition, _data.PatrolRange); ;
        }
        private void UpdatePatrol(float delta)
        {
            Player player = GameManager.Instance.Player;
            if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) >= 500 * 500)
            {
                ChangeState(EnemyState.Idle);
                return;
            }
            if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= _data.ViewRangeSq)
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            if (_naviAgent.IsNavigationFinished())
            {
                ChangeState(EnemyState.Idle);
                return;
            }

            _curDir = (_naviAgent.GetNextPathPosition() - GlobalPosition).Normalized();
            if (_curDir.X > 0)
                _animSprite.FlipH = false;
            else
                _animSprite.FlipH = true;
            _naviAgent.Velocity = _curDir * _data.MoveSpeed;
        }
        private void ExitPatrol()
        {

        }
        #endregion

        #region Chase
        private void EnterChase()
        {
            _animSprite.Play("Move");
            canMove = true;
        }
        private void UpdateChase(float delta)
        {
            if (Core.GetComponent<HpComponent>().CurValue <= 0)
            {
                ChangeState(EnemyState.Death);
                return;
            }

            Player player = GameManager.Instance.Player;
            if (player == null || GlobalPosition.DistanceSquaredTo(player.GlobalPosition) > _data.ViewRangeSq)
            {
                ChangeState(EnemyState.Idle);
                return;
            }

            if (GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= _data.AtkRangeSq)
            {
                ChangeState(EnemyState.Atk);
                return;
            }

            _naviAgent.TargetPosition = player.GlobalPosition;

            _curDir = (_naviAgent.GetNextPathPosition() - GlobalPosition).Normalized();
            if (_curDir.X > 0)
                _animSprite.FlipH = false;
            else
                _animSprite.FlipH = true;
            _naviAgent.Velocity = _curDir * _data.MoveSpeed;
        }
        private void ExitChase()
        {

        }
        #endregion

        #region Atk
        bool isAtked = false;
        private void EnterAtk()
        {
            _animSprite.Play("Atk");
            canMove = false;
        }
        private void UpdateAtk(float delta)
        {
            if (_animSprite.Frame == 3 && isAtked == false)
            {
                isAtked = true;
                GameManager.Instance.SpawnProjectile(_data.ProjectileType,
                    new ProjectileContext()
                    {
                        Projecter = this,
                        StartPos = GlobalPosition,
                        TargetPos = GameManager.Instance.Player.GlobalPosition
                    });
            }
            if (_animSprite.IsPlaying() == false)
            {
                ChangeState(EnemyState.Idle);
                return;
            }


        }
        private void ExitAtk()
        {
            isAtked = false;
        }
        #endregion

        #region Death
        private void EnterDeath()
        {
            canMove = false;
            GameManager.Instance.SpawnExpBall(GlobalPosition, 100);
            Tween tween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            tween.TweenProperty(_animSprite, "scale", new Vector2(0.01f, 0.01f), 1f);
            tween.Finished += () =>
            {

                QueueFree();
            };
        }
        private void UpdateDeath(float delta)
        {

        }
        private void ExitDeath()
        {

        }
        #endregion

        public Vector2 GetRandomValidTarget(Vector2 origin, float radius)
        {
            Rid map = _naviAgent.GetNavigationMap();
            for (int i = 0; i < 10; i++) // 多试几次，防止采到的点超出半径
            {
                Vector2 point = NavigationServer2D.MapGetRandomPoint(map, 1, false);
                if (origin.DistanceTo(point) <= radius)
                    return point;
            }
            return origin; // 兜底：找不到合适点就留在原地
        }

        private void _naviAgent_VelocityComputed(Vector2 safeVelocity)
        {
            if (canMove == false)
                return;
            Velocity = safeVelocity;
            MoveAndSlide();
        }


    }
}