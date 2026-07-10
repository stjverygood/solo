using Godot;
using System.Collections.Generic;
namespace Solo.Scripts.System.InventorySystem
{
    public partial class InventoryView : Control
    {
        [Export] public PackedScene SlotViewPs;
        [Export] private GridContainer _gridContainer;
        public Inventory Inventory;
        private List<InventorySlotView> _slotViewList = new List<InventorySlotView>();

        public void Init(Inventory inventory)
        {
            Inventory = inventory;

            foreach (Node child in _gridContainer.GetChildren())
                child.QueueFree();

            for (int i = 0; i < Inventory.SlotList.Count; i++)
            {
                InventorySlotView slotView = SlotViewPs.Instantiate<InventorySlotView>();
                slotView.Init(Inventory, i);//SetData
                slotView.SetData(Inventory.SlotList[i].ItemInstance);
                _gridContainer.AddChild(slotView);
                _slotViewList.Add(slotView);
            }
            Inventory.SlotChanged += (int index) =>
            {
                _slotViewList[index].SetData(Inventory.SlotList[index].ItemInstance);
            };
        }

        public void SetSelected(int index, bool isSelected)
        {
            _slotViewList[index].SetSelected(isSelected);
        }

        public void RefreshSlot(int index)
        {
            _slotViewList[index].SetData(Inventory.SlotList[index].ItemInstance);
        }
    }
}