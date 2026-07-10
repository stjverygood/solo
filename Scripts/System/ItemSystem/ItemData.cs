using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemData
    {
        public string Name = "";
        public byte MaxCount = 1;
        public string IconPath = "";
        public int MaxDur = -1;
        public bool IsArmor = false;
        public ArmorSlotType ArmorSlot = ArmorSlotType.Helmet;
        public int DefBonus = 0;
        public int AtkBonus = 0;
        public int MoveSpeedBonus = 0;
        public float HpBonus = 0;
        public float MpBonus = 0;
        public bool CanBuild = false;
        public bool CanAim = false;
        public bool CanConsume = false;

        public List<(ItemType, int)> CraftRequiredItemList;//合成所需的材料
    }
}
