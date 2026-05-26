using Godot;
using Solo.Scripts.Character.Player;

public partial class MainLevel : Node2D
{
    [Export] private PackedScene _playerPs;
    [Export] private PackedScene _ChunkManagerPs;

    public override void _Ready()
    {
        ChunkManager chunkManager = _ChunkManagerPs.Instantiate<ChunkManager>();
        AddChild(chunkManager);
        Player player = _playerPs.Instantiate<Player>();
        AddChild(player);

    }
}
