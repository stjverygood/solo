using Godot;
using Solo.Scripts.Character.Player;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.ItemSystem;
using Solo.Scripts.System.SaveSystem;
using System.Collections.Generic;

namespace Solo.Scripts.System.ChunkSystem
{

    public struct TileInfo
    {
        public int SourceId;
        public Vector2I AtlasCoords;
    }

    public partial class ChunkManager : Node2D
    {
        [Export] private TileMapLayer _tileMapLayer;
        [Export] public PackedScene TreePs;
        [Export] public PackedScene StonePs;
        [Export] public PackedScene DropItemPs;

        public BuildingManager BuildingManager;
        private FastNoiseLite _noise = new FastNoiseLite();
        private int _chunkSize = 8;       // 每个区块的瓦片数量
        private int _tileSize = 16;        // 每个瓦片的像素大小
        private int _renderDistance = 1;  // 玩家视野内的区块半径
        private string _seedString;
        public Dictionary<Vector2I, Chunk> CurActiveChunkMap = new Dictionary<Vector2I, Chunk>();
        public Dictionary<Vector2I, ChunkSaveData> ChunkSaveDataMap = new Dictionary<Vector2I, ChunkSaveData>();


        private double _unLoadChunkTimer = 0;// 卸载计时器，没必要每帧都检测卸载
        private double _unloadChunkCd = 1.0; // 每秒检查一次卸载

        private Dictionary<TileInfo, TileType> _tileInfoTypeMap = new Dictionary<TileInfo, TileType>()
        {
            { new TileInfo() { SourceId = 0, AtlasCoords = new Vector2I(1, 1) }, TileType.Grass },
            { new TileInfo() { SourceId = 1, AtlasCoords = new Vector2I(0, 0) }, TileType.Water },
            { new TileInfo() { SourceId = 3, AtlasCoords = new Vector2I(9, 19) }, TileType.Stone },
        };
        private Dictionary<TileType, TileInfo> _tileTypeInfoMap = new Dictionary<TileType, TileInfo>()
        {
            { TileType.Grass,new TileInfo() { SourceId = 0, AtlasCoords = new Vector2I(1, 1) }  },
            { TileType.Water,new TileInfo() { SourceId = 1, AtlasCoords = new Vector2I(0, 0) } },
            { TileType.Stone, new TileInfo() { SourceId = 3, AtlasCoords = new Vector2I(9, 19) } },
        };


        public override void _Ready()
        {
            GameManager.Instance.ChunkManager = this;
            BuildingManager = new BuildingManager(_tileSize);
            foreach (ChunkSaveData chunkSaveData in SaveManager.Instance.CurSaveData.ChunkSaveDataList)
            {
                ChunkSaveDataMap[new Vector2I(chunkSaveData.X, chunkSaveData.Y)] = chunkSaveData;
            }
            _seedString = SaveManager.Instance.CurSaveData.WorldSeedStr;// 初始化噪声
            _noise.Seed = GD.Hash(_seedString);
            _noise.Frequency = 0.05f;
            _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        }

