using Godot;
using Solo.Scripts.System.InventorySystem;
namespace Solo.Scripts.Entities.Players
{
    namespace Solo.Scripts.Entities.Players
    {
        public partial class InventoryManagerView : Control
        {
            [Export] private InventoryView _fastInventoryView = null!;
            [Export] private InventoryView _bagInventoryView = null!;
            [Export] private InventoryView _equipmentInventoryView = null!;
            public void Init(InventoryManager inventoryManager)
            {
                _fastInventoryView.Init(inventoryManager.FastBarInventory);
                _bagInventoryView.Init(inventoryManager.BagInventory);
                _equipmentInventoryView.Init(inventoryManager.EquipmentInventory);
            }
        }
    }
}