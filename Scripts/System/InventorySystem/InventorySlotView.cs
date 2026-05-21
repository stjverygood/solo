using Godot;

public partial class InventorySlotView : Control
{
    [Export] private TextureRect _iconTr;
    [Export] private Label _countLb;

    public override void _Ready()
    {
        _iconTr.Texture = null;
        _countLb.Text = "";
    }

    public void SetData(Texture2D icon, int count)
    {
        _iconTr.Texture = icon;
        _countLb.Text = $"{count}";
    }
}
