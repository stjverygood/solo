using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Units;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.ChunkSystem;
using Solo.Scripts.System.RealmSystem;
using Solo.Scripts.System.SaveSystem;
using Solo.Scripts.System.UiSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solo.Scripts.Global
{

    public partial class GameManager : Node
    {
        static private GameManager _instance;
        static public GameManager Instance => _instance;
        private GameState _curState;
        public bool IsDebugMode = true;

        [Export] public PackedScene DropItemPs;
        [Export] public PackedScene FloatTextLbPs;
        [Export] private PackedScene _startMenuPs;//todo : 这里等到后面有美术资源了, 开屏变慢了, 要优化成场景路径, 动态加载packedScene
        [Export] private PackedScene _loadingViewPs;
        [Export] private PackedScene _saveListMenuPs;
        [Export] private PackedScene _mainLevelPs;
        [Export] private PauseView _pauseView;


        public Player Player;
        public List<Unit> UnitList = new List<Unit>();
        public List<IQiRangeable> IQiRangeableList = new List<IQiRangeable>();
        public ChunkManager ChunkManager;
        public BuildingManager BuildingManager;

        //vfx
        [Export] public PackedScene ExplosionEffectPs;
        [Export] public PackedScene SmogEffectPs;

        public override void _Ready()
        {
            _instance = this;
            GD.Print("Game Start ~~");
            _curState = GameState.StartMenu;

            _pauseView.Visible = false;
            ProcessMode = ProcessModeEnum.Always;

            //
            foreach (RealmType realm in Enum.GetValues<RealmType>())
            {
                RealmData realmData = RealmDataManager.Instance.GetData(realm);
                GD.Print($"{realmData.Name} Atk : {realmData.Atk}");
            }
            GD.Print($" ");
            foreach (RealmType realm in Enum.GetValues<RealmType>())
            {
                RealmData realmData = RealmDataManager.Instance.GetData(realm);
                GD.Print($"{realmData.Name} Def : {realmData.Def}");
            }
            GD.Print($" ");
            foreach (RealmType realm in Enum.GetValues<RealmType>())
            {
                RealmData realmData = RealmDataManager.Instance.GetData(realm);
                GD.Print($"{realmData.Name} MaxHp : {realmData.MaxHp}");
            }
            GD.Print($" ");
            foreach (RealmType realm in Enum.GetValues<RealmType>())
            {
                RealmData realmData = RealmDataManager.Instance.GetData(realm);
                GD.Print($"{realmData.Name} MaxQi : {realmData.MaxQi}");
            }
            GD.Print($" ");
            foreach (RealmType realm in Enum.GetValues<RealmType>())
            {
                RealmData realmData = RealmDataManager.Instance.GetData(realm);
                GD.Print($"{realmData.Name} MaxExp : {realmData.MaxExp}");
            }
            //

            foreach (RealmType realm in Enum.GetValues<RealmType>())
            {
                RealmData realmData = RealmDataManager.Instance.GetData(realm);
                GD.Print($"{realmData.Name} : ");
                GD.Print($"攻 : {realmData.Atk}");
                GD.Print($"御 : {realmData.Def}");
                GD.Print($"血 : {realmData.MaxHp}");
                GD.Print($"气 : {realmData.MaxQi}");
                GD.Print($"修 : {realmData.MaxExp}");
                GD.Print($"");
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            UpdateGameTime((float)delta);
            //GD.Print("UnitList.Count : " + UnitList.Count);
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
                    //if (newState == GameState.StartMenu)
                    //{
                    //    _curState = newState;
                    //    GetTree().Paused = false;

                    //    SaveManager.Instance.CurSaveData.PlayerSaveData = Player.GetSaveData();

                    //    ChunkManager.SaveActiveChunk();
                    //    SaveManager.Instance.CurSaveData.ChunkSaveDataList = ChunkManager.ChunkSaveDataMap.Values.ToList();
                    //    SaveManager.Instance.WriteCurSaveData();

                    //    GetTree().ChangeSceneToPacked(_startMenuPs);
                    //    _pauseView.Visible = false;
                    //    return;
                    //}
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

                        SaveManager.Instance.CurSaveData.PlayerSaveData = Player.GetSaveData();
                        ChunkManager.SaveActiveChunk();
                        SaveManager.Instance.CurSaveData.ChunkSaveDataList = ChunkManager.ChunkSaveDataMap.Values.ToList();
                        SaveManager.Instance.WriteCurSaveData();

                        GetTree().ChangeSceneToPacked(_startMenuPs);
                        _pauseView.Visible = false;
                        return;
                    }
                    break;
            }
        }

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
        public float GetDamage(float atk, float def)
        {
            const float C = 100f;
            float damage = atk * C / (C + def);
            return Mathf.Max(1f, damage);//保底机制：哪怕防御再高，打中也应该至少有 1 点伤害
        }
    }

}
