using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Grasses;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.Levels;
using Solo.Scripts.System.ChunkSystem;
using Solo.Scripts.System.SaveSystem;
using System.Collections.Generic;
using Tree = Solo.Scripts.Entities.Trees.Tree;

public partial class World : Node2D
{
    [Export] private CanvasModulate _canvasModulate = null!;
    [Export] private Gradient _dayNightGradient = null!;


    private FastNoiseLite _noise = new FastNoiseLite();
    private string _seedString = "";

    //管理当前激活区块的实体
    private Dictionary<Vector2I, List<IEntity>> _entityMap = new();

    //private HashSet<Vector2I> _initedChunkPosSet = new();

    //区块实体的缓存, 卸载区块时把数据写入这个缓存, 区块加载时用这个缓存恢复, 游戏关闭后全部区块都要卸载, 会先写入缓存, 最后缓存再进入存档, 下次打开游戏, 也是先从存档加载缓存
    //private Dictionary<Vector2I, List<EntitySaveData>> _entitySaveDataMap = new Dictionary<Vector2I, List<EntitySaveData>>();
    private Dictionary<Vector2I, List<EntitySaveData>> _entitySaveDataMap = new();

    public void Init()
    {
        GameManager.Instance.World = this;
        Player player = GameManager.Instance.PlayerPs.Instantiate<Player>();
        AddChild(player);
        if (SaveManager.Instance.CurSaveData.PlayerSaveData == null)
            player.Init(new Vector2(0, 0), (PlayerData)EntityDataManager.Instance.GetData(EntityType.Player), null);
        else
            player.Init(new Vector2(0, 0), (PlayerData)EntityDataManager.Instance.GetData(EntityType.Player), SaveManager.Instance.CurSaveData.PlayerSaveData);





        ChunkManager chunkManager = GameManager.Instance.ChunkManagerPs.Instantiate<ChunkManager>();
        AddChild(chunkManager);
        chunkManager.OnChunkLoaded += ChunkManager_OnChunkLoaded;
        chunkManager.OnChunkUnloaded += ChunkManager_OnChunkUnloaded;
        chunkManager.Init(player);

        foreach (ChunkEntitySaveData saveData in SaveManager.Instance.CurSaveData.ChunkEntitySaveDataList)
        {
            //if (_entitySaveDataMap.TryGetValue(new Vector2I(saveData.ChunkX, saveData.ChunkY), out List<EntitySaveData>? entitySaveDataList) == false)
            //{
            //    _entitySaveDataMap[new Vector2I(saveData.ChunkX, saveData.ChunkY)] = new List<EntitySaveData>();
            //}
            //if (_chunkEntitySaveDataMap.TryGetValue(new Vector2I(saveData.ChunkX, saveData.ChunkY), out var))
            //    _chunkEntitySaveDataMap[].EntitySaveDataList = saveData.EntitySaveDataList;
            _entitySaveDataMap[new Vector2I(saveData.ChunkX, saveData.ChunkY)] = saveData.EntitySaveDataList;
        }

        //foreach (EntitySaveData entitySaveData in SaveManager.Instance.CurSaveData.EntitySaveDataList)
        //{
        //    Vector2I chunkPos = GameManager.Instance.ChunkManager.WorldToChunkPos(new Vector2(entitySaveData.WorldX, entitySaveData.WorldY));
        //    if (_entitySaveDataMap.ContainsKey(chunkPos) == false)
        //        _entitySaveDataMap[chunkPos] = new List<EntitySaveData>();
        //    _entitySaveDataMap[chunkPos].Add(entitySaveData);
        //}

        chunkManager.Start();

    }

    public override void _PhysicsProcess(double delta)
    {
        _canvasModulate.Color = _dayNightGradient.Sample(GameManager.Instance.TimeRatio);
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
        if (_entitySaveDataMap.ContainsKey(chunkPos) == false)
            _entitySaveDataMap[chunkPos] = new List<EntitySaveData>();
        List<IEntity> entityList = _entityMap[chunkPos];
        foreach (IEntity entity in entityList)
        {
            if (IsInstanceValid((Node2D)entity) == false)
                continue;
            if (entity is ISaveable saveable)
            {
                _entitySaveDataMap[chunkPos].Add(saveable.GetSaveData());
            }
            ((Node2D)entity).QueueFree();
        }
        _entityMap.Remove(chunkPos);
    }

