using Godot;
using Solo.Scripts.Character;

namespace Solo.Scripts.Global
{

    public partial class GameManager : Node
    {
        static private GameManager _instance;
        static public GameManager Instance => _instance;

        [Export] private PackedScene _startMenuPs;
        [Export] private PackedScene _worldListMenuPs;
        private GameState _curState;

        [Export] private PackedScene _playerPs;
        public Player Player;
        public ChunkManager ChunkManager;

        public override void _Ready()
        {
            _instance = this;
            GD.Print("Game Start ~~");
            _curState = GameState.StartMenu;


            //Player = _playerPs.Instantiate<Player>();
            //GetTree().CurrentScene.AddChild(Player);
        }

        public override void _Process(double delta)
        {
        }

        public void ChangeGameState(GameState newState)
        {
            switch (newState)
            {
                case GameState.StartMenu:
                    GetTree().ChangeSceneToPacked(_startMenuPs);
                    break;
                case GameState.WorldListMenu:
                    GetTree().ChangeSceneToPacked(_worldListMenuPs);
                    break;
                case GameState.Loading:
                    break;
                case GameState.Play:
                    break;
                case GameState.Stop:
                    break;
            }
        }
    }

}
