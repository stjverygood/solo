using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.ItemSystem;
using System.Collections.Generic;

namespace Solo.Scripts.System.ChunkSystem
{
    public class Chunk
    {
        //public List<ResObj> ResObjList = new List<ResObj>();//活跃区块的资源物
        public List<BuildingBase> BuildingList = new List<BuildingBase>();//活跃区块的建筑物
        public List<DropItem> DropItemList = new List<DropItem>();//活跃区块的掉落物
    }
}
