using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemData
    {
        public string Name = "";
        public string IconPath = "";


        public byte MaxCount = 99;
        public int MaxDur = -1;//默认-1, 没有耐久度
        public bool IsArmor = false;
        public ArmorSlotType ArmorSlot = ArmorSlotType.Helmet;
        public int AtkBonus = 0;
        public int DefBonus = 0;
        public float MaxHpBonus = 0;
        public float MaxQiBonus = 0;
        public int MoveSpeedBonus = 0;
        public bool CanBuild = false;
        public bool CanAim = false;
        public bool CanConsume = false;

        public List<(ItemType, int)> CraftRequiredItemList = new();//合成所需的材料
    }
}
