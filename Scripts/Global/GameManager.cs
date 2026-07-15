using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Units;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.ChunkSystem;
using Solo.Scripts.System.ItemSystem;
using Solo.Scripts.System.SaveSystem;
using Solo.Scripts.System.UiSystem;
using System.Collections.Generic;

namespace Solo.Scripts.Global
{
    public enum GameState
    {
        StartMenu,//开始菜单
        SaveListMenu,//世界列表
        SettingMenu,
        Loading,//加载界面 : 加载世界, 退出并保存世界
        Play,//游戏中
        Pause,//按exc暂停游戏
    }

    public partial class GameManager : Node
    {
        static private GameManager _instance;
        static public GameManager Instance => _instance;
        private GameState _curState;
        public bool IsDebugMode = true;

        [Export] public PackedScene DropItemPs = null!;
        [Export] public PackedScene FloatTextLbPs = null!;
        [Export] private PackedScene _startMenuPs = null!;//todo : 这里等到后面有美术资源了, 开屏变慢了, 要优化成场景路径, 动态加载packedScene
        [Export] private PackedScene _loadingViewPs = null!;
        [Export] private PackedScene _saveListMenuPs = null!;
        [Export] private PackedScene _mainLevelPs = null!;
        [Export] private PauseView _pauseView = null!;


        [Export] private PackedScene _mainMenuViewPs = null!;
        [Export] private PackedScene _worldPs = null!;

        [Export] public PackedScene ChunkManagerPs = null!;


        public Player Player;
        public List<Unit> UnitList = new List<Unit>();
        public List<IQiRangeable> IQiRangeableList = new List<IQiRangeable>();
        public ChunkManager ChunkManager;
        public BuildingManager BuildingManager;

        //vfx
        [Export] public PackedScene ExplosionEffectPs = null!;
        [Export] public PackedScene SmogEffectPs = null!;


        //projectile
        [Export] private PackedScene _arrowPs = null!;
        [Export] private PackedScene _fireballPs = null!;
        [Export] private PackedScene _swordWavePs = null!;
        [Export] private PackedScene _hammerWavePs = null!;

        //dropitem
        [Export] private PackedScene _expBallPs = null!;

        //entity
        [Export] public PackedScene PlayerPs = null!;
        [Export] public PackedScene TreePs = null!;
        [Export] public PackedScene GrassPs = null!;
        [Export] public PackedScene ZombiePs = null!;


        public override void _Ready()
        {
            _instance = this;
            GD.Print("GameManager _Ready!");
            _curState = GameState.StartMenu;

            _pauseView.Visible = false;
            ProcessMode = ProcessModeEnum.Always;
        }

        public override void _PhysicsProcess(double delta)
        {
            UpdateGameTime((float)delta);
            //GD.Print("UnitList.Count : " + UnitList.Count);
            //switch (_curState)
            //{
            //    case GameState.Play:
            //        if (Input.IsActionJustPressed("Back") && Player.CurState != PlayerState.BagUI)
            //        {
            //            ChangeState(GameState.Pause);
            //            return;
            //        }
            //        break;
            //    case GameState.Pause:
            //        if (Input.IsActionJustPressed("Back"))
            //        {
            //            ChangeState(GameState.Play);
            //            return;
            //        }
            //        break;
            //}
        }


        public void ReturnMainMenu()
        {
            MainMenuView mainMenuView = _mainMenuViewPs.Instantiate<MainMenuView>();
            GetTree().Root.AddChild(mainMenuView);
            GetTree().CurrentScene.QueueFree();
            GetTree().CurrentScene = mainMenuView;
            mainMenuView.Init();
        }
        public void EnterWorld()
        {
            SaveManager.Instance.LoadSaveData();
            World world = _worldPs.Instantiate<World>();
            GetTree().Root.AddChild(world);
            GetTree().CurrentScene.QueueFree();
            GetTree().CurrentScene = world;
            world.Init();
        }