        public override void _PhysicsProcess(double delta)
        {
            Player player = GameManager.Instance.Player;
            if (player == null)
                return;
            Vector2I playerChunkPos = WorldToChunkPos(player.GlobalPosition);// 获取玩家所在的区块坐标
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
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Water].SourceId, _tileTypeInfoMap[TileType.Water].AtlasCoords);
                        }
                        else if (noiseVal < 0.35)//草
                        {
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Grass].SourceId, _tileTypeInfoMap[TileType.Grass].AtlasCoords);
                        }
                        else//石
                        {
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Stone].SourceId, _tileTypeInfoMap[TileType.Stone].AtlasCoords);
                        }
                    }
                }
                foreach (BuildingSaveData buildingSaveData in existChunkData.BuildingSaveDataList)
                {
                    Vector2 snapPos;
                    switch (buildingSaveData.Type)
                    {
                        case BuildingType.Tree:
                            snapPos = BuildingManager.SnapToCell(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree), new Vector2(buildingSaveData.X, buildingSaveData.Y));
                            if (BuildingManager.CanPlaced(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree), new Vector2(buildingSaveData.X, buildingSaveData.Y)))
                            {
                                BuildingManager.Place(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree), new Vector2(buildingSaveData.X, buildingSaveData.Y));
                                BuildingBase tree = TreePs.Instantiate<BuildingBase>();
                                tree.Data = BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree);
                                tree.GlobalPosition = snapPos;
                                GetTree().CurrentScene.AddChild(tree);
                                curChunk.BuildingList.Add(tree);
                            }
                            break;
                        case BuildingType.Stone:
                            snapPos = BuildingManager.SnapToCell(BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone), new Vector2(buildingSaveData.X, buildingSaveData.Y));
                            if (BuildingManager.CanPlaced(BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone), new Vector2(buildingSaveData.X, buildingSaveData.Y)))
                            {
                                BuildingManager.Place(BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone), new Vector2(buildingSaveData.X, buildingSaveData.Y));
                                BuildingBase stone = StonePs.Instantiate<BuildingBase>();
                                stone.Data = BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone);
                                stone.GlobalPosition = snapPos;
                                GetTree().CurrentScene.AddChild(stone);
                                curChunk.BuildingList.Add(stone);
                            }
                            break;

                    }
                }
                foreach (DropItemSaveData dropItemData in existChunkData.DropItemSaveDataList)
                {
                    switch (dropItemData.Type)
                    {
                        case ItemType.Gold:
                            DropItem goldDropItem = DropItemPs.Instantiate<DropItem>();
                            goldDropItem.Init(new ItemInstance() { Data = ItemManager.Instance.GetItemData(ItemType.Gold), Count = 3 });
                            GetTree().CurrentScene.AddChild(goldDropItem);
                            goldDropItem.GlobalPosition = new Vector2(dropItemData.X, dropItemData.Y);
                            curChunk.DropItemList.Add(goldDropItem);
                            break;
                    }
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
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Water].SourceId, _tileTypeInfoMap[TileType.Water].AtlasCoords);
                        }
                        else if (noiseVal < 0.35)//草
                        {
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Grass].SourceId, _tileTypeInfoMap[TileType.Grass].AtlasCoords);
                            //随机生成
                            float rd = GD.Randf();
                            if (rd < 0.1)
                            {
                                Vector2 curTilePos = new Vector2(globalX * _tileSize, globalY * _tileSize) + new Vector2(_tileSize / 2f, _tileSize / 2f);
                                Vector2 snapPos = BuildingManager.SnapToCell(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree), curTilePos);
                                if (BuildingManager.CanPlaced(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree), curTilePos))
                                {
                                    BuildingManager.Place(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree), curTilePos);
                                    BuildingBase tree = TreePs.Instantiate<BuildingBase>();
                                    tree.Data = BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree);
                                    tree.GlobalPosition = snapPos;
                                    GetTree().CurrentScene.AddChild(tree);
                                    curChunk.BuildingList.Add(tree);
                                }
                            }
                            else if (rd < 0.15)
                            {
                                DropItem goldDropItem = DropItemPs.Instantiate<DropItem>();
                                Vector2 tileWorldPos = new Vector2(globalX * _tileSize, globalY * _tileSize);
                                Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);// 计算偏移量，使物体位于瓦片中心 (假设 _tileSize 是 16，偏移就是 8)
                                goldDropItem.GlobalPosition = tileWorldPos + offset;
                                GetTree().CurrentScene.AddChild(goldDropItem);
                                goldDropItem.Init(new ItemInstance() { Data = ItemManager.Instance.GetItemData(ItemType.Gold), Count = 3 });
                                curChunk.DropItemList.Add(goldDropItem);
                            }

                        }
                        else//石
                        {
                            _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Stone].SourceId, _tileTypeInfoMap[TileType.Stone].AtlasCoords);
                            Vector2 curTilePos = new Vector2(globalX * _tileSize, globalY * _tileSize) + new Vector2(_tileSize / 2f, _tileSize / 2f);
                            Vector2 snapPos = BuildingManager.SnapToCell(BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone), curTilePos);
                            if (BuildingManager.CanPlaced(BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone), curTilePos))
                            {
                                BuildingManager.Place(BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone), curTilePos);
                                BuildingBase stone = StonePs.Instantiate<BuildingBase>();
                                stone.Data = BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone);
                                stone.GlobalPosition = snapPos;
                                GetTree().CurrentScene.AddChild(stone);
                                curChunk.BuildingList.Add(stone);
                            }
                        }
                    }
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
            foreach (BuildingBase building in curChunk.BuildingList)//移除node
            {
                curChunkData.BuildingSaveDataList.Add(new BuildingSaveData() { Type = building.Data.Type, X = building.GlobalPosition.X, Y = building.GlobalPosition.Y });
                BuildingManager.Remove(BuildingDataManager.Instance.GetBuildingData(building.Data.Type), building.GlobalPosition);
                if (IsInstanceValid(building))
                    building.QueueFree();
            }
            foreach (DropItem dropItem in curChunk.DropItemList)//移除node
            {
                curChunkData.DropItemSaveDataList.Add(new DropItemSaveData() { Type = dropItem.ItemInstance.Data.Type, X = dropItem.GlobalPosition.X, Y = dropItem.GlobalPosition.Y });
                if (IsInstanceValid(dropItem))
                    dropItem.QueueFree();
            }
            ChunkSaveDataMap[chunkPos] = curChunkData;
            CurActiveChunkMap.Remove(chunkPos);
            GD.Print("UnloadChunk");
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
            int sourceId = _tileMapLayer.GetCellSourceId(mapCoords);// 从 TileMapLayer 获取该坐标下的 SourceID 和 AtlasCoords
            Vector2I atlasCoords = _tileMapLayer.GetCellAtlasCoords(mapCoords);
            return _tileInfoTypeMap[new TileInfo() { SourceId = sourceId, AtlasCoords = atlasCoords }];
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
                foreach (BuildingBase building in chunk.BuildingList)
                {
                    chunkSaveData.BuildingSaveDataList.Add(new BuildingSaveData() { Type = building.Data.Type, X = building.GlobalPosition.X, Y = building.GlobalPosition.Y });
                }
                foreach (DropItem dropItem in chunk.DropItemList)//移除node
                {
                    chunkSaveData.DropItemSaveDataList.Add(new DropItemSaveData() { Type = dropItem.ItemInstance.Data.Type, X = dropItem.GlobalPosition.X, Y = dropItem.GlobalPosition.Y });
                }
                ChunkSaveDataMap[chunkPos] = chunkSaveData;
            }
        }
    }
}