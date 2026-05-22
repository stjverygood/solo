using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;
using Solo.Scripts.System.ResourceSystem;
using System.Collections.Generic;

public struct TileInfo
{
    public int SourceId;
    public Vector2I AtlasCoords;
}

public partial class ChunkManager : Node2D
{
    [Export] private TileMapLayer _tileMapLayer;
    [Export] private int _chunkSize = 16;       // 每个区块的瓦片数量
    [Export] private int _tileSize = 16;        // 每个瓦片的像素大小
    [Export] private int _renderDistance = 2;  // 玩家视野内的区块半径
    [Export] private string _seedString = "test_seed_3";

    [Export] public PackedScene TreePs;
    [Export] public PackedScene StonePs;
    [Export] public PackedScene DropItemPs;
    private List<Node2D> _genNodeList = new List<Node2D>();//记录生成的节点, 在卸载区块时要销毁

    private Dictionary<TileInfo, TileType> _tileInfoTypeMap = new Dictionary<TileInfo, TileType>()
    {
        { new TileInfo() { SourceId = 0, AtlasCoords = new Vector2I(1, 1) }, TileType.Grass },
        { new TileInfo() { SourceId = 1, AtlasCoords = new Vector2I(0, 0) }, TileType.Water },
        { new TileInfo() { SourceId = 2, AtlasCoords = new Vector2I(0, 0) }, TileType.Dirt },
    };
    private Dictionary<TileType, TileInfo> _tileTypeInfoMap = new Dictionary<TileType, TileInfo>()
    {
        { TileType.Grass,new TileInfo() { SourceId = 0, AtlasCoords = new Vector2I(1, 1) }  },
        { TileType.Water,new TileInfo() { SourceId = 1, AtlasCoords = new Vector2I(0, 0) } },
        { TileType.Dirt, new TileInfo() { SourceId = 2, AtlasCoords = new Vector2I(0, 0) } },
    };


    private FastNoiseLite _noise = new FastNoiseLite();
    private HashSet<Vector2I> _generatedChunks = new HashSet<Vector2I>();// 存储已生成的区块坐标

    private double _cleanupTimer = 0;// 卸载计时器，没必要每帧都检测卸载
    private const double CleanupInterval = 1.0; // 每秒检查一次卸载

    public override void _Ready()
    {
        GameManager.Instance.ChunkManager = this;
        // 1. 初始化噪声
        _noise.Seed = GD.Hash(_seedString);
        _noise.Frequency = 0.05f;
        _noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
    }

    public override void _PhysicsProcess(double delta)
    {
        var player = GameManager.Instance.Player;
        if (player == null) return;


        Vector2I playerChunk = GetChunkCoords(player.GlobalPosition);// 获取玩家所在的区块坐标
        for (int x = -_renderDistance; x <= _renderDistance; x++)
        {
            for (int y = -_renderDistance; y <= _renderDistance; y++)
            {
                Vector2I chunkCoord = playerChunk + new Vector2I(x, y);
                if (!_generatedChunks.Contains(chunkCoord))
                {
                    GenerateChunk(chunkCoord);
                }
            }
        }

        _cleanupTimer += delta;
        if (_cleanupTimer >= CleanupInterval)
        {
            CleanupFarChunks(playerChunk);
            _cleanupTimer = 0;
        }
    }

