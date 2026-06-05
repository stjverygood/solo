using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.BuildingSystem
{
    public class BuildingData
    {
        public BuildingType Type;
        public int Width = 1;
        public int Height = 1;
        public string TexturePath = "";
        public int MaxHp = 100;
        public List<(ItemType, int, int)> DropItemList;//掉落物类型, 最小掉落数量, 最大掉落数量
    }
}
