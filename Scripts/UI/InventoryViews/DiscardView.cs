using Godot;
using Solo.Scripts.Components.InventoryComponents;
using Solo.Scripts.Global;
using Solo.Scripts.System.InventorySystem;

namespace Solo.Scripts.UI.InventoryViews
{
    public partial class DiscardView : Control
    {
        public override bool _CanDropData(Vector2 atPosition, Variant data)
        {
            return true;
        }

        public override void _DropData(Vector2 atPosition, Variant data)
        {
            InventorySlotView sourceSlotView = data.As<InventorySlotView>();
            GameManager.Instance.Player.Core.GetComponent<InventoryComponent>().RemoveItem(sourceSlotView.Inventory, sourceSlotView.Index);
        }
    }
}
