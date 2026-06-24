using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Units;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.BuildingSystem.Buildings;
using Solo.Scripts.System.ItemSystem;
using Solo.Scripts.System.SaveSystem;
using System;
using System.Collections.Generic;
using Tree = Solo.Scripts.System.BuildingSystem.Buildings.Tree;

namespace Solo.Scripts.System.ChunkSystem
{
    //public struct TileInfo
    //{
    //    //public TileType Type;
    //    public int SourceId;
    //    public Vector2I AtlasCoords;
    //}

    public partial class ChunkManager : Node2D
    {
        [Export] private TileMapLayer _tileMapLayer;
        //[Export] private PackedScene _buildingPs;
        [Export] public PackedScene DropItemPs;
        [Export] public PackedScene _unitPs;

        //public BuildingManager BuildingManager;
        private FastNoiseLite _noise = new FastNoiseLite();
        private int _chunkSize = 8;       // 每个区块的瓦片数量
        private int _tileSize = 16;        // 每个瓦片的像素大小
        private int _renderDistance = 3;  // 区块渲染距离
        private string _seedString;
        public Dictionary<Vector2I, Chunk> CurActiveChunkMap = new Dictionary<Vector2I, Chunk>();
        public Dictionary<Vector2I, ChunkSaveData> ChunkSaveDataMap = new Dictionary<Vector2I, ChunkSaveData>();


        private double _unLoadChunkTimer = 0;// 卸载计时器，没必要每帧都检测卸载
        private double _unloadChunkCd = 1.0; // 每秒检查一次卸载


        private Dictionary<TileType, List<Vector2I>> TileCoordsListMap = new Dictionary<TileType, List<Vector2I>>()
        {
            { TileType.Grass, new List<Vector2I>(){ new Vector2I(7, 6)}},
            { TileType.Stone, new List<Vector2I>(){new Vector2I(7, 7)}},
            { TileType.Water, new List<Vector2I>(){ new Vector2I(7, 8)}},
        };
        private Dictionary<Vector2I, TileType> TileTypeMap = new Dictionary<Vector2I, TileType>();//用于反查

