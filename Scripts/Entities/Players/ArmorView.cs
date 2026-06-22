using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.InventorySystem;
using Solo.Scripts.System.ItemSystem;

public partial class ArmorView : Control
{
    [Export] private Sprite2D _helmetSprite;
    [Export] private Sprite2D _ArmorSprite;
    [Export] private Sprite2D _bootSprite;
    [Export] public InventoryView ArmorInventoryView;

    public void RefreshVisuals(Inventory armorInventory)
    {
        _helmetSprite.Texture = null;
        _ArmorSprite.Texture = null;
        _bootSprite.Texture = null;

        for (int i = 0; i < armorInventory.ItemInstanceList.Count; i++)
        {
            if (armorInventory.ItemInstanceList[i] == null)
                continue;

            ItemData itemData = ItemDataManager.Instance.GetItemData(armorInventory.ItemInstanceList[i].Type);
            Texture2D texture = GD.Load<Texture2D>(itemData.IconPath);
            switch ((ArmorSlotType)i)
            {
                case ArmorSlotType.Helmet:
                    _helmetSprite.Texture = texture;
                    break;
                case ArmorSlotType.Armor:
                    _ArmorSprite.Texture = texture;
                    break;
                case ArmorSlotType.Boot:
                    _bootSprite.Texture = texture;
                    break;
            }
        }
    }
}
