using Godot;
using Solo.Scripts.Character;

public partial class MainLevel : Node2D
{
    [Export] private PackedScene _playerPs;
    [Export] private PackedScene _ChunkManagerPs;

    public override void _Ready()
    {
        Player player = _playerPs.Instantiate<Player>();
        ChunkManager chunkManager = _ChunkManagerPs.Instantiate<ChunkManager>();
        AddChild(player);
        AddChild(chunkManager);
    }

    public override void _Process(double delta)
    {
    }
}
