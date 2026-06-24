using Solo.Scripts.Entities.Units;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.ItemSystem;
using System.Collections.Generic;

namespace Solo.Scripts.System.ChunkSystem
{
    public class Chunk
    {
        public List<Building> BuildingList = new List<Building>();//活跃区块的建筑物
        public List<DropItem> DropItemList = new List<DropItem>();//活跃区块的掉落物
        public List<Unit> DayUnitList = new List<Unit>();//白天单位列表 
        public List<Unit> NightUnitList = new List<Unit>();//夜晚单位列表 
    }
}
