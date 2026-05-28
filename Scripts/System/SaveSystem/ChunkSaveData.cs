using System.Collections.Generic;

namespace Solo.Scripts.System.SaveSystem
{
    public class ChunkSaveData
    {
        //public Vector2I Position { get; set; }//用于list->map
        public int X { get; set; }
        public int Y { get; set; }
        public List<BuildingSaveData> BuildingSaveDataList { get; set; }//记录区块内的资源物
        public List<DropItemSaveData> DropItemSaveDataList { get; set; }//记录区块内的掉落物
        public ChunkSaveData(int x, int y)
        {
            X = x;
            Y = y;
            BuildingSaveDataList = new List<BuildingSaveData>();
            DropItemSaveDataList = new List<DropItemSaveData>();
        }
    }
}
