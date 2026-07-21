using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.UI.ScreenViews
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

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey keyEvent && keyEvent.Pressed)
            {
                switch (keyEvent.Keycode)
                {
                    case Key.Escape:
                        Visible = false;
                        GetTree().Paused = false;
                        GetViewport().SetInputAsHandled();
                        break;
                }
            }
        }
    }
}
