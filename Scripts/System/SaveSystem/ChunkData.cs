using System.Collections.Generic;

namespace Solo.Scripts.System.SaveSystem
{
    public class ChunkData
    {
        //public Vector2I Pos { get; set; }
        public List<ResObjData> ResObjDataList { get; set; }//记录区块内的资源物
        public List<DropItemData> DropItemDataList { get; set; }//记录区块内的掉落物
        public ChunkData()
        {
            ResObjDataList = new List<ResObjData>();
            DropItemDataList = new List<DropItemData>();
        }
    }
}
