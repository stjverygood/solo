using Godot;
using Solo.Scripts.System.InventorySystem;

public partial class ItemView : Control
{
    [Export] private InventoryView _bagInventoryView;
    [Export] private InventoryView _armorInventoryView;
    [Export] private DiscardView _discardView;
    public void Init(Inventory bagInventory, Inventory armorInventory)
    {
        _bagInventoryView.Init(bagInventory);
        _armorInventoryView.Init(armorInventory);
    }
}
