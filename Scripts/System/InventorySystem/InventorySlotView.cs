using Godot;
using Godot.Collections;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;

public partial class InventorySlotView : Control
{
    [Export] private TextureRect _iconTr;
    [Export] private Label _nameLb;
    [Export] private Label _countLb;
    private ItemInstance _itemInstance;
    private int _index;
    private InventoryView _parent;

    //public override void _Ready()
    //{

    //}

    public void Init(InventoryView parent, int index)
    {
        _parent = parent;
        _index = index;
        _itemInstance = null;
        _iconTr.Texture = null;
        _nameLb.Text = "";
        _countLb.Text = "";
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
        _iconTr.Texture = GD.Load<Texture2D>(_itemInstance.Data.IconPath);
        _countLb.Text = $"{_itemInstance.Count}";
        _nameLb.Text = $"{_itemInstance.Data.Type}";
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
        previewContainer.AddChild(previewIconTr);
        previewIconTr.Position = -atPosition;

        SetDragPreview(previewContainer);

        Dictionary dict = new Dictionary()
        {
            {"InventoryType",(int)_parent.Inventory.Type },
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
        InventoryType sourceInventoryType = (InventoryType)(int)dict["InventoryType"];
        int sourceIndex = (int)dict["Index"];
        if (sourceInventoryType == _parent.Inventory.Type)//背包内拖动
        {
            if (sourceIndex == _index)
                return;
            _parent.Inventory.SwapItem(sourceIndex, _index);
        }
        else//跨背包拖动
        {

        }


    }
}