        public override void _Ready()
        {
            GameManager.Instance.ChunkManager = this;
            GameManager.Instance.BuildingManager = new BuildingManager(_tileSize);
            foreach (ChunkSaveData chunkSaveData in SaveManager.Instance.CurSaveData.ChunkSaveDataList)
            {
                ChunkSaveDataMap[new Vector2I(chunkSaveData.X, chunkSaveData.Y)] = chunkSaveData;
            }
            _seedString = SaveManager.Instance.CurSaveData.WorldSeedStr;// 初始化噪声
            _noise.Seed = GD.Hash(_seedString);
            _noise.Frequency = 0.05f;
            _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
            foreach (var kvp in TileCoordsListMap)
            {
                foreach (Vector2I tileCoords in kvp.Value)
                    TileTypeMap[tileCoords] = kvp.Key;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            Player player = GameManager.Instance.Player;
            if (player == null)
                return;
            Vector2I playerChunkPos = WorldToChunkPos(player.GlobalPosition);// 获取玩家所在的区块坐标

            GenerateUnit((float)delta);

            for (int x = -_renderDistance; x <= _renderDistance; x++)//生存区块
            {
                for (int y = -_renderDistance; y <= _renderDistance; y++)
                {
                    Vector2I chunkPos = playerChunkPos + new Vector2I(x, y);
                    if (CurActiveChunkMap.ContainsKey(chunkPos) == false)//不存在的区块要加载/生成
                    {
                        LoadChunk(chunkPos);
                    }
                }
            }

            _unLoadChunkTimer += delta;
            if (_unLoadChunkTimer >= _unloadChunkCd)//定时清理远处的区块
            {
                int unloadDistance = _renderDistance + 1;// 卸载距离比加载距离大 1-2，防止在边缘反复生成/卸载
                List<Vector2I> removeChunkPosList = new List<Vector2I>();
                foreach (Vector2I chunkPos in CurActiveChunkMap.Keys)
                {
                    if (Mathf.Abs(chunkPos.X - playerChunkPos.X) > unloadDistance || Mathf.Abs(chunkPos.Y - playerChunkPos.Y) > unloadDistance)
                    {
                        removeChunkPosList.Add(chunkPos);
                    }
                }
                foreach (Vector2I chunkPos in removeChunkPosList)
                {
                    UnloadChunk(chunkPos);
                }
                _unLoadChunkTimer = 0;
            }
        }

        private void LoadChunk(Vector2I chunkPos)
        {
            Chunk curChunk = new Chunk();
            CurActiveChunkMap[chunkPos] = curChunk;
            if (ChunkSaveDataMap.TryGetValue(chunkPos, out ChunkSaveData existChunkData))
            {
                //恢复区块
                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int y = 0; y < _chunkSize; y++)
                    {
                        int globalX = chunkPos.X * _chunkSize + x;
                        int globalY = chunkPos.Y * _chunkSize + y;

                        float noiseVal = _noise.GetNoise2D(globalX, globalY);
                        if (noiseVal < 0)//水
                        {
                            Vector2I rdTileCoords = TileCoordsListMap[TileType.Water][Random.Shared.Next(TileCoordsListMap[TileType.Water].Count)];
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), 0, rdTileCoords);
                            Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingType.Water, new Vector2I(globalX * _tileSize, globalY * _tileSize));
                            GameManager.Instance.BuildingManager.Place(BuildingType.Water, snapPos);
                            //_tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Water].SourceId, _tileTypeInfoMap[TileType.Water].AtlasCoords);
                        }
                        else if (noiseVal < 0.35)//草
                        {
                            Vector2I rdTileCoords = TileCoordsListMap[TileType.Grass][Random.Shared.Next(TileCoordsListMap[TileType.Grass].Count)];
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), 0, rdTileCoords);
                            //_tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Grass].SourceId, _tileTypeInfoMap[TileType.Grass].AtlasCoords);
                        }
                        else//石
                        {
                            Vector2I rdTileCoords = TileCoordsListMap[TileType.Stone][Random.Shared.Next(TileCoordsListMap[TileType.Stone].Count)];
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), 0, rdTileCoords);
                            //_tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Stone].SourceId, _tileTypeInfoMap[TileType.Stone].AtlasCoords);
                        }
                    }
                }
                foreach (BuildingSaveData buildingSaveData in existChunkData.BuildingSaveDataList)
                {
                    Vector2 snapPos;
                    BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(buildingSaveData.Type);
                    snapPos = GameManager.Instance.BuildingManager.SnapToCell(buildingSaveData.Type, new Vector2(buildingSaveData.X, buildingSaveData.Y));
                    if (GameManager.Instance.BuildingManager.CanPlaced(buildingSaveData.Type, snapPos) && WorldToChunkPos(snapPos) == chunkPos)//可放置且不越界
                    {
                        PackedScene bdPs = GD.Load<PackedScene>(BuildingDataManager.Instance.GetBuildingData(buildingSaveData.Type).TscnPath);
                        Building building = bdPs.Instantiate<Building>();
                        GetTree().CurrentScene.AddChild(building);
                        building.Init(buildingSaveData.Type, snapPos);
                    }
                }
                foreach (DropItemSaveData dropItemData in existChunkData.DropItemSaveDataList)
                {
                    DropItem DropItem = DropItemPs.Instantiate<DropItem>();
                    GetTree().CurrentScene.AddChild(DropItem);
                    DropItem.Init(dropItemData.Instance, new Vector2(dropItemData.X, dropItemData.Y));
                }
            }
            else
            {
                //新生成区块
                for (int x = 0; x < _chunkSize; x++)
                {
                    for (int y = 0; y < _chunkSize; y++)
                    {
                        int globalX = chunkPos.X * _chunkSize + x;
                        int globalY = chunkPos.Y * _chunkSize + y;
                        float noiseVal = _noise.GetNoise2D(globalX, globalY);
                        if (noiseVal < 0)//水
                        {

                            Vector2I rdTileCoords = TileCoordsListMap[TileType.Water][Random.Shared.Next(TileCoordsListMap[TileType.Water].Count)];
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), 0, rdTileCoords);
                            Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingType.Water, new Vector2I(globalX * _tileSize, globalY * _tileSize));
                            GameManager.Instance.BuildingManager.Place(BuildingType.Water, snapPos);
                        }
                        else if (noiseVal < 0.35)//草
                        {
                            Vector2I rdTileCoords = TileCoordsListMap[TileType.Grass][Random.Shared.Next(TileCoordsListMap[TileType.Grass].Count)];
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), 0, rdTileCoords);
                            //随机生成
                            float rd = GD.Randf();
                            if (rd < 0.1)//树
                            {
                                Vector2 curTilePos = new Vector2(globalX * _tileSize, globalY * _tileSize) + new Vector2(_tileSize / 2f, _tileSize / 2f);
                                Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingType.Tree, curTilePos);
                                if (GameManager.Instance.BuildingManager.CanPlaced(BuildingType.Tree, snapPos) && WorldToChunkPos(snapPos) == chunkPos)
                                {
                                    PackedScene treePs = GD.Load<PackedScene>(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree).TscnPath);
                                    Tree tree = treePs.Instantiate<Tree>();
                                    GetTree().CurrentScene.AddChild(tree);
                                    tree.Init(BuildingType.Tree, snapPos);
                                }
                            }
                            else if (rd < 0.15)
                            {
                                Vector2 tileWorldPos = new Vector2(globalX * _tileSize, globalY * _tileSize);
                                Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);// 计算偏移量，使物体位于瓦片中心 (假设 _tileSize 是 16，偏移就是 8)
                                DropItem goldDropItem = DropItemPs.Instantiate<DropItem>();
                                GetTree().CurrentScene.AddChild(goldDropItem);
                                goldDropItem.Init(new ItemInstance() { Type = ItemType.Grass, Count = 3 }, tileWorldPos + offset);
                            }
                            else if (rd < 0.2)
                            {
                                Vector2 tileWorldPos = new Vector2(globalX * _tileSize, globalY * _tileSize);
                                Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);// 计算偏移量，使物体位于瓦片中心 (假设 _tileSize 是 16，偏移就是 8)
                                DropItem bananaDropItem = DropItemPs.Instantiate<DropItem>();
                                GetTree().CurrentScene.AddChild(bananaDropItem);
                                bananaDropItem.Init(new ItemInstance() { Type = ItemType.Banana, Count = 3 }, tileWorldPos + offset);
                            }
                        }
                        else//石
                        {
                            Vector2I rdTileCoords = TileCoordsListMap[TileType.Stone][Random.Shared.Next(TileCoordsListMap[TileType.Stone].Count)];
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), 0, rdTileCoords);
                            //_tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Stone].SourceId, _tileTypeInfoMap[TileType.Stone].AtlasCoords);
                            if (GD.Randf() < 0.2)
                            {
                                Vector2 curTilePos = new Vector2(globalX * _tileSize, globalY * _tileSize) + new Vector2(_tileSize / 2f, _tileSize / 2f);
                                Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingType.Stone, curTilePos);
                                if (GameManager.Instance.BuildingManager.CanPlaced(BuildingType.Stone, snapPos) && WorldToChunkPos(snapPos) == chunkPos)
                                {
                                    PackedScene stonePs = GD.Load<PackedScene>(BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone).TscnPath);
                                    Stone stone = stonePs.Instantiate<Stone>();
                                    GetTree().CurrentScene.AddChild(stone);
                                    stone.Init(BuildingType.Stone, snapPos);
                                }
                            }
                        }
                    }
                }

                //if (GD.Randf() < 0.2)
                //{
                //    Unit unit = _unitPs.Instantiate<Unit>();
                //    GetTree().CurrentScene.AddChild(unit);
                //    unit.Init(UnitType.Wolf, new Vector2(chunkPos.X * _chunkSize * _tileSize + GD.RandRange(0, _chunkSize * _tileSize), chunkPos.Y * _chunkSize * _tileSize + GD.RandRange(0, _chunkSize * _tileSize)));
                //}
                //if (GD.Randf() < 0.2)
                //{
                //    Unit unit = _unitPs.Instantiate<Unit>();
                //    GetTree().CurrentScene.AddChild(unit);
                //    unit.Init(UnitType.Fox, new Vector2(chunkPos.X * _chunkSize * _tileSize + GD.RandRange(0, _chunkSize * _tileSize), chunkPos.Y * _chunkSize * _tileSize + GD.RandRange(0, _chunkSize * _tileSize)));
                //}

                // 第一个区块(0,0)首次生成时，必定掉落1个太古源石，位置随机
                if (chunkPos == Vector2I.Zero)
                {
                    float randomX = (float)GD.RandRange(0, _chunkSize * _tileSize);
                    float randomY = (float)GD.RandRange(0, _chunkSize * _tileSize);
                    Vector2 randomPos = new Vector2(
                        chunkPos.X * _chunkSize * _tileSize + randomX,
                        chunkPos.Y * _chunkSize * _tileSize + randomY
                    );
                    DropItem stoneDrop = DropItemPs.Instantiate<DropItem>();
                    GetTree().CurrentScene.AddChild(stoneDrop);
                    stoneDrop.Init(new ItemInstance() { Type = ItemType.MainBaseStone, Count = 1 }, randomPos);
                }

            }
        }

        private void UnloadChunk(Vector2I chunkPos)
        {
            Chunk curChunk = CurActiveChunkMap[chunkPos];
            for (int x = 0; x < _chunkSize; x++)//移除瓦片
            {
                for (int y = 0; y < _chunkSize; y++)
                {
                    int globalX = chunkPos.X * _chunkSize + x;
                    int globalY = chunkPos.Y * _chunkSize + y;
                    _tileMapLayer.SetCell(new Vector2I(globalX, globalY), -1);// 设置为 -1 表示移除瓦片
                }
            }
            //写入savedata
            ChunkSaveData curChunkData = new ChunkSaveData(chunkPos.X, chunkPos.Y);
            foreach (Building building in curChunk.BuildingList)//移除node
            {
                curChunkData.BuildingSaveDataList.Add(new BuildingSaveData() { Type = building.Type, X = building.GlobalPosition.X, Y = building.GlobalPosition.Y });
                GameManager.Instance.BuildingManager.Remove(building.Type, building.GlobalPosition);
                if (IsInstanceValid(building))
                    building.QueueFree();
            }
            foreach (DropItem dropItem in curChunk.DropItemList)//移除node
            {
                curChunkData.DropItemSaveDataList.Add(new DropItemSaveData() { Instance = dropItem.ItemInstance, Count = dropItem.Count, X = dropItem.GlobalPosition.X, Y = dropItem.GlobalPosition.Y });
                if (IsInstanceValid(dropItem))
                    dropItem.QueueFree();
            }
            foreach (Unit dayUnit in curChunk.DayUnitList)
            {
                if (IsInstanceValid(dayUnit))
                    dayUnit.QueueFree();
            }
            foreach (Unit night in curChunk.NightUnitList)
            {
                if (IsInstanceValid(night))
                    night.QueueFree();
            }
            ChunkSaveDataMap[chunkPos] = curChunkData;
            CurActiveChunkMap.Remove(chunkPos);
        }

        public Vector2I WorldToChunkPos(Vector2 worldPos)
        {
            float px = Mathf.Floor(worldPos.X / (_chunkSize * _tileSize));
            float py = Mathf.Floor(worldPos.Y / (_chunkSize * _tileSize));
            return new Vector2I((int)px, (int)py);
        }

        public TileType GetTileType(Vector2 worldPos)
        {
            Vector2I mapCoords = _tileMapLayer.LocalToMap(_tileMapLayer.ToLocal(worldPos));
            //int sourceId = _tileMapLayer.GetCellSourceId(mapCoords);// 从 TileMapLayer 获取该坐标下的 SourceID 和 AtlasCoords
            Vector2I atlasCoords = _tileMapLayer.GetCellAtlasCoords(mapCoords);
            return TileTypeMap[atlasCoords];
        }

        public void AddItem(Node2D node, Vector2 worldPos)
        {
            Vector2I chunkPos = WorldToChunkPos(worldPos);
            if (node is Building building)
            {
                CurActiveChunkMap[chunkPos].BuildingList.Add(building);
            }
            else if (node is DropItem dropItem)
            {
                CurActiveChunkMap[chunkPos].DropItemList.Add(dropItem);
            }
        }
        public void RemoveItem(Node2D node, Vector2 worldPos)
        {
            Vector2I chunkPos = WorldToChunkPos(worldPos);
            if (node is Building building)
            {
                CurActiveChunkMap[chunkPos].BuildingList.Remove(building);
            }
            else if (node is DropItem dropItem)
            {
                CurActiveChunkMap[chunkPos].DropItemList.Remove(dropItem);
            }
        }

        //单位生成 : 在玩家圆环内随机取点, 判断该点所属区块的单位数量, 若符合生成条件(中立单位容量和敌对单位容量, 非水), 生成
        //
        private int _dayUnitChunkCapacity = 1;
        private int _nightUnitChunkCapacity = 1;
        private List<UnitType> _dayUnitTypeList = new List<UnitType>() { UnitType.Fox };
        private List<UnitType> _nightUnitTypeList = new List<UnitType>() { UnitType.Wolf };

        private float _generateUnitDuration = 1;
        private float _generateUnitTimer = 0;
        private void GenerateUnit(float delta)
        {
            _generateUnitTimer += delta;
            if (_generateUnitTimer < _generateUnitDuration)
                return;
            _generateUnitTimer = 0;//最后成功才重置计时器, 若中途有失败退出, 下次直接重试

            float randomX = 0;//随机x
            if (GD.Randf() < 0.5f)
                randomX = (float)GD.RandRange(GameManager.Instance.Player.GlobalPosition.X - 300, GameManager.Instance.Player.GlobalPosition.X - 200);
            else
                randomX = (float)GD.RandRange(GameManager.Instance.Player.GlobalPosition.X + 200, GameManager.Instance.Player.GlobalPosition.X + 300);

            float randomY = 0;//随机y
            if (GD.Randf() < 0.5f)
                randomY = (float)GD.RandRange(GameManager.Instance.Player.GlobalPosition.Y - 200, GameManager.Instance.Player.GlobalPosition.Y - 300);
            else
                randomY = (float)GD.RandRange(GameManager.Instance.Player.GlobalPosition.Y + 200, GameManager.Instance.Player.GlobalPosition.Y + 300);

            Vector2 generatePosition = new Vector2(randomX, randomY);

            // 打印测试结果
            //GD.Print($"generatePosition : {generatePosition}");

            if (GetTileType(generatePosition) == TileType.Water)//非可活动区域
                return;

            //生成 : 
            //天亮, 在天亮单位列表里随机, 占用天亮单位容量
            Vector2I chunkPos = WorldToChunkPos(generatePosition);
            Chunk curChunk = CurActiveChunkMap[chunkPos];
            if (GameManager.Instance.TimeRatio >= 0.25 && GameManager.Instance.TimeRatio <= 0.75)//白天
            {
                if (curChunk.DayUnitList.Count >= _dayUnitChunkCapacity)//超过了该区块白天单位上限
                    return;
                UnitType unitType = _dayUnitTypeList[GD.RandRange(0, _dayUnitTypeList.Count - 1)];
                Unit unit = _unitPs.Instantiate<Unit>();
                GetTree().CurrentScene.AddChild(unit);
                unit.Init(unitType, generatePosition, true);
            }
            else//夜晚
            {
                if (curChunk.NightUnitList.Count >= _nightUnitChunkCapacity)
                    return;
                UnitType unitType = _nightUnitTypeList[GD.RandRange(0, _nightUnitTypeList.Count - 1)];
                Unit unit = _unitPs.Instantiate<Unit>();
                GetTree().CurrentScene.AddChild(unit);
                unit.Init(unitType, generatePosition, false);
            }




        }



        public override void _ExitTree()
        {
            foreach (Vector2I chunkPos in CurActiveChunkMap.Keys)
            {
                UnloadChunk(chunkPos);
            }
        }

        //未激活区块在卸载时已经保存过了, 这里提供一个接口让激活区块可以保存
        public void SaveActiveChunk()
        {
            foreach (Vector2I chunkPos in CurActiveChunkMap.Keys)
            {
                ChunkSaveData chunkSaveData = new ChunkSaveData(chunkPos.X, chunkPos.Y);
                Chunk chunk = CurActiveChunkMap[chunkPos];
                foreach (Building building in chunk.BuildingList)
                {
                    chunkSaveData.BuildingSaveDataList.Add(new BuildingSaveData() { Type = building.Type, X = building.GlobalPosition.X, Y = building.GlobalPosition.Y });
                }
                foreach (DropItem dropItem in chunk.DropItemList)//移除node
                {
                    chunkSaveData.DropItemSaveDataList.Add(new DropItemSaveData() { Instance = dropItem.ItemInstance, Count = dropItem.Count, X = dropItem.GlobalPosition.X, Y = dropItem.GlobalPosition.Y });
                }
                ChunkSaveDataMap[chunkPos] = chunkSaveData;
            }
        }
    }
}