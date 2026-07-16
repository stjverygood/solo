using Godot;
using Solo.Scripts.Global;
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
                GetTree().Paused = false;
                Visible = false;
            };
            _exitBtn.Pressed += () =>
            {
                GetTree().Paused = false;
                GameManager.Instance.ReturnMainMenu();
            };
        }

        public override void _PhysicsProcess(double delta)
        {

        }
    }
}
