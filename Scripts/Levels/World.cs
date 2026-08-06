using Godot;
using Solo.Scripts.Entities.Components.PositionComponents;
using Solo.Scripts.Entities.Components.StartPositionComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.Levels;
using Solo.Scripts.System.ChunkSystem;
using Solo.Scripts.System.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;

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
            //new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 1f },
            //new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 1f },
            //new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 1f },
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
            //new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 7f },
            //new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 1f },
            //new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 2f },
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
            //new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 2f },
            //new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 8f },
            //new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 2f },
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
            //new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 10f },
            //new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 2f },
            //new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 8f },
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
            //new EntityWeight(){ Type = EntityType.SpeedMeleeEnemy, Weight = 8f },
            //new EntityWeight(){ Type = EntityType.StrongMeleeEnemy, Weight = 12f },
            //new EntityWeight(){ Type = EntityType.NormalRangedEnemy, Weight = 10f },
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
        Player player = GameManager.Instance.EntityPsMap[EntityType.Player].Instantiate<Player>();
        AddChild(player);
        if (SaveManager.Instance.CurSaveData.PlayerSaveData == null)
        {
            player.Init(EntityType.Player, null);
            player.Core.GetComponent<StartPositionComponent>().StartPositon = new Vector2(0, 0);
            player.Core.GetComponent<PositionComponent>().InitWorldPosition(new Vector2(0, 0));
        }
        else
            player.Init(EntityType.Player, SaveManager.Instance.CurSaveData.PlayerSaveData);

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

        RefreshEntityChunkMap((float)delta);
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
            _entitySaveDataMap[chunkPos].Add(entity.Core.GetEntitySaveData());
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
            if (!EntityWeigthListMap.TryGetValue(pair.Key, out List<EntityWeight>? entityWeightList) || entityWeightList == null)
                continue;

            float totalWeight = 0;
            foreach (EntityWeight entityWeight in entityWeightList)
                totalWeight += entityWeight.Weight;

            if (totalWeight <= 0)
                continue;

            List<Vector2I> shuffledPositions = new List<Vector2I>(pair.Value);
            ShuffleList(shuffledPositions);
            int totalTiles = shuffledPositions.Count;
            int cursor = 0; // 当前切片起始下标
            foreach (EntityWeight entityWeight in entityWeightList)
            {
                int count = (int)(entityWeight.Weight / totalWeight * totalTiles);
                if (count <= 0)
                    continue;
                count = Mathf.Min(count, totalTiles - cursor);
                if (count <= 0)
                    break;
                if (entityWeight.Type != null)
                {
                    for (int i = cursor; i < cursor + count; i++)
                    {
                        Vector2 worldPos = shuffledPositions[i] * GameManager.Instance.ChunkManager.TileSize + new Vector2(GameManager.Instance.ChunkManager.TileSize / 2f, GameManager.Instance.ChunkManager.TileSize / 2f);
                        IEntity entity = SpawnEntity((EntityType)entityWeight.Type);
                        entity.Init((EntityType)entityWeight.Type, null);
                        if (entity.Core.TryGetComponent<PositionComponent>(out PositionComponent posComp) == true)
                            posComp.InitWorldPosition(worldPos);
                    }
                }
                cursor += count;
            }
        }
    }

    private void RecoverEntity(Vector2I chunkPos)
    {
        if (_entityMap.ContainsKey(chunkPos) == false)
            _entityMap[chunkPos] = new List<IEntity>();
        foreach (EntitySaveData entitySaveData in _entitySaveDataMap[chunkPos])
        {
            IEntity entity = SpawnEntity(entitySaveData.Type);
            entity.Init(entitySaveData.Type, entitySaveData);
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
        SaveManager.Instance.CurSaveData.PlayerSaveData = GameManager.Instance.Player.Core.GetEntitySaveData();
        SaveManager.Instance.WriteCurSaveData();
    }

    // 实体移动(如敌人的MoveAndSlide)不会主动上报区块, 由World定期校准归属
    // 卸载是玩家距离驱动(4区块), 实体跨区块后最多1秒即被修正, 窗口不可达
    private float _entityChunkRefreshTimer = 0;
    private float _entityChunkRefreshDuration = 1f; // 与ChunkManager卸载检查同频
    private void RefreshEntityChunkMap(float delta)
    {
        _entityChunkRefreshTimer += delta;
        if (_entityChunkRefreshTimer < _entityChunkRefreshDuration)
            return;
        _entityChunkRefreshTimer = 0;
        foreach (KeyValuePair<Vector2I, List<IEntity>> pair in _entityMap.ToList())
        {
            foreach (IEntity entity in pair.Value.ToList())
            {
                if (entity.Core.Type == EntityType.Player)
                    continue;
                if (IsInstanceValid((Node2D)entity) == false)
                {
                    pair.Value.Remove(entity); // 已销毁实体从账本清理
                    continue;
                }
                PositionComponent posComp = entity.Core.GetComponent<PositionComponent>();
                Vector2I newChunkPos = GameManager.Instance.ChunkManager.WorldToChunkPos(posComp.GetWorldPosition());
                if (newChunkPos != pair.Key)
                {
                    _entityMap[pair.Key].Remove(entity);
                    if (_entityMap.TryGetValue(newChunkPos, out List<IEntity>? entityList) == false)
                        entityList = new List<IEntity>();
                    entityList.Add(entity);
                    _entityMap[newChunkPos] = entityList;
                }
            }
        }
    }
    public void InitChunkPos(Vector2 worldPos, IEntity entity)
    {
        if (entity.Core.Type == EntityType.Player)
            return;
        Vector2I chunkPos = GameManager.Instance.ChunkManager.WorldToChunkPos(worldPos);
        if (_entityMap.ContainsKey(chunkPos) == false)
            _entityMap[chunkPos] = new List<IEntity>();
        _entityMap[chunkPos].Add(entity);
    }
    //public void RefreshEntityChunk(IEntity entity)
    //{
    //    if (entity.Core.Type == EntityType.Player)
    //        return;
    //    PositionComponent posComp = entity.Core.GetComponent<PositionComponent>();
    //    Vector2I oldChunkPos = posComp.CurChunkPos;
    //    Vector2I newChunkPos = GameManager.Instance.ChunkManager.WorldToChunkPos(posComp.GetWorldPosition());
    //    if (_entityMap.TryGetValue(oldChunkPos, out List<IEntity>? oldList))
    //        oldList.Remove(entity);
    //    if (_entityMap.TryGetValue(newChunkPos, out List<IEntity>? newList))
    //    {
    //        // 直加路径(如SpawnDropItem)未设置CurChunkPos, 首次校正时会重复添加
    //        if (newList.Contains(entity) == false)
    //            newList.Add(entity);
    //    }
    //    else
    //    {
    //        _entityMap[newChunkPos] = new List<IEntity>() { entity };
    //    }
    //    posComp.CurChunkPos = newChunkPos;
    //}

    public IEntity SpawnEntity(EntityType type)
    {
        PackedScene entityPs = GameManager.Instance.EntityPsMap[type];
        IEntity entity = entityPs.Instantiate<IEntity>();
        GetTree().CurrentScene.AddChild((Node2D)entity);

        return entity;
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


    //public void SpawnDropItem(Vector2 worldPos, List<DropItemDropInfo> dropInfoList)
    //{
    //    foreach (DropItemDropInfo info in dropInfoList)
    //    {
    //        for (int i = 0; i < info.Times; i++)
    //        {
    //            if (GD.Randf() > info.Chance)
    //                continue;
    //            DropItem dropItem = GameManager.Instance.EntityPsMap[EntityType.DropItem].Instantiate<DropItem>();
    //            GetTree().CurrentScene.AddChild(dropItem);
    //            //dropItem.Init(EntityType.DropItem, worldPos, null);
    //            _entityMap[GameManager.Instance.ChunkManager.WorldToChunkPos(worldPos)].Add(dropItem);
    //            dropItem.SetItemInstance(new ItemInstance() { Type = info.Type, Count = 1 });
    //            dropItem.ApplyForce();
    //        }
    //    }
    //}
    //public void SpawnDropItem(Vector2 worldPos, ItemInstance itemInstance)
    //{
    //    DropItem dropItem = GameManager.Instance.EntityPsMap[EntityType.DropItem].Instantiate<DropItem>();
    //    GetTree().CurrentScene.AddChild(dropItem);
    //    //dropItem.Init(EntityType.DropItem, worldPos, null);
    //    _entityMap[GameManager.Instance.ChunkManager.WorldToChunkPos(worldPos)].Add(dropItem);
    //    dropItem.SetItemInstance(itemInstance);
    //    dropItem.ApplyForce();
    //}

    //public void SpawnExpBall(Vector2 worldPos, ExpBallDropInfo info)
    //{
    //    for (int i = 0; i < info.Times; i++)
    //    {
    //        if (GD.Randf() > info.Chance) continue;
    //        float exp = (float)GD.RandRange(info.MinExp, info.MaxExp);
    //        ExpBall expBall = GameManager.Instance.EntityPsMap[EntityType.ExpBall].Instantiate<ExpBall>();
    //        GetTree().CurrentScene.AddChild(expBall);
    //        //expBall.Init(EntityType.ExpBall, worldPos, null);
    //        _entityMap[GameManager.Instance.ChunkManager.WorldToChunkPos(worldPos)].Add(expBall);
    //        expBall.SetExp(exp);
    //    }
    //}
}