using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.System.ChunkSystem;

public partial class MainLevel : Node2D
{
    [Export] private PackedScene _playerPs;
    [Export] private PackedScene _ChunkManagerPs;
    [Export] private CanvasModulate _canvasModulate;
    [Export] private Gradient _dayNightGradient;

    public override void _Ready()
    {
        ChunkManager chunkManager = _ChunkManagerPs.Instantiate<ChunkManager>();
        AddChild(chunkManager);
        Player player = _playerPs.Instantiate<Player>();
        AddChild(player);

    }

    public override void _PhysicsProcess(double delta)
    {
        //GD.Print("GameManager.Instance.TimeRatio : " + GameManager.Instance.TimeRatio);
        _canvasModulate.Color = _dayNightGradient.Sample(GameManager.Instance.TimeRatio);
    }
}
