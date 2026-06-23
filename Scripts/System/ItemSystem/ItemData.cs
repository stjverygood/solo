using Solo.Scripts.Global;

namespace Solo.Scripts.System.ItemSystem
{
    public class ItemData
    {
        public string Name = "";
        public byte MaxCount = 1;
        public string IconPath = "";
        public int MaxDur = -1;
        public bool IsBuilding = false;
        public bool IsArmor = false;
        public ArmorSlotType ArmorSlot = ArmorSlotType.Helmet;
        public int DefBonus = 0;
        public int AtkBonus = 0;
        public int MoveSpeedBonus = 0;
        public bool IsConsumable = false;
        public float HpBonus = 0;
        public float MpBonus = 0;
    }
}
