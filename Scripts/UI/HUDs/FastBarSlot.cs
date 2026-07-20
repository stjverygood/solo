using Godot;
using Solo.Scripts.System.InventorySystem;
using Solo.Scripts.System.ItemSystem;

public partial class FastBarSlot : PanelContainer
{
    [Export] private Label _indexLb = null!;
    [Export] private TextureRect _iconTr = null!;
    [Export] private Label _nameLb = null!;
    [Export] private Label _countLb = null!;
    [Export] private TextureProgressBar _durTpb = null!;
    [Export] private Button _bgBtn = null!;
    private ItemInstance? _itemInstance;
    public int Index;
    public Inventory? Inventory;


    //style : 
    //private bool _isSelected;
    //private bool _isHover;
    //[Export] private StyleBoxTexture _normalStyle;
    //[Export] private StyleBoxTexture _hoverStyle;
    //[Export] private StyleBoxTexture _selectedStyle;

    public void Init(Inventory inventory, int index, ButtonGroup btnGroup)
    {
        Inventory = inventory;
        Index = index;
        _itemInstance = null;
        Refresh();
        SetSelected(false);
        PivotOffset = Size / 2;

        _indexLb.Text = (index + 1).ToString();
        _bgBtn.ButtonGroup = btnGroup;
    }

    public void Refresh()
    {
        var itemInstance = Inventory?.SlotList[Index].ItemInstance;
        if (itemInstance == null)
        {
            _iconTr.Texture = null;
            _nameLb.Text = "";
            _countLb.Text = "";
            _durTpb.Visible = false;
            return;
        }

        _itemInstance = itemInstance;
        ItemData itemData = ItemDataManager.Instance.GetData(_itemInstance.Type);
        _iconTr.Texture = GD.Load<Texture2D>(itemData.IconPath);
        _countLb.Text = itemData.MaxCount == 1 ? "" : $"{_itemInstance.Count}";
        _nameLb.Text = $"{itemData.Name}";
        if (itemData.MaxDur == -1)
        {
            _durTpb.Visible = false;
        }
        else
        {
            _durTpb.Visible = true;
            _durTpb.MaxValue = itemData.MaxDur;
            _durTpb.Value = itemInstance.CurDur;
        }
    }

    public void SetSelected(bool isSelected)
    {
        _bgBtn.ButtonPressed = isSelected;
        //if (_isSelected == isSelected)
        //    return;
        //_isSelected = isSelected;
        //RefreshStyle();
    }

    //private void RefreshStyle()
    //{
    //    if (_isSelected)
    //    {
    //        Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
    //        animTween.TweenProperty(this, "scale", new Vector2(0.9f, 0.9f), 0.1f);
    //        animTween.TweenProperty(this, "scale", Vector2.One, 0.1f);
    //        AddThemeStyleboxOverride("panel", _selectedStyle);
    //    }
    //    else if (_isHover)
    //    {
    //        //Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
    //        //animTween.TweenProperty(this, "scale", new Vector2(0.9f, 0.9f), 0.1f);
    //        //animTween.TweenProperty(this, "scale", Vector2.One, 0.1f);
    //        AddThemeStyleboxOverride("panel", _hoverStyle);
    //    }
    //    else
    //    {
    //        AddThemeStyleboxOverride("panel", _normalStyle);
    //    }
    //}
}
