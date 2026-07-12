using Godot;
using Solo.Scripts.System.InventorySystem;

public partial class ItemView : Control
{
    [Export] private InventoryView _bagInventoryView = null!;
    [Export] private InventoryView _armorInventoryView = null!;
    [Export] private DiscardView _discardView = null!;
    public void Init(Inventory bagInventory, Inventory armorInventory)
    {
        _bagInventoryView.Init(bagInventory);
        _armorInventoryView.Init(armorInventory);
    }
}
