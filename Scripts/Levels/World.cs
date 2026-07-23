using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.Levels;
using Solo.Scripts.System.ChunkSystem;
using Solo.Scripts.System.SaveSystem;
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
            new EntityWeight(){ Type = null, Weight = 2000f },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 20f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 8f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 5f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 2f },
            new EntityWeight(){ Type = EntityType.NormalMeleeEnemy, Weight = 8f },
            new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 4f },
            new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 1f },
            new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 2f },
        }
    },
    {
        TileType.Forest,
        new List<EntityWeight>()
        {
            new EntityWeight(){ Type = null, Weight = 2000f },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 15f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 35f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 3f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 2f },
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
            new EntityWeight(){ Type = null, Weight = 2000f },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 1f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 1f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 25f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 12f },
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
            new EntityWeight(){ Type = null, Weight = 2000f },
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
            new EntityWeight(){ Type = null, Weight = 2000f },
            new EntityWeight(){ Type = EntityType.Grass, Weight = 0f },
            new EntityWeight(){ Type = EntityType.Tree, Weight = 0f },
            new EntityWeight(){ Type = EntityType.Stone, Weight = 10f },
            new EntityWeight(){ Type = EntityType.Ore, Weight = 18f },
            new EntityWeight(){ Type = EntityType.NormalMeleeEnemy, Weight = 2f },
            new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 8f },
            new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 12f },
            new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 10f },
        }
    },
};

    //管理当前激活区块的实体
    private Dictionary<Vector2I, List<IEntity>> _entityMap = new();

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
        if (!_entityMap.ContainsKey(chunkPos))
            _entityMap[chunkPos] = new List<IEntity>();

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
            // 检查当前瓦片类型是否有对应的生成权重配置
            if (!EntityWeigthListMap.TryGetValue(pair.Key, out List<EntityWeight>? entityWeightList) || entityWeightList == null)
                continue;
            float totalWeight = 0;
            foreach (EntityWeight entityWeight in entityWeightList)
                totalWeight += entityWeight.Weight;
            if (totalWeight <= 0) continue;
            Dictionary<EntityType, int> entityCountMap = new();
            foreach (EntityWeight entityWeight in entityWeightList)
            {
                if (entityWeight.Type == null)
                    continue;
                entityCountMap[(EntityType)entityWeight.Type] = (int)(entityWeight.Weight / totalWeight * pair.Value.Count);
            }
            foreach (KeyValuePair<EntityType, int> typeCountPair in entityCountMap)
            {
                if (typeCountPair.Value <= 0) continue;
                List<int> randomIndexList = GetRandomIndexList(pair.Value.Count, typeCountPair.Value);
                foreach (int index in randomIndexList)
                {
                    SpawnEntity(typeCountPair.Key, chunkPos, pair.Value[index], null);
                }
            }
        }
    }
    private void RecoverEntity(Vector2I chunkPos)
    {
        if (_entityMap.ContainsKey(chunkPos) == false)
            _entityMap[chunkPos] = new List<IEntity>();
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
        _entityMap[chunkPos].Add(entity);
    }


    /// <summary>
    /// 从 [0, totalCount - 1] 范围内随机抽取 count 个不重复的索引
    /// </summary>
    public static List<int> GetRandomIndexList(int totalCount, int count)
    {
        if (count >= totalCount)
        {
            List<int> allIndices = new(totalCount);
            for (int i = 0; i < totalCount; i++) allIndices.Add(i);
            return allIndices;
        }
        List<int> pool = new(totalCount);
        for (int i = 0; i < totalCount; i++) pool.Add(i);
        List<int> result = new(count);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = (int)(GD.Randi() % pool.Count);
            result.Add(pool[randomIndex]);
            pool[randomIndex] = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
        }

        return result;
    }
}