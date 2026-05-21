using Godot;
using Solo.Scripts.System.InventorySystem;
using System.Collections.Generic;

public partial class InventoryView : Control
{
    [Export] public PackedScene SlotViewPs;
    [Export] private GridContainer _gridContainer;
    private Inventory _inventory;
    private List<InventorySlotView> _slotViewList = new List<InventorySlotView>();

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
    }

    public void Init(Inventory inventory)
    {
        _inventory = inventory;
        for (int i = 0; i < _inventory.ItemInstanceList.Count; i++)
        {
            InventorySlotView slotView = SlotViewPs.Instantiate<InventorySlotView>();
            _gridContainer.AddChild(slotView);
            _slotViewList.Add(slotView);
        }
        _inventory.SlotChanged += (int index) =>
        {
            GD.Print("SlotChangedSlotChangedSlotChanged");
            _slotViewList[index].SetData(GD.Load<Texture2D>(_inventory.ItemInstanceList[index].Data.IconPath), _inventory.ItemInstanceList[index].Count);
        };
    }
}
