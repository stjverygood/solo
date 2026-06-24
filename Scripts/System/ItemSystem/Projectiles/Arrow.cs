using Godot;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.System.ItemSystem
{
    // 直接继承 Area2D，方便处理 2D 区域碰撞
    public partial class Arrow : Area2D
    {
        [Export] public float Speed = 600f; // 箭矢飞行速度
        [Export] public float LifeTime = 3f; // 3秒没撞到东西自动销毁，防止飞出地图
        private Vector2 _curDir = Vector2.Zero;
        private float _damage;
        private ITargetable _projecter;

        public void Init(ITargetable projecter, Vector2 startPos, Vector2 targetPos, float damage)
        {
            _projecter = projecter;
            GlobalPosition = startPos;
            _damage = damage;
            _curDir = (targetPos - GlobalPosition).Normalized();
            Rotation = _curDir.Angle();
            BodyEntered += _BodyEntered;
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
                target.TakeDamage(this, _damage, null);
                QueueFree();
            }
        }
    }
}