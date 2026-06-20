using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.BuildingSystem
{
    public class BuildingData
    {
        public TargetType TargetType;
        public int Width = -1;
        public int Height = -1;
        //public int TextureHeight = -1;
        public int MaxHp = -1;
        public string TexturePath = "";//给preview用
        public string TscnPath;
        public List<(ItemType, int, int)> DropItemList;//掉落物类型, 最小掉落数量, 最大掉落数量
        //public List<string> ComponentPsPath;
    }
}
