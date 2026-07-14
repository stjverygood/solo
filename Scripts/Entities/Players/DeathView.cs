using Godot;
using Solo.Scripts.Global;
namespace Solo.Scripts.Entities.Players
{
    public partial class DeathView : Control
    {
        [Export] private Button _exitBtn = null!;
        [Export] private Button _restartBtn = null!;
        public override void _Ready()
        {
            _exitBtn.Pressed += () =>
            {
                //GameManager.Instance.ChangeState(GameState.StartMenu);
                Visible = false;
            };

            _restartBtn.Pressed += () =>
            {
                Visible = false;
                GetTree().CreateTimer(1).Timeout += () =>
                {
                    GameManager.Instance.Player.Revive();
                };
            };
        }



        public override void _Process(double delta)
        {
        }
    }
}
