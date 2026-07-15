using Godot;
using Solo.Scripts.System.UiSystem;

public partial class UiManager : CanvasLayer
{

    [Export] private PauseView _pauseView = null!;
    public override void _Ready()
    {
        _pauseView.Visible = false;
        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("Back"))
        {
            _pauseView.Visible = !_pauseView.Visible;
            GetTree().Paused = !GetTree().Paused;
        }
    }
}
