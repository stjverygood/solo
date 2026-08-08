using Godot;
using Solo.Scripts.Components.AtkComponents;
using Solo.Scripts.Components.DefComponents;
using Solo.Scripts.Components.HpComponents;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Projectiles
{
    public partial class Arrow : Area2D
    {
        [Export] public float Speed = 200f; // 箭矢飞行速度
        [Export] public float LifeTime = 3f; // 3秒没撞到东西自动销毁，防止飞出地图
        private ProjectileContext _context = null!;
        private Vector2 _curDir = Vector2.Zero;

        public void Init(ProjectileContext context)
        {
            _context = context;
            GlobalPosition = _context.StartPos;
            _curDir = (_context.TargetPos - GlobalPosition).Normalized();
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
            if (body == _context.Projecter)
                return;
            if (body is not IEntity entity)
                return;
            if (entity.Core.TryGetComponent(out DefComponent defComponent) == false)
                return;
            if (entity.Core.TryGetComponent(out HpComponent hpComponent) == false)
                return;
            float damage = GameManager.Instance.CalculateDamage(_context.Projecter.Core.GetComponent<AtkComponent>().GetAtk(), defComponent.GetDef());
            hpComponent.Consume(damage);
            QueueFree();
        }
    }
}