using Godot;
using Solo.Scripts.Global;
namespace Solo.Scripts.Entities.Players
{
    public partial class DeathView : Control
    {
        [Export] private Button _exitBtn;
        [Export] private Button _restartBtn;
        public override void _Ready()
        {
            _exitBtn.Pressed += () =>
            {
                GameManager.Instance.ChangeState(GameState.StartMenu);
                Visible = false;
            };

            _restartBtn.Pressed += () =>
            {
                Visible = false;
                GetTree().CreateTimer(1).Timeout += () =>
                {
                    GameManager.Instance.Player.Restart();
                };
            };
        }



        public override void _Process(double delta)
        {
        }
    }
}
