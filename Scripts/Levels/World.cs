using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Levels;
using Solo.Scripts.System.ChunkSystem;
using Solo.Scripts.System.SaveSystem;
using System.Collections.Generic;

public partial class World : Node2D
{
    //[Export] private PackedScene _playerPs = null!;
    //[Export] private PackedScene _ChunkManagerPs = null!;
    [Export] private CanvasModulate _canvasModulate = null!;
    [Export] private Gradient _dayNightGradient = null!;


    private FastNoiseLite _noise = new FastNoiseLite();
    private string _seedString = "";

    //private Dictionary<Vector2I, List<IEntity>> _entityMap = new Dictionary<Vector2I, List<IEntity>>();//动态数据
    private Dictionary<Vector2I, List<EntitySaveData>> _entitySaveDataMap = new Dictionary<Vector2I, List<EntitySaveData>>();//存档数据, 用于恢复实体, 记录在这里说明加载过了, 来源 : 1. 存档初始化, 2. 区块卸载时

    public void Init()
    {

        Player player = GameManager.Instance.PlayerPs.Instantiate<Player>();
        AddChild(player);
        player.Init();
        ChunkManager chunkManager = GameManager.Instance.ChunkManagerPs.Instantiate<ChunkManager>();
        AddChild(chunkManager);
        chunkManager.OnChunkLoaded += ChunkManager_OnChunkLoaded;
        chunkManager.OnChunkUnloaded += ChunkManager_OnChunkUnloaded;
        chunkManager.Init();
        foreach (ChunkEntitySaveData chunkEntitySaveData in SaveManager.Instance.CurSaveData.ChunkEntitySaveDataList)
        {
            _entitySaveDataMap[new Vector2I(chunkEntitySaveData.ChunkPosX, chunkEntitySaveData.ChunkPosY)] = chunkEntitySaveData.EntitySaveDataList;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        //GD.Print("GameManager.Instance.TimeRatio : " + GameManager.Instance.TimeRatio);
        _canvasModulate.Color = _dayNightGradient.Sample(GameManager.Instance.TimeRatio);
        //if (Input.IsActionJustPressed("Back"))
        //{
        //    GetTree().Paused = true;
        //    _pauseView.Visible = true;
        //}
    }

    private void ChunkManager_OnChunkLoaded(Vector2I chunkPos)
    {
        if (_entitySaveDataMap.ContainsKey(chunkPos) == false)
        {
            InitEntity(chunkPos);
        }
        else
        {
            RecoverEntity(chunkPos);
        }
    }

    private void ChunkManager_OnChunkUnloaded(Vector2I chunkPos)
    {
        //todo : 写入动态表里, 退出时写入存档
        _entitySaveDataMap[chunkPos] = new List<EntitySaveData>();
    }

    private void InitEntity(Vector2I chunkPos)
    {
        for (int x = 0; x < GameManager.Instance.ChunkManager.ChunkSize; x++)
            for (int y = 0; y < GameManager.Instance.ChunkManager.ChunkSize; y++)
            {

                Vector2I tilePos = new Vector2I(chunkPos.X * GameManager.Instance.ChunkManager.ChunkSize + x, chunkPos.Y * GameManager.Instance.ChunkManager.ChunkSize + y);
                TileType tileType = GameManager.Instance.ChunkManager.GetTileType(tilePos * GameManager.Instance.ChunkManager.TileSize);

                Vector2 tileCenterPos = tilePos * GameManager.Instance.ChunkManager.TileSize + new Vector2(GameManager.Instance.ChunkManager.TileSize / 2f, GameManager.Instance.ChunkManager.TileSize / 2f);
                if (tileType == TileType.Grass)
                {
                    if (GD.Randf() < 0.2)
                    {
                        Grass grass = GameManager.Instance.GrassPs.Instantiate<Grass>();
                        GetTree().CurrentScene.AddChild(grass);
                        grass.Init(tileCenterPos);
                    }
                    //if (GD.Randf() < 0.01)
                    //{
                    //    Zombie zombie = GameManager.Instance.ZombiePs.Instantiate<Zombie>();
                    //    GetTree().CurrentScene.AddChild(zombie);
                    //    zombie.Init(tileCenterPos);
                    //}
                    continue;
                }
                if (tileType == TileType.Forest)
                {
                    if (GD.Randf() < 0.2)
                    {
                        Tree tree = GameManager.Instance.TreePs.Instantiate<Tree>();
                        GetTree().CurrentScene.AddChild(tree);
                        tree.Init(tileCenterPos);
                    }
                    continue;
                }

            }

    }
    private void RecoverEntity(Vector2I chunkPos)
    {

    }
}
