using Godot;
using Solo.Scripts.Character.Player;
using Solo.Scripts.System.ChunkSystem;
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
        [Export] private PackedScene _chunkManagerPs;
        [Export] private PackedScene _playerPs;

        [Export] private PauseView _pauseView;
        public Player Player;
        public ChunkManager ChunkManager;

        public override void _Ready()
        {
            _instance = this;
            GD.Print("Game Start ~~");
            _curState = GameState.StartMenu;

            _pauseView.Visible = false;
            //Player = _playerPs.Instantiate<Player>();
            //GetTree().CurrentScene.AddChild(Player);
            ProcessMode = ProcessModeEnum.Always;
        }

        public override void _PhysicsProcess(double delta)
        {
            switch (_curState)
            {
                case GameState.Play:
                    if (Input.IsActionJustPressed("Back") && Player.CurState != PlayerState.BagUI)
                    {
                        ChangeState(GameState.Pause);
                        return;
                    }
                    break;
                case GameState.Pause:
                    if (Input.IsActionJustPressed("Back"))
                    {
                        ChangeState(GameState.Play);
                        return;
                    }
                    break;
            }
        }

        public void ChangeState(GameState newState)
        {
            switch (_curState)
            {
                case GameState.StartMenu:
                    if (newState == GameState.SaveListMenu)
                    {
                        _curState = newState;
                        GetTree().ChangeSceneToPacked(_saveListMenuPs);
                        return;
                    }
                    break;
                case GameState.SaveListMenu:
                    if (newState == GameState.StartMenu)
                    {
                        _curState = newState;
                        GetTree().ChangeSceneToPacked(_startMenuPs);
                        return;
                    }
                    if (newState == GameState.Loading)
                    {
                        _curState = newState;
                        GetTree().ChangeSceneToPacked(_loadingViewPs);//加载世界
                        SaveManager.Instance.LoadSaveData();
                        ChangeState(GameState.Play);
                        return;
                    }
                    break;
                case GameState.Loading:
                    if (newState == GameState.Play)
                    {
                        _curState = newState;
                        GetTree().ChangeSceneToPacked(_mainLevelPs);
                    }
                    break;
                case GameState.Play:
                    if (newState == GameState.Pause)
                    {
                        _curState = newState;
                        _pauseView.Visible = true;
                        GetTree().Paused = true;
                        return;
                    }
                    break;
                case GameState.Pause:
                    if (newState == GameState.Play)
                    {
                        _curState = newState;
                        _pauseView.Visible = false;
                        GetTree().Paused = false;
                    }
                    if (newState == GameState.StartMenu)
                    {
                        _curState = newState;
                        GetTree().Paused = false;

                        SaveManager.Instance.CurSaveData.PlayerPosX = Player.GlobalPosition.X;
                        SaveManager.Instance.CurSaveData.PlayerPosY = Player.GlobalPosition.Y;
                        SaveManager.Instance.CurSaveData.BagInventoryGuidStr = Player.BagInventory.GuidStr;
                        SaveManager.Instance.CurSaveData.BagInventoryList = Player.BagInventory.ItemInstanceList;
                        SaveManager.Instance.CurSaveData.FastBarInventoryGuidStr = Player.FastBarInventory.GuidStr;
                        SaveManager.Instance.CurSaveData.FastBarInventoryList = Player.FastBarInventory.ItemInstanceList;
                        SaveManager.Instance.WriteCurSaveData();

                        GetTree().ChangeSceneToPacked(_startMenuPs);
                        _pauseView.Visible = false;
                        return;
                    }
                    break;
            }
        }
    }

}
