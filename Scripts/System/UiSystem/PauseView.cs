using Godot;
using Solo.Scripts.Global;

public partial class PauseView : Control
{
    [Export] private Button _continueBtn;
    [Export] private Button _exitBtn;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.WhenPaused;
        _continueBtn.Pressed += () =>
        {
            GameManager.Instance.ChangeState(GameState.Play);
        };
        _exitBtn.Pressed += () =>
        {
            GameManager.Instance.ChangeState(GameState.StartMenu);
        };
    }
}
