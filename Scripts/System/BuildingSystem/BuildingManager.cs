using Godot;
using System.Collections.Generic;

namespace Solo.Scripts.System.BuildingSystem
{
    public class BuildingManager
    {
        private HashSet<Vector2I> _placedCellPosSet = new HashSet<Vector2I>();//被占用了的格子坐标 
        private int _cellSize;
        public BuildingManager(int gridSize)
        {
            _cellSize = gridSize;
        }

        public Vector2 SnapToCell(BuildingData buildingData, Vector2 worldPos)
        {
            Vector2 snappedPos = Vector2.Zero;
            // X 轴吸附逻辑
            if (buildingData.Width % 2 == 1) // 奇数尺寸（1, 3, 5...）-> 吸附到格子中心
                snappedPos.X = Mathf.Floor(worldPos.X / _cellSize) * _cellSize + _cellSize / 2;
            else // 偶数尺寸（2, 4, 6...）-> 吸附到格子交点
                snappedPos.X = Mathf.Round(worldPos.X / _cellSize) * _cellSize;

            // Y 轴吸附逻辑
            if (buildingData.Height % 2 == 1) // 奇数尺寸 -> 吸附到格子中心
                snappedPos.Y = Mathf.Floor(worldPos.Y / _cellSize) * _cellSize + _cellSize / 2;
            else // 偶数尺寸 -> 吸附到格子交点
                snappedPos.Y = Mathf.Round(worldPos.Y / _cellSize) * _cellSize;
            return snappedPos;
        }

        public bool CanPlaced(BuildingData buildingData, Vector2 snapWorldPos)
        {
            List<Vector2I> placedCellPosList = GetPlacedCellPosList(buildingData, snapWorldPos);
            foreach (Vector2I cellPos in placedCellPosList)
            {
                if (_placedCellPosSet.Contains(cellPos))
                    return false;
            }
            return true;
        }

        public void Place(BuildingData buildingData, Vector2 snapWorldPos)
        {
            if (!CanPlaced(buildingData, snapWorldPos)) return;

            List<Vector2I> targetCellPosList = GetPlacedCellPosList(buildingData, snapWorldPos);
            foreach (Vector2I cell in targetCellPosList)
            {
                _placedCellPosSet.Add(cell);
            }

            // 接下来在这里实例化你的建筑场景，把它的 Position 设为 snappedWorldPos 即可
        }

        public void Remove(BuildingData buildingData, Vector2 snapWorldPos)
        {
            List<Vector2I> targetCellPosList = GetPlacedCellPosList(buildingData, snapWorldPos);// 1. 获取该建筑占用的所有格子坐标
            foreach (Vector2I cell in targetCellPosList)// 2. 从已占用集合中将这些格子释放
            {
                _placedCellPosSet.Remove(cell);// 使用 Remove，如果存在就移除，不存在也不会报错
            }
        }

        private List<Vector2I> GetPlacedCellPosList(BuildingData buildingData, Vector2 snapWorldPos)
        {
            List<Vector2I> occupiedCells = new List<Vector2I>();

            // 核心逻辑：先找出建筑“左上角”那个格子所在的整数坐标
            // 无论奇偶，建筑中心点减去 1/2 的宽高，就是左上角的物理位置
            float leftTopX = snapWorldPos.X - (buildingData.Width * _cellSize) / 2f;
            float leftTopY = snapWorldPos.Y - (buildingData.Height * _cellSize) / 2f;

            // 将左上角的物理位置转换为网格的整数坐标 (Vector2I)
            int startCellX = Mathf.FloorToInt(leftTopX / _cellSize);
            int startCellY = Mathf.FloorToInt(leftTopY / _cellSize);

            // 从左上角开始，遍历建筑的宽高，拿到所有格子
            for (int x = 0; x < buildingData.Width; x++)
            {
                for (int y = 0; y < buildingData.Height; y++)
                {
                    occupiedCells.Add(new Vector2I(startCellX + x, startCellY + y));
                }
            }

            return occupiedCells;
        }
    }
}
