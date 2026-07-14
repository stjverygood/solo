using Godot;
namespace Solo.Scripts.System.UiSystem
{
    public partial class PauseView : Control
    {
        [Export] private Button _continueBtn = null!;
        [Export] private Button _exitBtn = null!;

        public override void _Ready()
        {
            ProcessMode = ProcessModeEnum.WhenPaused;
            _continueBtn.Pressed += () =>
            {
                //GameManager.Instance.ChangeState(GameState.Play);
            };
            _exitBtn.Pressed += () =>
            {
                //GameManager.Instance.ChangeState(GameState.StartMenu);
            };
        }
    }
}
