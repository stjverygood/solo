using Godot;
using Solo.Scripts.UI.InventoryViews;
using Solo.Scripts.UI.ScreenViews;

public partial class UIManager : CanvasLayer
{

    [Export] private PauseView _pauseView = null!;
    [Export] private InventoryManagerView _inventoryManagerView = null!;
    public override void _Ready()
    {
        _pauseView.Visible = false;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("Back"))
        {
            //_pauseView.Visible = !_pauseView.Visible;
            //GetTree().Paused = !GetTree().Paused;
        }
    }
}
