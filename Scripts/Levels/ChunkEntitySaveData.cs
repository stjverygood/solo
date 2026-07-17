using Solo.Scripts.Entities.Core;
using System.Collections.Generic;

namespace Solo.Scripts.Levels
{
    public class ChunkEntitySaveData
    {
        public int ChunkX { get; set; }
        public int ChunkY { get; set; }
        public List<EntitySaveData> EntitySaveDataList { get; set; } = new List<EntitySaveData>();
    }
}
