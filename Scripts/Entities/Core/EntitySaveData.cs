using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Core
{

    public class EntitySaveData
    {
        public required EntityType Type { get; set; }
        public required List<ComponentSaveData> ComponentSaveDataList { get; set; }
        //public float WorldX { get; set; }
        //public float WorldY { get; set; }
    }
}