    private void GenerateChunk(Vector2I chunkCoord)
    {
        for (int x = 0; x < _chunkSize; x++)
        {
            for (int y = 0; y < _chunkSize; y++)
            {
                int globalX = chunkCoord.X * _chunkSize + x;
                int globalY = chunkCoord.Y * _chunkSize + y;

                float noiseVal = _noise.GetNoise2D(globalX, globalY);
                if (noiseVal < 0)//水
                {
                    _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Water].SourceId, _tileTypeInfoMap[TileType.Water].AtlasCoords);
                }
                else if (noiseVal < 0.3)//草
                {
                    //todo : 生成树木
                    float rd = GD.Randf();

                    if (rd < 0.1)
                    {
                        ResItem treeRes = TreePs.Instantiate<ResItem>();
                        Vector2 tileWorldPos = new Vector2(globalX * _tileSize, globalY * _tileSize);
                        Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);// 计算偏移量，使物体位于瓦片中心 (假设 _tileSize 是 16，偏移就是 8)
                        treeRes.GlobalPosition = tileWorldPos + offset;
                        GetTree().CurrentScene.AddChild(treeRes);


                    }
                    else if (rd < 0.15)
                    {
                        DropItem goldDropItem = DropItemPs.Instantiate<DropItem>();
                        Vector2 tileWorldPos = new Vector2(globalX * _tileSize, globalY * _tileSize);
                        Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);// 计算偏移量，使物体位于瓦片中心 (假设 _tileSize 是 16，偏移就是 8)
                        goldDropItem.GlobalPosition = tileWorldPos + offset;
                        GetTree().CurrentScene.AddChild(goldDropItem);
                        goldDropItem.Init(new ItemInstance() { Data = ItemManager.Instance.GetItemData(ItemType.Gold), Count = 3 });
                    }
                    _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Grass].SourceId, _tileTypeInfoMap[TileType.Grass].AtlasCoords);
                }
                else//泥
                {
                    if (GD.Randf() < 0.1)
                    {
                        ResItem stonePs = StonePs.Instantiate<ResItem>();
                        Vector2 tileWorldPos = new Vector2(globalX * _tileSize, globalY * _tileSize);
                        Vector2 offset = new Vector2(_tileSize / 2f, _tileSize / 2f);// 计算偏移量，使物体位于瓦片中心 (假设 _tileSize 是 16，偏移就是 8)
                        stonePs.GlobalPosition = tileWorldPos + offset;
                        GetTree().CurrentScene.AddChild(stonePs);
                    }
                    _tileMapLayer.SetCell(new Vector2I(globalX, globalY), _tileTypeInfoMap[TileType.Dirt].SourceId, _tileTypeInfoMap[TileType.Dirt].AtlasCoords);

                }
            }
        }
        _generatedChunks.Add(chunkCoord);
    }

    private void CleanupFarChunks(Vector2I playerChunk)
    {
        // 卸载距离建议比加载距离大 1-2，防止在边缘反复生成/卸载
        int unloadDistance = _renderDistance + 2;
        List<Vector2I> toRemove = new List<Vector2I>();

        foreach (var chunk in _generatedChunks)
        {
            if (Mathf.Abs(chunk.X - playerChunk.X) > unloadDistance || Mathf.Abs(chunk.Y - playerChunk.Y) > unloadDistance)
            {
                toRemove.Add(chunk);
            }
        }

        foreach (var chunk in toRemove)
        {
            UnloadChunk(chunk);
        }
    }

    private void UnloadChunk(Vector2I chunkCoord)
    {
        for (int x = 0; x < _chunkSize; x++)
        {
            for (int y = 0; y < _chunkSize; y++)
            {
                int globalX = chunkCoord.X * _chunkSize + x;
                int globalY = chunkCoord.Y * _chunkSize + y;
                _tileMapLayer.SetCell(new Vector2I(globalX, globalY), -1);// 设置为 -1 表示移除瓦片
            }
        }
        _generatedChunks.Remove(chunkCoord);
    }

    private Vector2I GetChunkCoords(Vector2 worldPos)
    {
        float px = Mathf.Floor(worldPos.X / (_chunkSize * _tileSize));
        float py = Mathf.Floor(worldPos.Y / (_chunkSize * _tileSize));
        return new Vector2I((int)px, (int)py);
    }


    public TileType GetTileType(Vector2 worldPos)
    {
        Vector2I mapCoords = _tileMapLayer.LocalToMap(_tileMapLayer.ToLocal(worldPos));

        // 2. 从 TileMapLayer 获取该坐标下的 SourceID 和 AtlasCoords
        int sourceId = _tileMapLayer.GetCellSourceId(mapCoords);
        Vector2I atlasCoords = _tileMapLayer.GetCellAtlasCoords(mapCoords);

        return _tileInfoTypeMap[new TileInfo() { SourceId = sourceId, AtlasCoords = atlasCoords }];
    }
}
