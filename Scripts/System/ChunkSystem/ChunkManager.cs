using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Entities.Units;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
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
        [Export] private TileMapLayer _waterTileMapLayer;
        [Export] private TileMapLayer _grassTileMapLayer;
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
            { TileType.Grass, new List<Vector2I>(){ new Vector2I(1, 7)}},
            { TileType.Water, new List<Vector2I>(){ new Vector2I(1, 8)}},

            { TileType.GrassWaterLeft, new List<Vector2I>(){new Vector2I(3, 5)}},
            { TileType.GrassWaterRight, new List<Vector2I>(){new Vector2I(1, 5)}},
            { TileType.GrassWaterUp, new List<Vector2I>(){new Vector2I(2, 6)}},
            { TileType.GrassWaterDown, new List<Vector2I>(){new Vector2I(2, 4)}},

            { TileType.GrassWaterLeftUp, new List<Vector2I>(){new Vector2I(6, 4)}},
            { TileType.GrassWaterLeftRight, new List<Vector2I>(){new Vector2I(6, 8)}},
            { TileType.GrassWaterLeftDown, new List<Vector2I>(){new Vector2I(6, 5)}},
            { TileType.GrassWaterRightUp, new List<Vector2I>(){new Vector2I(7, 4)}},
            { TileType.GrassWaterUpDown, new List<Vector2I>(){new Vector2I(5, 7)}},
            { TileType.GrassWaterRightDown, new List<Vector2I>(){new Vector2I(7, 5)}},

            { TileType.GrassWaterNoLeft, new List<Vector2I>(){new Vector2I(13, 5)}},
            { TileType.GrassWaterNoRight, new List<Vector2I>(){new Vector2I(11, 5)}},
            { TileType.GrassWaterNoUp, new List<Vector2I>(){new Vector2I(12, 6)}},
            { TileType.GrassWaterNoDown, new List<Vector2I>(){new Vector2I(12, 4)}},

            { TileType.GrassWaterAll, new List<Vector2I>(){new Vector2I(17, 4)}},
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

        private void PlaceChunkTiles(Vector2I chunkPos, out Dictionary<Vector2I, TileType> localLogicMap, out List<Vector2I> pureGrassPositions)
        {
            localLogicMap = new Dictionary<Vector2I, TileType>();
            pureGrassPositions = new List<Vector2I>();
            for (int x = -1; x <= _chunkSize; x++)
            {
                for (int y = -1; y <= _chunkSize; y++)
                {
                    int globalX = chunkPos.X * _chunkSize + x;
                    int globalY = chunkPos.Y * _chunkSize + y;
                    Vector2I globalPos = new Vector2I(globalX, globalY);
                    float noiseVal = _noise.GetNoise2D(globalX, globalY);
                    localLogicMap[globalPos] = noiseVal < 0 ? TileType.Water : TileType.Grass;
                }
            }
            for (int x = 0; x < _chunkSize; x++)
            {
                for (int y = 0; y < _chunkSize; y++)
                {
                    int globalX = chunkPos.X * _chunkSize + x;
                    int globalY = chunkPos.Y * _chunkSize + y;
                    Vector2I globalPos = new Vector2I(globalX, globalY);
                    TileType myType = localLogicMap[globalPos];
                    if (myType == TileType.Water)
                    {
                        Vector2I rdTileCoords = TileCoordsListMap[TileType.Water][Random.Shared.Next(TileCoordsListMap[TileType.Water].Count)];
                        _waterTileMapLayer.SetCell(new Vector2I(globalX, globalY), 0, rdTileCoords);
                        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingType.Water, new Vector2I(globalX * _tileSize, globalY * _tileSize));
                        GameManager.Instance.BuildingManager.Place(BuildingType.Water, snapPos);
                    }
                    else if (myType == TileType.Grass)
                    {
                        bool leftIsWater = localLogicMap[globalPos + Vector2I.Left] == TileType.Water;
                        bool rightIsWater = localLogicMap[globalPos + Vector2I.Right] == TileType.Water;
                        bool upIsWater = localLogicMap[globalPos + Vector2I.Up] == TileType.Water;
                        bool downIsWater = localLogicMap[globalPos + Vector2I.Down] == TileType.Water;
                        TileType grassTileType = CheckGrassWater(leftIsWater, rightIsWater, upIsWater, downIsWater);
                        Vector2I rdTileCoords = TileCoordsListMap[grassTileType][0];
                        _grassTileMapLayer.SetCell(globalPos, 0, rdTileCoords);
                        if (grassTileType == TileType.Grass)
                            pureGrassPositions.Add(globalPos);
                    }
                }
            }
        }

        private void LoadChunk(Vector2I chunkPos)
        {
            Chunk curChunk = new Chunk();
            CurActiveChunkMap[chunkPos] = curChunk;
            PlaceChunkTiles(chunkPos, out _, out List<Vector2I> pureGrassPositions);

            if (ChunkSaveDataMap.TryGetValue(chunkPos, out ChunkSaveData existChunkData) == false)
            {
                foreach (Vector2I pos in pureGrassPositions)
                {
                    float rd = GD.Randf();
                    Vector2 tileWorldPos = new Vector2(pos.X * _tileSize, pos.Y * _tileSize);
                    Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);
                    if (rd < 0.1)
                    {
                        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingType.Tree, tileWorldPos + offset);
                        if (GameManager.Instance.BuildingManager.CanPlaced(BuildingType.Tree, snapPos) && WorldToChunkPos(snapPos) == chunkPos)
                        {
                            Tree tree = GD.Load<PackedScene>(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree).TscnPath).Instantiate<Tree>();
                            GetTree().CurrentScene.AddChild(tree);
                            tree.Init(BuildingType.Tree, snapPos);
                        }
                    }
                    else if (rd < 0.15)
                    {
                        DropItem goldDropItem = DropItemPs.Instantiate<DropItem>();
                        GetTree().CurrentScene.AddChild(goldDropItem);
                        goldDropItem.Init(new ItemInstance() { Type = ItemType.Grass, Count = 3 }, tileWorldPos + offset);
                    }
                    else if (rd < 0.2)
                    {
                        DropItem bananaDropItem = DropItemPs.Instantiate<DropItem>();
                        GetTree().CurrentScene.AddChild(bananaDropItem);
                        bananaDropItem.Init(new ItemInstance() { Type = ItemType.Banana, Count = 3 }, tileWorldPos + offset);
                    }
                    else if (rd < 0.25)
                    {
                        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(BuildingType.Stone, tileWorldPos + offset);
                        if (GameManager.Instance.BuildingManager.CanPlaced(BuildingType.Stone, snapPos) && WorldToChunkPos(snapPos) == chunkPos)
                        {
                            Stone stone = GD.Load<PackedScene>(BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone).TscnPath).Instantiate<Stone>();
                            GetTree().CurrentScene.AddChild(stone);
                            stone.Init(BuildingType.Stone, snapPos);
                        }
                    }
                }

                if (chunkPos == Vector2I.Zero)
                {
                    float randomX = (float)GD.RandRange(0, _chunkSize * _tileSize);
                    float randomY = (float)GD.RandRange(0, _chunkSize * _tileSize);
                    Vector2 randomPos = new Vector2(randomX, randomY);
                    DropItem stoneDrop = DropItemPs.Instantiate<DropItem>();
                    GetTree().CurrentScene.AddChild(stoneDrop);
                    stoneDrop.Init(new ItemInstance() { Type = ItemType.MainBaseStone, Count = 1 }, randomPos);
                }
            }
            else
            {
                foreach (BuildingSaveData buildingSaveData in existChunkData.BuildingSaveDataList)
                {
                    Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(buildingSaveData.Type, new Vector2(buildingSaveData.X, buildingSaveData.Y));
                    if (GameManager.Instance.BuildingManager.CanPlaced(buildingSaveData.Type, snapPos) && WorldToChunkPos(snapPos) == chunkPos)
                    {
                        Building building = GD.Load<PackedScene>(BuildingDataManager.Instance.GetBuildingData(buildingSaveData.Type).TscnPath).Instantiate<Building>();
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
        }

        private ChunkSaveData CreateChunkSaveData(Vector2I chunkPos, Chunk chunk)
        {
            ChunkSaveData data = new ChunkSaveData(chunkPos.X, chunkPos.Y);
            foreach (Building building in chunk.BuildingList)
            {
                data.BuildingSaveDataList.Add(new BuildingSaveData() { Type = building.Type, X = building.GlobalPosition.X, Y = building.GlobalPosition.Y });
            }
            foreach (DropItem dropItem in chunk.DropItemList)
            {
                data.DropItemSaveDataList.Add(new DropItemSaveData() { Instance = dropItem.ItemInstance, Count = dropItem.Count, X = dropItem.GlobalPosition.X, Y = dropItem.GlobalPosition.Y });
            }
            return data;
        }

        private void UnloadChunk(Vector2I chunkPos)
        {
            Chunk curChunk = CurActiveChunkMap[chunkPos];
            for (int x = 0; x < _chunkSize; x++)
            {
                for (int y = 0; y < _chunkSize; y++)
                {
                    int globalX = chunkPos.X * _chunkSize + x;
                    int globalY = chunkPos.Y * _chunkSize + y;
                    _waterTileMapLayer.SetCell(new Vector2I(globalX, globalY), -1);
                    _grassTileMapLayer.SetCell(new Vector2I(globalX, globalY), -1);
                }
            }
            foreach (Building building in curChunk.BuildingList)
            {
                GameManager.Instance.BuildingManager.Remove(building.Type, building.GlobalPosition);
                if (building is IQiRangeable qiRangeable)
                    GameManager.Instance.IQiRangeableList.Remove(qiRangeable);
                if (IsInstanceValid(building))
                    building.QueueFree();
            }
            foreach (DropItem dropItem in curChunk.DropItemList)
            {
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
            ChunkSaveDataMap[chunkPos] = CreateChunkSaveData(chunkPos, curChunk);
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
            Vector2I atlasCoords = _waterTileMapLayer.GetCellAtlasCoords(_waterTileMapLayer.LocalToMap(_waterTileMapLayer.ToLocal(worldPos)));
            if (atlasCoords != new Vector2I(-1, -1) && TileTypeMap.TryGetValue(atlasCoords, out TileType tileType))
                return tileType;
            atlasCoords = _grassTileMapLayer.GetCellAtlasCoords(_grassTileMapLayer.LocalToMap(_grassTileMapLayer.ToLocal(worldPos)));
            if (TileTypeMap.TryGetValue(atlasCoords, out tileType))
                return tileType;
            return TileType.Grass;
        }

        public void AddItem(Node2D node, Vector2 worldPos)
        {
            Vector2I chunkPos = WorldToChunkPos(worldPos);
            if (!CurActiveChunkMap.TryGetValue(chunkPos, out Chunk chunk))
                return;
            if (node is Building building)
                chunk.BuildingList.Add(building);
            else if (node is DropItem dropItem)
                chunk.DropItemList.Add(dropItem);
        }
        public void RemoveItem(Node2D node, Vector2 worldPos)
        {
            Vector2I chunkPos = WorldToChunkPos(worldPos);
            if (!CurActiveChunkMap.TryGetValue(chunkPos, out Chunk chunk))
                return;
            if (node is Building building)
                chunk.BuildingList.Remove(building);
            else if (node is DropItem dropItem)
                chunk.DropItemList.Remove(dropItem);
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

            float randomX = 0;
            if (GD.Randf() < 0.5f)
                randomX = (float)GD.RandRange(GameManager.Instance.Player.GlobalPosition.X - 300, GameManager.Instance.Player.GlobalPosition.X - 200);
            else
                randomX = (float)GD.RandRange(GameManager.Instance.Player.GlobalPosition.X + 200, GameManager.Instance.Player.GlobalPosition.X + 300);

            float randomY = 0;
            if (GD.Randf() < 0.5f)
                randomY = (float)GD.RandRange(GameManager.Instance.Player.GlobalPosition.Y - 200, GameManager.Instance.Player.GlobalPosition.Y - 300);
            else
                randomY = (float)GD.RandRange(GameManager.Instance.Player.GlobalPosition.Y + 200, GameManager.Instance.Player.GlobalPosition.Y + 300);

            Vector2 generatePosition = new Vector2(randomX, randomY);

            if (GetTileType(generatePosition) == TileType.Water)
                return;

            Vector2I chunkPos = WorldToChunkPos(generatePosition);
            if (!CurActiveChunkMap.TryGetValue(chunkPos, out Chunk curChunk))
                return;

            if (GameManager.Instance.TimeRatio >= 0.25 && GameManager.Instance.TimeRatio <= 0.75)
            {
                if (curChunk.DayUnitList.Count >= _dayUnitChunkCapacity)
                    return;
                UnitType unitType = _dayUnitTypeList[GD.RandRange(0, _dayUnitTypeList.Count - 1)];
                Unit unit = _unitPs.Instantiate<Unit>();
                GetTree().CurrentScene.AddChild(unit);
                unit.Init(unitType, generatePosition, true);
                curChunk.DayUnitList.Add(unit);
            }
            else
            {
                if (curChunk.NightUnitList.Count >= _nightUnitChunkCapacity)
                    return;
                UnitType unitType = _nightUnitTypeList[GD.RandRange(0, _nightUnitTypeList.Count - 1)];
                Unit unit = _unitPs.Instantiate<Unit>();
                GetTree().CurrentScene.AddChild(unit);
                unit.Init(unitType, generatePosition, false);
                curChunk.NightUnitList.Add(unit);
            }

            _generateUnitTimer = 0;
        }

        private TileType CheckGrassWater(bool leftIsWater, bool rightIsWater, bool upIsWater, bool downIsWater)
        {
            TileType grassTileType = TileType.Grass;
            if (leftIsWater && !rightIsWater && !upIsWater && !downIsWater)//1
                grassTileType = TileType.GrassWaterLeft;
            else if (!leftIsWater && rightIsWater && !upIsWater && !downIsWater)
                grassTileType = TileType.GrassWaterRight;
            else if (!leftIsWater && !rightIsWater && upIsWater && !downIsWater)
                grassTileType = TileType.GrassWaterUp;
            else if (!leftIsWater && !rightIsWater && !upIsWater && downIsWater)
                grassTileType = TileType.GrassWaterDown;

            else if (leftIsWater && rightIsWater && !upIsWater && !downIsWater)//2
                grassTileType = TileType.GrassWaterLeftRight;
            else if (leftIsWater && !rightIsWater && upIsWater && !downIsWater)
                grassTileType = TileType.GrassWaterLeftUp;
            else if (leftIsWater && !rightIsWater && !upIsWater && downIsWater)
                grassTileType = TileType.GrassWaterLeftDown;
            else if (!leftIsWater && rightIsWater && upIsWater && !downIsWater)
                grassTileType = TileType.GrassWaterRightUp;
            else if (!leftIsWater && rightIsWater && !upIsWater && downIsWater)
                grassTileType = TileType.GrassWaterRightDown;
            else if (!leftIsWater && !rightIsWater && upIsWater && downIsWater)
                grassTileType = TileType.GrassWaterUpDown;

            else if (!leftIsWater && rightIsWater && upIsWater && downIsWater)//3
                grassTileType = TileType.GrassWaterNoLeft;
            else if (leftIsWater && !rightIsWater && upIsWater && downIsWater)
                grassTileType = TileType.GrassWaterNoRight;
            else if (leftIsWater && rightIsWater && !upIsWater && downIsWater)
                grassTileType = TileType.GrassWaterNoUp;
            else if (leftIsWater && rightIsWater && upIsWater && !downIsWater)
                grassTileType = TileType.GrassWaterNoDown;

            else if (leftIsWater && rightIsWater && upIsWater && downIsWater)//4
                grassTileType = TileType.GrassWaterAll;

            return grassTileType;
        }

        public override void _ExitTree()
        {
            foreach (Vector2I chunkPos in new List<Vector2I>(CurActiveChunkMap.Keys))
            {
                UnloadChunk(chunkPos);
            }
        }

        public void SaveActiveChunk()
        {
            foreach (Vector2I chunkPos in CurActiveChunkMap.Keys)
            {
                ChunkSaveDataMap[chunkPos] = CreateChunkSaveData(chunkPos, CurActiveChunkMap[chunkPos]);
            }
        }

    }
}