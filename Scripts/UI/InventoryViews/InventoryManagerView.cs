using Godot;
using Solo.Scripts.Entities.Components.InventoryComponents;
using Solo.Scripts.System.InventorySystem;

namespace Solo.Scripts.UI.InventoryViews
{
    public partial class InventoryManagerView : Control
    {
        [Export] private InventoryView _fastInventoryView = null!;
        [Export] private InventoryView _bagInventoryView = null!;
        [Export] private InventoryView _equipmentInventoryView = null!;
        public void Init(InventoryComponent inventoryComponent)
        {
            _fastInventoryView.Init(inventoryComponent.FastBarInventory);
            _bagInventoryView.Init(inventoryComponent.BagInventory);
            _equipmentInventoryView.Init(inventoryComponent.EquipmentInventory);
        }
    }
}
