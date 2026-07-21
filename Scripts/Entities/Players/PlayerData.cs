using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.System.InventorySystem;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Players
{
    public class PlayerData : EntityData
    {
        public RealmType RealmType = RealmType.LianQi1;
        public int FastBarIndex = 0;
        public List<InventorySlot> FastBarInventorySlotList { get; set; }
        public List<InventorySlot> BagInventorySlotList { get; set; }
        public List<InventorySlot> EquipmentInventorySlotList { get; set; }
        //public float 
        public PlayerData()
        {
            FastBarInventorySlotList = new List<InventorySlot>(4);
            for (int i = 0; i < 4; i++)
            {
                FastBarInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Common });
            }

            BagInventorySlotList = new List<InventorySlot>(16);
            for (int i = 0; i < 16; i++)
            {
                BagInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Common });
            }

            EquipmentInventorySlotList = new List<InventorySlot>(3);
            EquipmentInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Helmet });
            EquipmentInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Armor });
            EquipmentInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Boot });
        }
    }


}