        //public void ChangeState(GameState newState)
        //{
        //    switch (_curState)
        //    {
        //        case GameState.StartMenu:
        //            if (newState == GameState.SaveListMenu)
        //            {
        //                _curState = newState;
        //                GetTree().ChangeSceneToPacked(_saveListMenuPs);
        //                return;
        //            }
        //            break;
        //        case GameState.SaveListMenu:
        //            if (newState == GameState.StartMenu)
        //            {
        //                _curState = newState;
        //                GetTree().ChangeSceneToPacked(_startMenuPs);
        //                return;
        //            }
        //            if (newState == GameState.Loading)
        //            {
        //                _curState = newState;
        //                GetTree().ChangeSceneToPacked(_loadingViewPs);//加载世界
        //                SaveManager.Instance.LoadSaveData();
        //                ChangeState(GameState.Play);
        //                return;
        //            }
        //            break;
        //        case GameState.Loading:
        //            if (newState == GameState.Play)
        //            {
        //                _curState = newState;
        //                MainLevel mainLevel = _mainLevelPs.Instantiate<MainLevel>();
        //                GetTree().Root.AddChild(mainLevel);
        //                GetTree().CurrentScene?.QueueFree();
        //                GetTree().CurrentScene = mainLevel;

        //            }
        //            break;
        //        case GameState.Play:
        //            if (newState == GameState.Pause)
        //            {
        //                _curState = newState;
        //                _pauseView.Visible = true;
        //                GetTree().Paused = true;
        //                return;
        //            }
        //            //if (newState == GameState.StartMenu)
        //            //{
        //            //    _curState = newState;
        //            //    GetTree().Paused = false;

        //            //    SaveManager.Instance.CurSaveData.PlayerSaveData = Player.GetSaveData();

        //            //    ChunkManager.SaveActiveChunk();
        //            //    SaveManager.Instance.CurSaveData.ChunkSaveDataList = ChunkManager.ChunkSaveDataMap.Values.ToList();
        //            //    SaveManager.Instance.WriteCurSaveData();

        //            //    GetTree().ChangeSceneToPacked(_startMenuPs);
        //            //    _pauseView.Visible = false;
        //            //    return;
        //            //}
        //            break;
        //        case GameState.Pause:
        //            if (newState == GameState.Play)
        //            {
        //                _curState = newState;
        //                _pauseView.Visible = false;
        //                GetTree().Paused = false;
        //            }
        //            if (newState == GameState.StartMenu)
        //            {
        //                _curState = newState;
        //                GetTree().Paused = false;

        //                SaveManager.Instance.CurSaveData.PlayerSaveData = Player.GetSaveData();
        //                ChunkManager.SaveActiveChunk();
        //                SaveManager.Instance.CurSaveData.ChunkSaveDataList = ChunkManager.ChunkSaveDataMap.Values.ToList();
        //                SaveManager.Instance.WriteCurSaveData();

        //                GetTree().ChangeSceneToPacked(_startMenuPs);
        //                _pauseView.Visible = false;
        //                return;
        //            }
        //            break;
        //    }
        //}

        //时间系统
        public float TimeRatio = 0;//0天黑, 0.25黎明, 0.5中午, 0.75日落, 1天黑; 白天 : 0.25-0.75, 晚上 : 0.75 - 后一天0.25
        private float DayDuration = 60f * 3;
        private float _curTime = 0;
        private void UpdateGameTime(float delta)
        {
            _curTime += delta;
            if (_curTime >= DayDuration)
                _curTime -= DayDuration;
            TimeRatio = _curTime / DayDuration;
        }


        //伤害公式
        public float CalculateDamage(float atk, float def)
        {
            const float C = 100f;
            float damage = atk * C / (C + def);
            return Mathf.Max(1f, damage);//保底机制：哪怕防御再高，打中也应该至少有 1 点伤害
        }

        //生成发射物
        public void SpawnSwordWave(IEntity spawner, Vector2 atkDir)
        {
            SwordWave swordWave = _swordWavePs.Instantiate<SwordWave>();
            GetTree().CurrentScene.AddChild(swordWave);
            swordWave.Init(spawner, atkDir);
        }

        public void SpawnExpBall(Vector2 worldPos, float exp)
        {
            ExpBall expBall = _expBallPs.Instantiate<ExpBall>();
            GetTree().CurrentScene.AddChild(expBall);
            expBall.Init(worldPos, exp);
        }
    }

}
