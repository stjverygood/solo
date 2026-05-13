using Godot;
using System;
using System.Collections.Generic;

public enum CellType : byte
{
    Water,
    Earth,
    Stone,
    Sand,
}

public struct GridCell
{
    public CellType Type; 
}

public partial class MapManager : Node2D
{
    [Export] public int MapSize = 512 * 1;
    [Export] public int CellSize = 32;

    [Export] public MultiMeshInstance2D EarthMultiMesh;
    [Export] public MultiMeshInstance2D WaterMultiMesh;
    [Export] public float NoiseFrequency = 0.05f;// 噪声参数控制地形“破碎”程度

    [Export] public PackedScene ResourcePs;

    private StaticBody2D _waterBody;
    private GridCell[,] _mapData;
    private Dictionary<CellType, List<Vector2I>> _cellCacheMap = new Dictionary<CellType, List<Vector2I>>();

    public override void _Ready()
    {
    }

    public void Init()
    {
        _mapData = new GridCell[MapSize, MapSize];
        GenerateRealisticMap();
        RenderMap();
        CreateWaterCollisions(); // 新增这行
    }


    private void GenerateRealisticMap()
    {
        FastNoiseLite noise = new FastNoiseLite();
        noise.Seed = (int)GD.Randi();
        noise.Frequency = NoiseFrequency;
        noise.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;

        // 土地阈值：FastNoiseLite 输出范围是 -1 到 1
        // 我们可以根据需要调整这个值。0.2 左右通常能留下约 30% 的土地
        float landThreshold = 0f;

        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                // 注意：这里改成了 GetNoise2D
                float val = noise.GetNoise2D(x, y);
                GridCell curCell = new GridCell { Type = CellType.Water };
                if (val > landThreshold)
                {
                    curCell = new GridCell { Type = CellType.Earth }; // 土地
                }
                else
                {
                    curCell = new GridCell { Type = CellType.Water }; // 水
                }
                _mapData[x, y] = curCell;
            }
        }

        // 后续的连通性检查逻辑保持不变
        EnsureConnectivity();
        UpdateCellCache();
    }

    private void UpdateCellCache()
    {
        _cellCacheMap.Clear();
        // 初始化所有类型的 List
        foreach (CellType type in Enum.GetValues(typeof(CellType)))
        {
            _cellCacheMap[type] = new List<Vector2I>();
        }

        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                _cellCacheMap[_mapData[x, y].Type].Add(new Vector2I(x, y));
            }
        }
    }

    private void EnsureConnectivity()
    {
        bool[,] visited = new bool[MapSize, MapSize];
        List<List<Vector2I>> islands = new List<List<Vector2I>>();

        // 寻找所有独立的土地岛屿
        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                if (_mapData[x, y].Type == CellType.Earth && !visited[x, y])
                {
                    islands.Add(ExploreIsland(x, y, visited));
                }
            }
        }

        if (islands.Count <= 1) return;

        // 按面积排序，保留最大的岛屿
        islands.Sort((a, b) => b.Count.CompareTo(a.Count));

        // 将除了最大岛屿之外的所有土地变成水
        for (int i = 1; i < islands.Count; i++)
        {
            foreach (var cell in islands[i])
            {
                _mapData[cell.X, cell.Y] = new GridCell { Type = CellType.Water };
            }
        }
    }

    // 经典的 BFS 洪水填充算法
    private List<Vector2I> ExploreIsland(int startX, int startY, bool[,] visited)
    {
        List<Vector2I> cells = new List<Vector2I>();
        Queue<Vector2I> queue = new Queue<Vector2I>();

        Vector2I start = new Vector2I(startX, startY);
        queue.Enqueue(start);
        visited[startX, startY] = true;

        Vector2I[] directions = { Vector2I.Up, Vector2I.Down, Vector2I.Left, Vector2I.Right };

        while (queue.Count > 0)
        {
            Vector2I curr = queue.Dequeue();
            cells.Add(curr);

            foreach (var dir in directions)
            {
                Vector2I next = curr + dir;
                if (next.X >= 0 && next.X < MapSize && next.Y >= 0 && next.Y < MapSize)
                {
                    if (_mapData[next.X, next.Y].Type == CellType.Earth && !visited[next.X, next.Y])
                    {
                        visited[next.X, next.Y] = true;
                        queue.Enqueue(next);
                    }
                }
            }
        }
        return cells;
    }

    private void RenderMap()
    {
        int waterCount = 0;
        int earthCount = 0;

        // 1. 统计数量（确保判断条件与下方填充时完全一致）
        foreach (var cell in _mapData)
        {
            if (cell.Type == CellType.Earth) earthCount++;
            else if (cell.Type == CellType.Water) waterCount++;
        }

        PrepareMultiMesh(EarthMultiMesh, earthCount);
        PrepareMultiMesh(WaterMultiMesh, waterCount);

        int earthIdx = 0;
        int waterIdx = 0;

        Vector2 offset = new Vector2(CellSize / 2f, CellSize / 2f);

        for (int x = 0; x < MapSize; x++)
        {
            for (int y = 0; y < MapSize; y++)
            {
                // 在原始坐标基础上加上 offset
                Vector2 finalPos = new Vector2(x * CellSize, y * CellSize) + offset;
                Transform2D transform = new Transform2D(0, finalPos);

                if (_mapData[x, y].Type == CellType.Earth)
                {
                    EarthMultiMesh.Multimesh.SetInstanceTransform2D(earthIdx++, transform);
                }
                else if (_mapData[x, y].Type == CellType.Water)
                {
                    WaterMultiMesh.Multimesh.SetInstanceTransform2D(waterIdx++, transform);
                }
            }
        }
    }

    private void CreateWaterCollisions()
    {
        // 1. 清理旧的碰撞体（防止重新生成地图时叠加）
        if (_waterBody != null)
        {
            _waterBody.QueueFree();
        }

        _waterBody = new StaticBody2D();
        _waterBody.Name = "WaterStaticBody";
        AddChild(_waterBody);

        // 2. 调用你写的 FastMerge 获取合并后的矩形顶点
        // 这里的参数可以是 null，因为你的 FastMerge 内部是直接读 _mapData 的
        Vector2[][] mergedRects = FastMerge(null);

        // 3. 遍历顶点数组，创建 CollisionPolygon2D
        foreach (Vector2[] points in mergedRects)
        {
            CollisionPolygon2D colPoly = new CollisionPolygon2D();
            colPoly.Polygon = points;
            _waterBody.AddChild(colPoly);
        }
    }

    private Vector2[][] FastMerge(List<Vector2[]> ignored)
    {
        // 这种方法通过合并同一行内连续的格子来减少碰撞体数量
        List<Vector2[]> mergedRects = new List<Vector2[]>();

        for (int y = 0; y < MapSize; y++)
        {
            for (int x = 0; x < MapSize; x++)
            {
                if (_mapData[x, y].Type == CellType.Water)
                {
                    int startX = x;
                    // 找出横向连续的水域
                    while (x < MapSize && _mapData[x, y].Type == CellType.Water)
                    {
                        x++;
                    }
                    int endX = x;

                    Vector2[] rect = new Vector2[]
                    {
                    new Vector2(startX * CellSize, y * CellSize),
                    new Vector2(endX * CellSize, y * CellSize),
                    new Vector2(endX * CellSize, (y + 1) * CellSize),
                    new Vector2(startX * CellSize, (y + 1) * CellSize)
                    };
                    mergedRects.Add(rect);
                }
            }
        }
        return mergedRects.ToArray();
    }

    private void PrepareMultiMesh(MultiMeshInstance2D instance, int count)
    {
        if (instance == null || instance.Multimesh == null)
        {
            GD.PrintErr($"请在编辑器中为 {instance?.Name} 分配 MultiMesh 资源");
            return;
        }
        instance.Multimesh.InstanceCount = count;
        instance.Multimesh.VisibleInstanceCount = count;
    }

    /// <summary>
    /// 随机获取一个指定类型的坐标
    /// </summary>
    public Vector2 GetRandomCellWorldPosition(CellType type)
    {
        if (!_cellCacheMap.ContainsKey(type) || _cellCacheMap[type].Count == 0)
            return Vector2.Zero;

        var list = _cellCacheMap[type];
        int randomIndex = (int)(GD.Randi() % (uint)list.Count);
        Vector2I cell = list[randomIndex];

        // 转换为世界坐标，并偏移到格子中心
        return new Vector2(cell.X * CellSize + CellSize / 2f, cell.Y * CellSize + CellSize / 2f);
    }

    public void GenerateResource(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = GetRandomCellWorldPosition(CellType.Earth);
            ResourceBase res = ResourcePs.Instantiate<ResourceBase>();
            AddChild(res);
            res.GlobalPosition = pos;
        }
    }
}