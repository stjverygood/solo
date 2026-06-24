using Godot;
using Solo.Scripts.Global.Interfaces;
using System.Collections.Generic;
namespace Solo.Scripts.System.ItemSystem
{
    public partial class Fireball : Area2D
    {
        [Export] public float Speed = 300f; // 箭矢飞行速度
        [Export] public float LifeTime = 3f; // 3秒没撞到东西自动销毁，防止飞出地图
        [Export] public Area2D _explosionArea; // 3秒没撞到东西自动销毁，防止飞出地图
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
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_curDir == Vector2.Zero)
                return;
            GlobalPosition += _curDir * Speed * (float)delta;
        }

        private void _BodyEntered(Node2D body)
        {
            if (body is ITargetable target && target != _projecter && target is not DropItem)
            {
                foreach (ITargetable targetable in _explosionTargetList)
                {
                    if (target.IsVaild())
                    {
                        targetable.TakeDamage(this, _damage, null);
                    }
                }
                QueueFree();
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
