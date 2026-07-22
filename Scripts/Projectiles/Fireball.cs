using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.Global.Vfx;
using System.Collections.Generic;
namespace Solo.Scripts.Projectiles
{
    public partial class Fireball : Area2D
    {
        [Export] public float Speed = 200f; // 箭矢飞行速度
        [Export] public float LifeTime = 3f; // 3秒没撞到东西自动销毁，防止飞出地图
        [Export] public Area2D _explosionArea = null!; // 3秒没撞到东西自动销毁，防止飞出地图
        //[Export] public GpuParticles2D _particle; // 3秒没撞到东西自动销毁，防止飞出地图
        private SmogEffect _smogEffect;

        private Vector2 _curDir = Vector2.Zero;
        private float _damage;
        private ITargetable _projecter;

        private List<ITargetable> _explosionTargetList = new List<ITargetable>();


        public void Init(ITargetable projecter, Vector2 startPos, Vector2 targetPos, float damage)
        {
            _projecter = projecter;
            GlobalPosition = startPos;
            _damage = damage;
            _curDir = (targetPos - GlobalPosition).Normalized();
            Rotation = _curDir.Angle();
            BodyEntered += _BodyEntered;
            _explosionArea.BodyEntered += _explosionArea_BodyEntered;
            _explosionArea.BodyExited += _explosionArea_BodyExited;
            GetTree().CreateTimer(LifeTime).Timeout += QueueFree;

            _smogEffect = GameManager.Instance.SmogEffectPs.Instantiate<SmogEffect>();
            GetTree().CurrentScene.AddChild(_smogEffect);
            _smogEffect.Init(GlobalPosition);
        }

        public override void _PhysicsProcess(double delta)
        {
            _smogEffect.GlobalPosition = GlobalPosition;

            if (_curDir == Vector2.Zero)
                return;
            GlobalPosition += _curDir * Speed * (float)delta;
        }

        private void _BodyEntered(Node2D body)
        {
            if (body is ITargetable target && target != _projecter)
            {
                var targets = new List<ITargetable>(_explosionTargetList);
                foreach (ITargetable targetable in targets)
                {
                    if (targetable.IsVaild())
                    {
                        targetable.TakeDamage(this, _damage, null);
                    }
                }

                _smogEffect.Distory();
                //RemoveChild(_particle);
                //GetTree().CreateTimer(0.5).Timeout += () =>
                //{
                //    GD.Print("粒子销毁");
                //    _particle.QueueFree();
                //};
                //GetTree().CurrentScene.AddChild(_particle);


                ExplosionEffect explosionEffect = GameManager.Instance.ExplosionEffectPs.Instantiate<ExplosionEffect>();
                GetTree().CurrentScene.AddChild(explosionEffect);
                explosionEffect.Init(GlobalPosition);
                QueueFree();
                GameManager.Instance.Player.TriggerScreenShake(3);
            }
        }

        private void _explosionArea_BodyEntered(Node2D body)
        {
            if (body is ITargetable target)
            {
                _explosionTargetList.Add(target);
            }
        }
        private void _explosionArea_BodyExited(Node2D body)
        {
            if (body is ITargetable target)
            {
                _explosionTargetList.Remove(target);
            }
        }
    }
}
