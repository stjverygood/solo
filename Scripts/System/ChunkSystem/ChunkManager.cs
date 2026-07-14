using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.SaveSystem;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.System.ChunkSystem
{
    public partial class ChunkManager : Node2D
    {
        [Export] private TileMapLayer _waterTileMapLayer = null!;
        [Export] private TileMapLayer _landTileMapLayer = null!;
        //[Export] private PackedScene _buildingPs;
        [Export] public PackedScene DropItemPs = null!;
        [Export] public PackedScene _unitPs = null!;

        [Export] public PackedScene _zombiePs = null!;

        //public BuildingManager BuildingManager;
        private FastNoiseLite _elevationNoise = new FastNoiseLite(); // 海拔/地形起伏,决定水/陆地
        private FastNoiseLite _moistureNoise = new FastNoiseLite();  // 湿度,决定陆地上具体是草地/森林/沙漠
        public int ChunkSize = 16;       // 每个区块的瓦片数量
        public int TileSize = 16;        // 每个瓦片的像素大小
        private int _renderDistance = 5;  // 区块渲染距离
        public Dictionary<Vector2I, Chunk> CurActiveChunkMap = new Dictionary<Vector2I, Chunk>();
        public Dictionary<Vector2I, ChunkSaveData> ChunkSaveDataMap = new Dictionary<Vector2I, ChunkSaveData>();

        private HashSet<Vector2I> _curChunkPosSet = new HashSet<Vector2I>();
        public event Action<Vector2I>? OnChunkLoaded;
        public event Action<Vector2I>? OnChunkUnloaded;


        private Dictionary<TileType, List<Vector2I>> TileCoordsListMap = new Dictionary<TileType, List<Vector2I>>()
        {
            { TileType.Grass, new List<Vector2I>(){ new Vector2I(1, 7)}},
            { TileType.Water, new List<Vector2I>(){ new Vector2I(1, 8)}},
            { TileType.Forest, new List<Vector2I>(){ new Vector2I(1, 9)}},
            { TileType.Desert, new List<Vector2I>(){ new Vector2I(1, 10)}},
            { TileType.Stone, new List<Vector2I>(){ new Vector2I(1, 11)}},
            { TileType.FireLand, new List<Vector2I>(){ new Vector2I(1, 12)}},
        };
        private Dictionary<Vector2I, TileType> TileTypeMap = new Dictionary<Vector2I, TileType>();//用于反查

        public void Init()
        {
            GameManager.Instance.ChunkManager = this;
            foreach (ChunkSaveData chunkSaveData in SaveManager.Instance.CurSaveData.ChunkSaveDataList)
            {
                ChunkSaveDataMap[new Vector2I(chunkSaveData.X, chunkSaveData.Y)] = chunkSaveData;
            }
            _elevationNoise.Seed = GD.Hash(SaveManager.Instance.CurSaveData.ChunkElevationNoiseSeedStr);
            _elevationNoise.Frequency = 0.005f;
            _moistureNoise.Seed = GD.Hash(SaveManager.Instance.CurSaveData.ChunkMoistureNoiseSeedStr);
            _moistureNoise.Frequency = 0.005f;
            foreach (var kvp in TileCoordsListMap)
            {
                foreach (Vector2I tileCoords in kvp.Value)
                    TileTypeMap[tileCoords] = kvp.Key;
            }

            RefreshChunk();
        }

        private float _refreshTimer = 0;// 卸载计时器，没必要每帧都检测卸载
        private float _refreshDuration = 1f; // 每秒检查一次卸载
        public override void _PhysicsProcess(double delta)
        {
            _refreshTimer += (float)delta;
            if (_refreshTimer < _refreshDuration)
                return;
            _refreshTimer = 0;

            RefreshChunk();
        }

        private void RefreshChunk()
        {
            Vector2I playerChunkPos = WorldToChunkPos(GameManager.Instance.Player.GlobalPosition);// 获取玩家所在的区块坐标
            for (int x = -_renderDistance; x <= _renderDistance; x++)//生存区块
            {
                for (int y = -_renderDistance; y <= _renderDistance; y++)
                {
                    Vector2I chunkPos = playerChunkPos + new Vector2I(x, y);
                    if (_curChunkPosSet.Contains(chunkPos) == false)//不存在的区块要加载/生成
                        LoadChunk(chunkPos);
                }
            }

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
        }

        public Vector2I WorldToChunkPos(Vector2 worldPos)
        {
            float px = Mathf.Floor(worldPos.X / (ChunkSize * TileSize));
            float py = Mathf.Floor(worldPos.Y / (ChunkSize * TileSize));
            return new Vector2I((int)px, (int)py);
        }

        private void LoadChunk(Vector2I chunkPos)
        {

            _curChunkPosSet.Add(chunkPos);
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    Vector2I globalPos = new Vector2I(chunkPos.X * ChunkSize + x, chunkPos.Y * ChunkSize + y);

                    float elevation = _elevationNoise.GetNoise2D(globalPos.X, globalPos.Y);
                    float moisture = _moistureNoise.GetNoise2D(globalPos.X, globalPos.Y);

                    if (elevation < -0f)
                    {
                        _waterTileMapLayer.SetCell(globalPos, 0, TileCoordsListMap[TileType.Water][0]);
                        continue;
                    }

                    if (elevation > 0.5f)
                    {
                        // 高海拔地区,比如火山地形
                        _landTileMapLayer.SetCell(globalPos, 0, TileCoordsListMap[TileType.FireLand][0]);
                        continue;
                    }

                    // 陆地上,根据湿度决定具体 biome
                    if (moisture < 0f)
                        _landTileMapLayer.SetCell(globalPos, 0, TileCoordsListMap[TileType.Desert][0]);
                    else if (moisture < 0.2f)
                        _landTileMapLayer.SetCell(globalPos, 0, TileCoordsListMap[TileType.Grass][0]);
                    else if (moisture < 0.5f)
                        _landTileMapLayer.SetCell(globalPos, 0, TileCoordsListMap[TileType.Forest][0]);
                    else
                        _landTileMapLayer.SetCell(globalPos, 0, TileCoordsListMap[TileType.Stone][0]);
                }
            }
        }
        private void UnloadChunk(Vector2I chunkPos)
        {
            _curChunkPosSet.Remove(chunkPos);
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    Vector2I globalPos = new Vector2I(chunkPos.X * ChunkSize + x, chunkPos.Y * ChunkSize + y);
                    _waterTileMapLayer.SetCell(globalPos, -1);
                    _landTileMapLayer.SetCell(globalPos, -1);
                }
            }
        }

        public TileType GetTileType(Vector2 worldPos)
        {
            Vector2I atlasCoords = _waterTileMapLayer.GetCellAtlasCoords(_waterTileMapLayer.LocalToMap(_waterTileMapLayer.ToLocal(worldPos)));
            if (atlasCoords != new Vector2I(-1, -1) && TileTypeMap.TryGetValue(atlasCoords, out TileType tileType))
                return tileType;
            atlasCoords = _landTileMapLayer.GetCellAtlasCoords(_landTileMapLayer.LocalToMap(_landTileMapLayer.ToLocal(worldPos)));
            if (TileTypeMap.TryGetValue(atlasCoords, out tileType))
                return tileType;
            return TileType.Grass;
        }

        public override void _ExitTree()
        {
            foreach (Vector2I chunkPos in new List<Vector2I>(CurActiveChunkMap.Keys))
            {
                UnloadChunk(chunkPos);
            }
        }
    }
}