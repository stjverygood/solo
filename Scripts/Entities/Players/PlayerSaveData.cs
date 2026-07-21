using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.System.InventorySystem;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Players
{
    public class PlayerSaveData : EntitySaveData
    {
        public float StartX { get; set; }
        public float StartY { get; set; }
        public RealmType RealmType { get; set; }
        public float CurHp { get; set; }
        public float CurQi { get; set; }
        public float CurExp { get; set; }
        public List<InventorySlot>? FastBarInventorySlotList { get; set; }
        public List<InventorySlot>? BagInventorySlotList { get; set; }
        public List<InventorySlot>? EquipmentInventorySlotList { get; set; }
    }
}