    private void InitEntity(Vector2I chunkPos)
    {
        if (_entityMap.ContainsKey(chunkPos) == false)
            _entityMap[chunkPos] = new List<IEntity>();

        for (int x = 0; x < GameManager.Instance.ChunkManager.ChunkSize; x++)
            for (int y = 0; y < GameManager.Instance.ChunkManager.ChunkSize; y++)
            {
                Vector2I tilePos = new Vector2I(chunkPos.X * GameManager.Instance.ChunkManager.ChunkSize + x, chunkPos.Y * GameManager.Instance.ChunkManager.ChunkSize + y);
                TileType tileType = GameManager.Instance.ChunkManager.GetTileType(tilePos * GameManager.Instance.ChunkManager.TileSize);

                Vector2 tileCenterPos = tilePos * GameManager.Instance.ChunkManager.TileSize + new Vector2(GameManager.Instance.ChunkManager.TileSize / 2f, GameManager.Instance.ChunkManager.TileSize / 2f);
                if (tileType == TileType.Grass)
                {
                    float rd = GD.Randf();
                    if (rd < 0.05)
                    {
                        Grass grass = GameManager.Instance.GrassPs.Instantiate<Grass>();
                        GetTree().CurrentScene.AddChild(grass);
                        grass.Init(tileCenterPos);
                        _entityMap[chunkPos].Add(grass);
                        continue;
                    }
                    if (rd < 0.07)
                    {
                        Tree tree = GameManager.Instance.TreePs.Instantiate<Tree>();
                        GetTree().CurrentScene.AddChild(tree);
                        tree.Init(tileCenterPos);
                        _entityMap[chunkPos].Add(tree);
                        continue;
                    }

                    //if (GD.Randf() < 0.01)
                    //{
                    //    Zombie zombie = GameManager.Instance.ZombiePs.Instantiate<Zombie>();
                    //    GetTree().CurrentScene.AddChild(zombie);
                    //    zombie.Init(tileCenterPos);
                    //}

                }
                if (tileType == TileType.Forest)
                {
                    float rd = GD.Randf();
                    if (rd < 0.01)
                    {
                        Grass grass = GameManager.Instance.GrassPs.Instantiate<Grass>();
                        GetTree().CurrentScene.AddChild(grass);
                        grass.Init(tileCenterPos);
                        _entityMap[chunkPos].Add(grass);
                        continue;
                    }
                    if (GD.Randf() < 0.05)
                    {
                        Tree tree = GameManager.Instance.TreePs.Instantiate<Tree>();
                        GetTree().CurrentScene.AddChild(tree);
                        tree.Init(tileCenterPos);
                        _entityMap[chunkPos].Add(tree);
                    }
                    continue;
                }

            }

    }
    private void RecoverEntity(Vector2I chunkPos)
    {
        if (_entityMap.ContainsKey(chunkPos) == false)
            _entityMap[chunkPos] = new List<IEntity>();
        foreach (EntitySaveData entitySaveData in _entitySaveDataMap[chunkPos])
        {
            switch (entitySaveData.Type)
            {
                case EntityType.Grass:
                    Grass grass = GameManager.Instance.GrassPs.Instantiate<Grass>();
                    GetTree().CurrentScene.AddChild(grass);
                    grass.Init(Vector2.Zero, entitySaveData);
                    _entityMap[chunkPos].Add(grass);
                    break;
                case EntityType.Tree:
                    Tree tree = GameManager.Instance.TreePs.Instantiate<Tree>();
                    GetTree().CurrentScene.AddChild(tree);
                    tree.Init(Vector2.Zero, entitySaveData);
                    _entityMap[chunkPos].Add(tree);
                    break;
            }
        }
        _entitySaveDataMap.Remove(chunkPos);
    }

    public void EntitySaveDataCacheToSave()
    {
        //区块实体
        SaveManager.Instance.CurSaveData.ChunkEntitySaveDataList.Clear();
        foreach (KeyValuePair<Vector2I, List<EntitySaveData>> pair in _entitySaveDataMap)
        {
            SaveManager.Instance.CurSaveData.ChunkEntitySaveDataList.Add(new ChunkEntitySaveData()
            {
                ChunkX = pair.Key.X,
                ChunkY = pair.Key.Y,
                EntitySaveDataList = pair.Value
            });
        }

        //玩家
        SaveManager.Instance.CurSaveData.PlayerSaveData = (PlayerSaveData)GameManager.Instance.Player.GetSaveData();

        SaveManager.Instance.WriteCurSaveData();
    }

    //public override void _ExitTree()
    //{
    //    GD.Print("world _ExitTree");
    //    List<EntitySaveData> entitySaveDataList = new List<EntitySaveData>();
    //    foreach (List<EntitySaveData> curEntitySaveDataList in _entitySaveDataMap.Values)
    //    {
    //        foreach (EntitySaveData saveData in curEntitySaveDataList)
    //            entitySaveDataList.Add(saveData);
    //    }
    //    SaveManager.Instance.CurSaveData.EntitySaveDataList = entitySaveDataList;
    //    SaveManager.Instance.WriteCurSaveData();
    //}
}
