using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.Levels;
using Solo.Scripts.System.ChunkSystem;
using Solo.Scripts.System.SaveSystem;
using System;
using System.Collections.Generic;

public struct EntityWeight
{
    public EntityType? Type;
    public float Weight;
}

public partial class World : Node2D
{
    [Export] private CanvasModulate _canvasModulate = null!;
    [Export] private Gradient _dayNightGradient = null!;

    private Dictionary<TileType, List<EntityWeight>> EntityWeigthListMap = new Dictionary<TileType, List<EntityWeight>>()
{
    {
        TileType.Grass,
        new List<EntityWeight>()
        {
            new EntityWeight(){ Type = null, Weight = 500 },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 20f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 5f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 1f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 1f },
            new EntityWeight(){ Type = EntityType.NormalMeleeEnemy, Weight = 1f },
            new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 1f },
            new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 1f },
            new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 1f },
        }
    },
    {
        TileType.Forest,
        new List<EntityWeight>()
        {
            new EntityWeight(){ Type = null, Weight = 500 },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 5f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 20f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 1f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 1f },
            new EntityWeight(){ Type = EntityType.NormalMeleeEnemy, Weight = 5f },
            new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 7f },
            new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 1f },
            new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 2f },
        }
    },
    {
        TileType.Stone,
        new List<EntityWeight>()
        {
            new EntityWeight(){ Type = null, Weight = 500 },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 1f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 1f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 20f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 10f },
            new EntityWeight(){ Type = EntityType.NormalMeleeEnemy, Weight = 4f },
            new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 2f },
            new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 8f },
            new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 2f },
        }
    },
    {
        TileType.Desert,
        new List<EntityWeight>()
        {
            new EntityWeight(){ Type = null, Weight = 500 },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 1f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 0f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 8f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 4f },
            new EntityWeight(){ Type = EntityType.NormalMeleeEnemy, Weight = 2f },
            new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 10f },
            new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 2f },
            new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 8f },
        }
    },
    {
        TileType.FireLand,
        new List<EntityWeight>()
        {
            new EntityWeight(){ Type = null, Weight = 500 },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 0f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 0f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 10f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 20f },
            new EntityWeight(){ Type = EntityType.NormalMeleeEnemy, Weight = 2f },
            new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 8f },
            new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 12f },
            new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 10f },
        }
    },
};

    //管理当前激活区块的实体
    public Dictionary<Vector2I, List<IEntity>> EntityMap = new();

    //区块实体的缓存, 卸载区块时把数据写入这个缓存, 区块加载时用这个缓存恢复, 游戏关闭后全部区块都要卸载, 会先写入缓存, 最后缓存再进入存档, 下次打开游戏, 也是先从存档加载缓存
    private Dictionary<Vector2I, List<EntitySaveData>> _entitySaveDataMap = new();

    public void Init()
    {
        GameManager.Instance.World = this;
        Player player = GameManager.Instance.PlayerPs.Instantiate<Player>();
        AddChild(player);
        if (SaveManager.Instance.CurSaveData.PlayerSaveData == null)
            player.Init(EntityType.Player, new Vector2(0, 0), null);
        else
            player.Init(EntityType.Player, new Vector2(0, 0), SaveManager.Instance.CurSaveData.PlayerSaveData);

        ChunkManager chunkManager = GameManager.Instance.ChunkManagerPs.Instantiate<ChunkManager>();
        AddChild(chunkManager);
        chunkManager.OnChunkLoaded += ChunkManager_OnChunkLoaded;
        chunkManager.OnChunkUnloaded += ChunkManager_OnChunkUnloaded;
        chunkManager.Init(player);
        foreach (ChunkEntitySaveData saveData in SaveManager.Instance.CurSaveData.ChunkEntitySaveDataList)
        {
            _entitySaveDataMap[new Vector2I(saveData.ChunkX, saveData.ChunkY)] = saveData.EntitySaveDataList;
        }
        chunkManager.Start();

        UIManager UIManager = GameManager.Instance.UIManagerPs.Instantiate<UIManager>();
        AddChild(UIManager);
        UIManager.Init();
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
        List<IEntity> entityList = EntityMap[chunkPos];
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
        EntityMap.Remove(chunkPos);
    }

    private void InitEntity(Vector2I chunkPos)
    {
        if (!EntityMap.ContainsKey(chunkPos))
            EntityMap[chunkPos] = new List<IEntity>();

        int chunkSize = GameManager.Instance.ChunkManager.ChunkSize;
        int tileSize = GameManager.Instance.ChunkManager.TileSize;

        Dictionary<TileType, List<Vector2I>> tilePosMap = new();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2I tilePos = new Vector2I(chunkPos.X * chunkSize + x, chunkPos.Y * chunkSize + y);
                TileType tileType = GameManager.Instance.ChunkManager.GetTileType(tilePos * tileSize);

                if (!tilePosMap.TryGetValue(tileType, out var posList))
                {
                    posList = new List<Vector2I>();
                    tilePosMap[tileType] = posList;
                }
                posList.Add(tilePos);
            }
        }

        foreach (KeyValuePair<TileType, List<Vector2I>> pair in tilePosMap)
        {
            if (!EntityWeigthListMap.TryGetValue(pair.Key, out List<EntityWeight>? entityWeightList) || entityWeightList == null)
                continue;

            float totalWeight = 0;
            foreach (EntityWeight entityWeight in entityWeightList)
                totalWeight += entityWeight.Weight;

            if (totalWeight <= 0)
                continue;

            // 关键改动1：先把该 tile 类型下的所有位置打乱一次，作为不放回抽样池
            List<Vector2I> shuffledPositions = new List<Vector2I>(pair.Value);
            ShuffleList(shuffledPositions);

            int totalTiles = shuffledPositions.Count;
            int cursor = 0; // 当前切片起始下标

            foreach (EntityWeight entityWeight in entityWeightList)
            {
                // null 表示"空地"，不生成实体，但仍要占用对应数量的位置，
                // 这样后面的实体类型才不会抢占这部分位置
                int count = (int)(entityWeight.Weight / totalWeight * totalTiles);

                if (count <= 0)
                    continue;

                // 防止越界（四舍五入误差导致 cursor+count 超过总数）
                count = Mathf.Min(count, totalTiles - cursor);
                if (count <= 0)
                    break;

                if (entityWeight.Type != null)
                {
                    // 关键改动2：直接从打乱后的位置池里切一段，不再单独随机，避免重复
                    for (int i = cursor; i < cursor + count; i++)
                    {
                        SpawnEntity((EntityType)entityWeight.Type, chunkPos, shuffledPositions[i], null);
                    }
                }

                cursor += count;
            }
        }
    }


    private void RecoverEntity(Vector2I chunkPos)
    {
        if (EntityMap.ContainsKey(chunkPos) == false)
            EntityMap[chunkPos] = new List<IEntity>();
        foreach (EntitySaveData entitySaveData in _entitySaveDataMap[chunkPos])
        {
            SpawnEntity(entitySaveData.Type, chunkPos, Vector2I.Zero, entitySaveData);
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


    private void SpawnEntity(EntityType type, Vector2I chunkPos, Vector2I tilePos, EntitySaveData? saveData)
    {
        PackedScene entityPs = GameManager.Instance.EntityPsMap[type];
        Vector2 tileCenterPos = tilePos * GameManager.Instance.ChunkManager.TileSize + new Vector2(GameManager.Instance.ChunkManager.TileSize / 2f, GameManager.Instance.ChunkManager.TileSize / 2f);
        IEntity entity = entityPs.Instantiate<IEntity>();
        GetTree().CurrentScene.AddChild((Node2D)entity);
        entity.Init(type, tileCenterPos, saveData);
        EntityMap[chunkPos].Add(entity);
    }


    // Fisher-Yates 洗牌，O(n)，比反复调用随机下标生成更高效也更安全
    private void ShuffleList<T>(List<T> list)
    {
        var rng = new Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}