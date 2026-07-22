using Godot;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Projectiles
{
    public class ProjectileContext
    {
        public IEntity Projecter = null!;
        public IEntity? Target;
        public Vector2 StartPos;
        public Vector2 TargetPos;
    }
}
