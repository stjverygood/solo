using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.Levels
{
    public partial class StartMenu : Control
    {
        [Export] private Button _startBtn = null!;
        [Export] private Button _settingBtn = null!;
        [Export] private Button _collectionBtn = null!;
        [Export] private Button _aboutBtn = null!;
        [Export] private Button _exitBtn = null!;

        public override void _Ready()
        {
            _startBtn.Pressed += () =>
            {
                GameManager.Instance.ChangeState(GameState.SaveListMenu);
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


