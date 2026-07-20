using Godot;
using Solo.Scripts.System.InventorySystem;

namespace Solo.Scripts.UI.HUDs
{
    public partial class FastBarView : Control
    {
        private Inventory? _fastBarInventory;
        private int _curIndex = 0;
        [Export] GridContainer _slotGc = null!;
        [Export] PackedScene _fastBarSlotPs = null!;

        public void Init(Inventory fastBarInventory, int curIndex)
        {
            _fastBarInventory = fastBarInventory;
            _fastBarInventory.SlotChanged += (index) =>
            {
                FastBarSlot slot = _slotGc.GetChild<FastBarSlot>(index);
                slot.Refresh();
            };
            _curIndex = curIndex;

            ButtonGroup btnGroup = new ButtonGroup();

            foreach (Node node in _slotGc.GetChildren())
                node.QueueFree();

            for (int i = 0; i < _fastBarInventory.SlotList.Count; i++)
            {
                FastBarSlot slot = _fastBarSlotPs.Instantiate<FastBarSlot>();
                _slotGc.AddChild(slot);
                slot.Init(_fastBarInventory, i, btnGroup);
                if (i == _curIndex)
                {
                    slot.SetSelected(true);
                }
            }
        }

        public void SetSelected(int index)
        {
            FastBarSlot slot = _slotGc.GetChild<FastBarSlot>(index);
            slot.SetSelected(true);
        }
    }
}