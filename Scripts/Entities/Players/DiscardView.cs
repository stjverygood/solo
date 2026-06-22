using Godot;
using Godot.Collections;
using Solo.Scripts.Global;
using Solo.Scripts.System.InventorySystem;

public partial class DiscardView : Control
{
    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return data.VariantType == Variant.Type.Dictionary;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        Dictionary dict = (Dictionary)data;
        string sourceInventoryGuid = (string)dict["InventoryGuid"];
        int sourceIndex = (int)dict["Index"];

        Inventory sourceInv = GameManager.Instance.Player.GetInventoryByGuid(sourceInventoryGuid);
        if (sourceInv == null || sourceIndex >= sourceInv.ItemInstanceList.Count)
            return;

        sourceInv.RemoveItem(sourceIndex);
    }
}
