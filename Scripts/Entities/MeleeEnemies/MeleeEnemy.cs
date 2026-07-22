using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
namespace Solo.Scripts.Entities.MeleeEnemies
{
    public enum MeleeEnemyState
    {
        Idle,
        Patrol,
        Chase,
        Atk,
        Death,
    }

    public partial class MeleeEnemy : CharacterBody2D, IEntity, ISaveable
    {
        public static EntityData DefaultData => new MeleeEnemyData
        {
            MaxHp = 100,
            Atk = 40,
            Def = 30,
            MoveSpeed = 50,
            ViewRange = 100,
            ViewRangeSq = 100 * 100,
            AtkRange = 20,
            AtkRangeSq = 20 * 20,
            IdleDuration = 1,
            PatrolRange = 200,
        };

        [Export] private AnimatedSprite2D _animSprite = null!;
        [Export] private NavigationAgent2D _naviAgent = null!;
        [Export] private Label _debugLb = null!;
        private MeleeEnemyState _curState;
        private Vector2 _curDir = Vector2.Right;
        private bool canMove = false;

        public EntityType Type;
        private MeleeEnemyData _data = null!;

        public EntityCore Core { get; private set; } = new();


        public void Init(Vector2 worldPosition, EntityType type, MeleeEnemySaveData? saveData = null)
        {
            Type = type;
            _data = (MeleeEnemyData)EntityDataManager.Instance.GetData(Type);

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

            _naviAgent.VelocityComputed += _naviAgent_VelocityComputed;

            ChangeState(MeleeEnemyState.Idle);
        }

        public EntitySaveData GetSaveData()
        {
            return new MeleeEnemySaveData()
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
        }

        private void ChangeState(MeleeEnemyState newState)
        {
            ExitState(_curState);
            _curState = newState;
            EnterState(newState);
        }
        private void EnterState(MeleeEnemyState state)
        {
            switch (state)
            {
                case MeleeEnemyState.Idle:
                    EnterIdle();
                    break;
                case MeleeEnemyState.Patrol:
                    EnterPatrol();
                    break;
                case MeleeEnemyState.Chase:
                    EnterChase();
                    break;
                case MeleeEnemyState.Atk:
                    EnterAtk();
                    break;
                case MeleeEnemyState.Death:
                    EnterDeath();
                    break;
            }
        }
        private void UpdateState(float delta)
        {
            switch (_curState)
            {
                case MeleeEnemyState.Idle:
                    UpdateIdle(delta);
                    break;
                case MeleeEnemyState.Patrol:
                    UpdatePatrol(delta);
                    break;
                case MeleeEnemyState.Chase:
                    UpdateChase(delta);
                    break;
                case MeleeEnemyState.Atk:
                    UpdateAtk(delta);
                    break;
                case MeleeEnemyState.Death:
                    UpdateDeath(delta);
                    break;
            }
        }
        private void ExitState(MeleeEnemyState state)
        {
            switch (state)
            {
                case MeleeEnemyState.Idle:
                    ExitIdle();
                    break;
                case MeleeEnemyState.Patrol:
                    ExitPatrol();
                    break;
                case MeleeEnemyState.Chase:
                    ExitChase();
                    break;
                case MeleeEnemyState.Atk:
                    ExitAtk();
                    break;
                case MeleeEnemyState.Death:
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
                ChangeState(MeleeEnemyState.Death);
                return;
            }


            Player player = GameManager.Instance.Player;
            if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= _data.ViewRangeSq)
            {
                ChangeState(MeleeEnemyState.Chase);
                return;
            }

            _idleTimer += delta;
            if (_idleTimer >= _data.IdleDuration)
            {
                ChangeState(MeleeEnemyState.Patrol);
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
            if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= _data.ViewRangeSq)
            {
                ChangeState(MeleeEnemyState.Chase);
                return;
            }

            if (_naviAgent.IsNavigationFinished())
            {
                ChangeState(MeleeEnemyState.Idle);
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
                ChangeState(MeleeEnemyState.Death);
                return;
            }

            Player player = GameManager.Instance.Player;
            if (player == null || GlobalPosition.DistanceSquaredTo(player.GlobalPosition) > _data.ViewRangeSq)
            {
                ChangeState(MeleeEnemyState.Idle);
                return;
            }

            if (GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= _data.AtkRangeSq)
            {
                ChangeState(MeleeEnemyState.Atk);
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
                GameManager.Instance.SpawnSwordWave(this, (GameManager.Instance.Player.GlobalPosition - GlobalPosition).Normalized());
            }
            if (_animSprite.IsPlaying() == false)
            {
                ChangeState(MeleeEnemyState.Idle);
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