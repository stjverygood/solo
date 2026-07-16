using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Trees;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.ChunkSystem;
using Solo.Scripts.System.SaveSystem;
using System.Collections.Generic;

public partial class World : Node2D
{
    [Export] private CanvasModulate _canvasModulate = null!;
    [Export] private Gradient _dayNightGradient = null!;


    private FastNoiseLite _noise = new FastNoiseLite();
    private string _seedString = "";

    //管理当前激活区块的实体
    private Dictionary<Vector2I, List<IEntity>> _entityMap = new();

    //区块实体的缓存, 卸载区块时把数据写入这个缓存, 区块加载时用这个缓存恢复, 游戏关闭后全部区块都要卸载, 会先写入缓存, 最后缓存再进入存档, 下次打开游戏, 也是先从存档加载缓存
    private Dictionary<Vector2I, List<EntitySaveData>> _entitySaveDataMap = new Dictionary<Vector2I, List<EntitySaveData>>();

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
        foreach (EntitySaveData entitySaveData in SaveManager.Instance.CurSaveData.EntitySaveDataList)
        {
            Vector2I chunkPos = GameManager.Instance.ChunkManager.WorldToChunkPos(new Vector2(entitySaveData.WorldX, entitySaveData.WorldY));
            if (_entitySaveDataMap.ContainsKey(chunkPos) == false)
                _entitySaveDataMap[chunkPos] = new List<EntitySaveData>();
            _entitySaveDataMap[chunkPos].Add(entitySaveData);
        }
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
                    if (GD.Randf() < 0.2)
                    {
                        Grass grass = GameManager.Instance.GrassPs.Instantiate<Grass>();
                        GetTree().CurrentScene.AddChild(grass);
                        grass.Init(tileCenterPos);
                        _entityMap[chunkPos].Add(grass);
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
                        tree.Init(tileCenterPos, 100, 100);
                        _entityMap[chunkPos].Add(tree);
                    }
                    continue;
                }

            }

    }
    private void RecoverEntity(Vector2I chunkPos)
    {
        //if (_entitySaveDataMap.ContainsKey(chunkPos) == false)
        //    _entitySaveDataMap[chunkPos] = new List<EntitySaveData>();
        //List<IEntity> entityList = _entityMap[chunkPos];
        //foreach (IEntity entity in entityList)
        //{
        //    if (entity is ISaveable saveable)
        //    {
        //        _entitySaveDataMap[chunkPos].Add(saveable.GetSaveData());
        //    }
        //    ((Node2D)entity).QueueFree();
        //}
        //_entityMap.Remove(chunkPos);
        if (_entityMap.ContainsKey(chunkPos) == false)
            _entityMap[chunkPos] = new List<IEntity>();
        foreach (EntitySaveData entitySaveData in _entitySaveDataMap[chunkPos])
        {
            switch (entitySaveData.Type)
            {
                case EntityType.Tree:
                    TreeSaveData treeSaveData = (TreeSaveData)entitySaveData;
                    Tree tree = GameManager.Instance.TreePs.Instantiate<Tree>();
                    GetTree().CurrentScene.AddChild(tree);
                    tree.Init(new Vector2(treeSaveData.WorldX, treeSaveData.WorldY), treeSaveData.CurHp, 100);
                    _entityMap[chunkPos].Add(tree);
                    break;
            }
        }
        _entitySaveDataMap.Remove(chunkPos);
    }
}
