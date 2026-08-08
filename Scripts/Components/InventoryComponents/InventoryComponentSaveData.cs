using Solo.Scripts.Components.Core;
using Solo.Scripts.System.InventorySystem;
using System.Collections.Generic;

namespace Solo.Scripts.Components.InventoryComponents
{
    public class InventoryComponentSaveData : ComponentSaveData
    {
        public required List<InventorySlot> FastBarInventorySlotList { get; set; }
        public required List<InventorySlot> BagInventorySlotList { get; set; }
        public required List<InventorySlot> EquipmentInventorySlotList { get; set; }
        public int FastBarIndex;
    }
}
