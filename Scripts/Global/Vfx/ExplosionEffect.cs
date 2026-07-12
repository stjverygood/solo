using Godot;

namespace Solo.Scripts.Global.Vfx
{
    public partial class ExplosionEffect : Node2D
    {
        [Export] private GpuParticles2D particles = null!;

        public void Init(Vector2 worldPos)
        {
            GlobalPosition = worldPos;

            particles.Emitting = true;
            particles.Finished += () =>
            {
                QueueFree();
            };
        }
    }
}
