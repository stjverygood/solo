using Godot;
using Solo.Scripts.Global;

public partial class BuildingPreview : Node2D
{
    [Export] Node2D Sprite;
    [Export] public BuildingType Type;

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public void SetCanPlace(bool canPlace)
    {
        Sprite.Modulate = canPlace ? new Color(0, 1, 0) : new Color(1, 0, 0);
    }
}
