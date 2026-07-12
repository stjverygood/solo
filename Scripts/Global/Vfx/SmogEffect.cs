using Godot;

public partial class SmogEffect : Node2D
{
    [Export] private GpuParticles2D _particles = null!;

    public void Init(Vector2 worldPos)
    {
        GlobalPosition = worldPos;
    }

    public override void _Process(double delta)
    {
    }

    public void Distory()
    {
        GetTree().CreateTimer(_particles.Lifetime).Timeout += () =>
        {
            QueueFree();
        };
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        GD.Print("smog exit tree");
    }
}
