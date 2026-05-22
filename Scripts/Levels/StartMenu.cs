using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.Levels
{
    public partial class StartMenu : Control
    {
        [Export] private Button _startBtn;
        [Export] private Button _settingBtn;
        [Export] private Button _collectionBtn;
        [Export] private Button _aboutBtn;
        [Export] private Button _exitBtn;

        public override void _Ready()
        {
            _startBtn.Pressed += () =>
            {
                GameManager.Instance.ChangeGameState(GameState.WorldListMenu);
            };
            _exitBtn.Pressed += () =>
            {
                GetTree().Quit();
            };
        }

        public override void _Process(double delta)
        {
        }
    }
}


