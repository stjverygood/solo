using Solo.Scripts.Entities.Core;
using System.Collections.Generic;

namespace Solo.Scripts.Levels
{
    public class ChunkEntitySaveData
    {
        public int ChunkPosX { get; set; }
        public int ChunkPosY { get; set; }
        public List<EntitySaveData> EntitySaveDataList { get; set; } = new List<EntitySaveData>();
    }
}
