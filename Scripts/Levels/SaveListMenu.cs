using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.Levels
{
    public partial class SaveListMenu : Control
    {
        [Export] private Button _backBtn;
        public override void _Ready()
        {
            _backBtn.Pressed += () =>
            {
                GameManager.Instance.ChangeGameState(GameState.StartMenu);
            };
        }

        public override void _Process(double delta)
        {
        }
    }
}


