using Godot;
using Godot.Collections;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;

public partial class InventorySlotView : Control
{
    [Export] private Panel _bgPanel;
    [Export] private TextureRect _iconTr;
    [Export] private Label _nameLb;
    [Export] private Label _countLb;
    private ItemInstance _itemInstance;
    private int _index;
    private InventoryView _parent;
    [Export] private StyleBoxFlat _normalStyle;
    [Export] private StyleBoxFlat _selectedStyle;

    public void Init(InventoryView parent, int index)
    {
        _parent = parent;
        _index = index;
        _itemInstance = null;
        _iconTr.Texture = null;
        _nameLb.Text = "";
        _countLb.Text = "";
        SetSelected(false);
        PivotOffset = Size / 2;
    }

    public void SetData(ItemInstance itemInstance)
    {

        if (itemInstance == null)
        {
            _iconTr.Texture = null;
            _nameLb.Text = "";
            _countLb.Text = "";
            return;
        }

        _itemInstance = itemInstance;
        _iconTr.Texture = GD.Load<Texture2D>(ItemDataManager.Instance.GetItemData(_itemInstance.Type).IconPath);
        _countLb.Text = $"{_itemInstance.Count}";
        _nameLb.Text = $"{ItemDataManager.Instance.GetItemData(_itemInstance.Type).Name}";
    }

    public override Variant _GetDragData(Vector2 atPosition)
    {
        GD.Print("_GetDragData atPosition : " + atPosition);
        if (_itemInstance == null)
            return new Variant();

        TextureRect previewIconTr = new TextureRect();
        previewIconTr.Texture = _iconTr.Texture;
        previewIconTr.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        previewIconTr.StretchMode = TextureRect.StretchModeEnum.Scale;
        previewIconTr.Size = Size;

        Control previewContainer = new Control();
        previewContainer.ZIndex = 100;
        previewContainer.AddChild(previewIconTr);
        previewIconTr.Position = -atPosition;

        SetDragPreview(previewContainer);

        Dictionary dict = new Dictionary()
        {
            {"InventoryGuid", _parent.Inventory.GuidStr },
            {"Index",_index },
        };
        return dict;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
        return true;
    }

    public override void _DropData(Vector2 atPosition, Variant data)
    {
        Dictionary dict = (Dictionary)data;
        string sourceInventoryGuid = (string)dict["InventoryGuid"];
        int sourceIndex = (int)dict["Index"];
        if (sourceInventoryGuid == _parent.Inventory.GuidStr)//背包内拖动
        {
            if (sourceIndex == _index)
                return;
            _parent.Inventory.SwapItem(sourceIndex, _index);
        }
        else//跨背包拖动
        {
            GameManager.Instance.Player.SwapItemInterInventory(sourceInventoryGuid, sourceIndex, _parent.Inventory.GuidStr, _index);
        }


    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(this, "scale", new Vector2(0.8f, 0.8f), 0.1f);
            animTween.TweenProperty(this, "scale", Vector2.One, 0.1f);
            _bgPanel.AddThemeStyleboxOverride("panel", _selectedStyle);
        }
        else
        {
            _bgPanel.AddThemeStyleboxOverride("panel", _normalStyle);
        }
    }
}
