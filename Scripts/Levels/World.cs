using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.System.ChunkSystem;

public partial class World : Node2D
{
    //[Export] private PackedScene _playerPs = null!;
    //[Export] private PackedScene _ChunkManagerPs = null!;
    [Export] private CanvasModulate _canvasModulate = null!;
    [Export] private Gradient _dayNightGradient = null!;
    private FastNoiseLite _noise = new FastNoiseLite();
    private string _seedString = "";

    public void Init()
    {
        Player player = GameManager.Instance.PlayerPs.Instantiate<Player>();
        AddChild(player);
        player.Init();
        ChunkManager chunkManager = GameManager.Instance.ChunkManagerPs.Instantiate<ChunkManager>();
        AddChild(chunkManager);
        chunkManager.Init();
    }

    public override void _PhysicsProcess(double delta)
    {
        //GD.Print("GameManager.Instance.TimeRatio : " + GameManager.Instance.TimeRatio);
        _canvasModulate.Color = _dayNightGradient.Sample(GameManager.Instance.TimeRatio);
    }
}
