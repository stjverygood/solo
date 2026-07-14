using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
namespace Solo.Scripts.Entities.Zombies
{
    public enum ZombieState
    {
        Idle,
        Patrol,
        Chase,
        Atk,
        Death,
    }

    public partial class Zombie : CharacterBody2D, IEntity
    {
        [Export] private AnimatedSprite2D _animSprite = null!;
        [Export] private NavigationAgent2D _naviAgent = null!;
        [Export] private Label _debugLb = null!;
        private ZombieState _curState;
        private Vector2 _curDir = Vector2.Right;
        private bool canMove = false;

        private ComponentHost _componentHost = null!;
        public T GetComponent<T>() where T : Component
        {
            return _componentHost.Get<T>();
        }
        public bool TryGetComponent<T>(out T component) where T : Component
        {
            return _componentHost.TryGet<T>(out component);
        }


        public void Init(Vector2 worldPosition)
        {
            GlobalPosition = worldPosition;

            _componentHost = new ComponentHost(this);

            ZombieData zombieData = EntityDataManager.Instance.GetData<ZombieData>();

            HpComponent hpComponent = new HpComponent();
            hpComponent.Refresh(zombieData.MaxHp, zombieData.MaxHp);
            hpComponent.OnValueChanged += (curValue, maxValue) =>
            {
                GD.Print($"{curValue} / {maxValue}");
            };
            _componentHost.Add(hpComponent);

            AtkComponent atkComponent = new AtkComponent();
            atkComponent.Refresh(zombieData.Atk);
            _componentHost.Add(atkComponent);

            DefComponent defComponent = new DefComponent();
            defComponent.Refresh(zombieData.Def);
            _componentHost.Add(defComponent);

            _naviAgent.VelocityComputed += _naviAgent_VelocityComputed;

            ChangeState(ZombieState.Idle);
        }



        public override void _PhysicsProcess(double delta)
        {
            UpdateState((float)delta);
            _debugLb.Text = _curState.ToString();
        }

        private void ChangeState(ZombieState newState)
        {
            ExitState(_curState);
            _curState = newState;
            EnterState(newState);
        }
        private void EnterState(ZombieState state)
        {
            switch (state)
            {
                case ZombieState.Idle:
                    EnterIdle();
                    break;
                case ZombieState.Patrol:
                    EnterPatrol();
                    break;
                case ZombieState.Chase:
                    EnterChase();
                    break;
                case ZombieState.Atk:
                    EnterAtk();
                    break;
                case ZombieState.Death:
                    EnterDeath();
                    break;
            }
        }
        private void UpdateState(float delta)
        {
            switch (_curState)
            {
                case ZombieState.Idle:
                    UpdateIdle(delta);
                    break;
                case ZombieState.Patrol:
                    UpdatePatrol(delta);
                    break;
                case ZombieState.Chase:
                    UpdateChase(delta);
                    break;
                case ZombieState.Atk:
                    UpdateAtk(delta);
                    break;
                case ZombieState.Death:
                    UpdateDeath(delta);
                    break;
            }
        }
        private void ExitState(ZombieState state)
        {
            switch (state)
            {
                case ZombieState.Idle:
                    ExitIdle();
                    break;
                case ZombieState.Patrol:
                    ExitPatrol();
                    break;
                case ZombieState.Chase:
                    ExitChase();
                    break;
                case ZombieState.Atk:
                    ExitAtk();
                    break;
                case ZombieState.Death:
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
            if (GetComponent<HpComponent>().CurValue <= 0)
            {
                ChangeState(ZombieState.Death);
                return;
            }


            Player player = GameManager.Instance.Player;
            if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= EntityDataManager.Instance.GetData<ZombieData>().ViewRangeSq)
            {
                ChangeState(ZombieState.Chase);
                return;
            }

            _idleTimer += delta;
            if (_idleTimer >= EntityDataManager.Instance.GetData<ZombieData>().IdleDuration)
            {
                ChangeState(ZombieState.Patrol);
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
            _naviAgent.TargetPosition = GetRandomValidTarget(GlobalPosition, EntityDataManager.Instance.GetData<ZombieData>().PatrolRange); ;
        }
        private void UpdatePatrol(float delta)
        {
            Player player = GameManager.Instance.Player;
            if (player != null && GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= EntityDataManager.Instance.GetData<ZombieData>().ViewRangeSq)
            {
                ChangeState(ZombieState.Chase);
                return;
            }

            if (_naviAgent.IsNavigationFinished())
            {
                ChangeState(ZombieState.Idle);
                return;
            }

            _curDir = (_naviAgent.GetNextPathPosition() - GlobalPosition).Normalized();
            if (_curDir.X > 0)
                _animSprite.FlipH = false;
            else
                _animSprite.FlipH = true;
            _naviAgent.Velocity = _curDir * EntityDataManager.Instance.GetData<ZombieData>().MoveSpeed;
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
            if (GetComponent<HpComponent>().CurValue <= 0)
            {
                ChangeState(ZombieState.Death);
                return;
            }

            Player player = GameManager.Instance.Player;
            if (player == null || GlobalPosition.DistanceSquaredTo(player.GlobalPosition) > EntityDataManager.Instance.GetData<ZombieData>().ViewRangeSq)
            {
                ChangeState(ZombieState.Idle);
                return;
            }

            if (GlobalPosition.DistanceSquaredTo(player.GlobalPosition) <= EntityDataManager.Instance.GetData<ZombieData>().AtkRangeSq)
            {
                ChangeState(ZombieState.Atk);
                return;
            }

            _naviAgent.TargetPosition = player.GlobalPosition;

            _curDir = (_naviAgent.GetNextPathPosition() - GlobalPosition).Normalized();
            if (_curDir.X > 0)
                _animSprite.FlipH = false;
            else
                _animSprite.FlipH = true;
            _naviAgent.Velocity = _curDir * EntityDataManager.Instance.GetData<ZombieData>().MoveSpeed;
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
                ChangeState(ZombieState.Idle);
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