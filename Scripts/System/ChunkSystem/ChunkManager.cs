using Godot;
using Solo.Scripts.Character.Player;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.ItemSystem;
using Solo.Scripts.System.ResourceSystem;
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
        //private Dictionary<Vector2I, List<Node2D>> _curActiveChunkNodeListMap = new Dictionary<Vector2I, List<Node2D>>();
        //private Dictionary<Vector2I, List<WorldObjectData>> _curActiveChunkObjListMap = new Dictionary<Vector2I, List<WorldObjectData>>();
        //private HashSet<Vector2I> _curActiveChunkPosSet = new HashSet<Vector2I>();
        //public Dictionary<Vector2I, List<ResObj>> CurActiveChunkResObjListMap = new Dictionary<Vector2I, List<ResObj>>();//活跃区块的资源物
        //public Dictionary<Vector2I, List<DropItem>> CurActiveChunkDropItemListMap = new Dictionary<Vector2I, List<DropItem>>();//活跃区块的掉落物
        public Dictionary<Vector2I, Chunk> CurActiveChunkMap = new Dictionary<Vector2I, Chunk>();


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
            //GD.Print($"playerChunkPos : {playerChunkPos}");
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
            if (SaveManager.Instance.CurSaveData.ChunkDataMap.TryGetValue(chunkPos, out ChunkData existChunkData))
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
                foreach (ResObjData resObjData in existChunkData.ResObjDataList)
                {
                    switch (resObjData.Type)
                    {
                        case ResObjType.Tree:
                            ResObj treeRes = TreePs.Instantiate<ResObj>();
                            GetTree().CurrentScene.AddChild(treeRes);
                            treeRes.GlobalPosition = resObjData.Position;
                            curChunk.ResObjList.Add(treeRes);
                            break;
                        case ResObjType.Stone:
                            ResObj stoneRes = StonePs.Instantiate<ResObj>();
                            GetTree().CurrentScene.AddChild(stoneRes);
                            stoneRes.GlobalPosition = resObjData.Position;
                            curChunk.ResObjList.Add(stoneRes);
                            break;

                    }
                }
                foreach (DropItemData dropItemData in existChunkData.DropItemDataList)
                {
                    switch (dropItemData.Type)
                    {
                        case ItemType.Gold:
                            DropItem goldDropItem = DropItemPs.Instantiate<DropItem>();
                            goldDropItem.Init(new ItemInstance() { Data = ItemManager.Instance.GetItemData(ItemType.Gold), Count = 3 });
                            GetTree().CurrentScene.AddChild(goldDropItem);
                            goldDropItem.GlobalPosition = dropItemData.Position;
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
                                ResObj treeResObj = TreePs.Instantiate<ResObj>();
                                Vector2 tileWorldPos = new Vector2(globalX * _tileSize, globalY * _tileSize);
                                Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);// 计算偏移量，使物体位于瓦片中心 (假设 _tileSize 是 16，偏移就是 8)
                                treeResObj.GlobalPosition = tileWorldPos + offset;
                                GetTree().CurrentScene.AddChild(treeResObj);
                                curChunk.ResObjList.Add(treeResObj);
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
                            if (GD.Randf() < 0.1)
                            {
                                ResObj stoneResObj = StonePs.Instantiate<ResObj>();
                                Vector2 tileWorldPos = new Vector2(globalX * _tileSize, globalY * _tileSize);
                                Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);// 计算偏移量，使物体位于瓦片中心 (假设 _tileSize 是 16，偏移就是 8)
                                stoneResObj.GlobalPosition = tileWorldPos + offset;
                                GetTree().CurrentScene.AddChild(stoneResObj);
                                curChunk.ResObjList.Add(stoneResObj);
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
            ChunkData curChunkData = new ChunkData();
            foreach (ResObj resObj in curChunk.ResObjList)//移除node
            {
                curChunkData.ResObjDataList.Add(new ResObjData() { Type = resObj.Type, Position = resObj.GlobalPosition });
                if (IsInstanceValid(resObj))
                    resObj.QueueFree();
            }
            foreach (DropItem dropItem in curChunk.DropItemList)//移除node
            {
                curChunkData.DropItemDataList.Add(new DropItemData() { Type = dropItem.ItemInstance.Data.Type, Position = dropItem.GlobalPosition });
                if (IsInstanceValid(dropItem))
                    dropItem.QueueFree();
            }
            SaveManager.Instance.CurSaveData.ChunkDataMap[chunkPos] = curChunkData;
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
            int sourceId = _tileMapLayer.GetCellSourceId(mapCoords);// 从 TileMapLayer 获取该坐标下的 SourceID 和 AtlasCoords
            Vector2I atlasCoords = _tileMapLayer.GetCellAtlasCoords(mapCoords);
            return _tileInfoTypeMap[new TileInfo() { SourceId = sourceId, AtlasCoords = atlasCoords }];
        }
    }
}