using Solo.Scripts.System.ItemSystem;
using Solo.Scripts.System.ResourceSystem;
using System.Collections.Generic;

namespace Solo.Scripts.System.ChunkSystem
{
    public class Chunk
    {
        public List<ResObj> ResObjList = new List<ResObj>();//活跃区块的资源物
        public List<DropItem> DropItemList = new List<DropItem>();//活跃区块的掉落物
    }
}
