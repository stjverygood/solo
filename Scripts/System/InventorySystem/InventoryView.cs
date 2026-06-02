using Godot;
using Solo.Scripts.System.InventorySystem;
using System.Collections.Generic;

public partial class InventoryView : Control
{
    [Export] public PackedScene SlotViewPs;
    [Export] private GridContainer _gridContainer;
    public Inventory Inventory;
    private List<InventorySlotView> _slotViewList = new List<InventorySlotView>();

    public override void _Ready()
    {
        GD.Print("InventoryView Ready~~~");
    }

    public override void _Process(double delta)
    {
    }

    public void Init(Inventory inventory)
    {
        Inventory = inventory;
        for (int i = 0; i < Inventory.ItemInstanceList.Count; i++)
        {
            InventorySlotView slotView = SlotViewPs.Instantiate<InventorySlotView>();
            slotView.Init(this, i);//SetData
            slotView.SetData(Inventory.ItemInstanceList[i]);
            _gridContainer.AddChild(slotView);
            _slotViewList.Add(slotView);
        }
        Inventory.SlotChanged += (int index) =>
        {
            _slotViewList[index].SetData(Inventory.ItemInstanceList[index]);
        };
    }

    public void SetSelected(int index, bool isSelected)
    {
        _slotViewList[index].SetSelected(isSelected);
    }

    public void RefreshSlot(int index)
    {
        _slotViewList[index].SetData(Inventory.ItemInstanceList[index]);
    }
}
