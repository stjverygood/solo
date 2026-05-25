using Godot;
using Solo.Scripts.Character;
using Solo.Scripts.System.SaveSystem;

namespace Solo.Scripts.Global
{

    public partial class GameManager : Node
    {
        static private GameManager _instance;
        static public GameManager Instance => _instance;
        private GameState _curState;

        [Export] private PackedScene _startMenuPs;//todo : 这里等到后面有美术资源了, 开屏变慢了, 要优化成场景路径, 动态加载packedScene
        [Export] private PackedScene _loadingViewPs;
        [Export] private PackedScene _saveListMenuPs;
        [Export] private PackedScene _mainLevelPs;
        [Export] private PackedScene _ChunkManagerPs;
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
            _curState = newState;
            switch (newState)
            {
                case GameState.StartMenu:
                    GetTree().ChangeSceneToPacked(_startMenuPs);
                    break;
                case GameState.SaveListMenu:
                    GetTree().ChangeSceneToPacked(_saveListMenuPs);
                    break;
                case GameState.Loading:
                    GetTree().ChangeSceneToPacked(_loadingViewPs);//加载世界
                    SaveManager.Instance.LoadSaveData();
                    ChangeGameState(GameState.Play);
                    break;
                case GameState.Play:
                    GetTree().ChangeSceneToPacked(_mainLevelPs);
                    //GD.Print("GetTree().CurrentScene : " + GetTree().CurrentScene);
                    //GetTree().CurrentScene.AddChild(Player);
                    //GetTree().CurrentScene.AddChild(ChunkManager);
                    break;
                case GameState.Stop:
                    break;
            }
        }

        //public void LoadGame(SaveInfo saveInfo)
        //{


        //}
    }

}
