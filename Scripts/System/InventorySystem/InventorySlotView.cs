using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.System.InventorySystem
{
    public partial class InventorySlotView : PanelContainer
    {
        [Export] private TextureRect _iconTr = null!;
        [Export] private Label _nameLb = null!;
        [Export] private Label _countLb = null!;
        [Export] private TextureProgressBar _durTpb = null!;
        private ItemInstance _itemInstance;
        public int Index;
        public Inventory Inventory;

        //style : 
        private bool _isSelected;
        private bool _isHover;
        [Export] private StyleBoxTexture _normalStyle = null!;
        [Export] private StyleBoxTexture _hoverStyle = null!;
        [Export] private StyleBoxTexture _selectedStyle = null!;

        public void Init(Inventory inventory, int index)
        {
            Inventory = inventory;
            Index = index;
            _itemInstance = null;
            SetData(null);
            SetSelected(false);
            PivotOffset = Size / 2;

            MouseEntered += () =>
            {
                _isHover = true;
                RefreshStyle();
            };
            MouseExited += () =>
            {
                _isHover = false;
                RefreshStyle();
            };
        }

        public void SetData(ItemInstance itemInstance)
        {
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


        public override Variant _GetDragData(Vector2 atPosition)
        {
            GD.Print("_GetDragData atPosition : " + atPosition);
            if (_itemInstance == null)
                return new Variant();

            TextureRect previewIconTr = new TextureRect();
            previewIconTr.Texture = _iconTr.Texture;
            previewIconTr.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            previewIconTr.StretchMode = TextureRect.StretchModeEnum.Scale;
            previewIconTr.Size = _iconTr.Size;

            Control previewContainer = new Control();
            previewContainer.ZIndex = 100;
            previewContainer.AddChild(previewIconTr);
            previewIconTr.Position = -atPosition;

            SetDragPreview(previewContainer);
            return this;
        }

        public override bool _CanDropData(Vector2 atPosition, Variant data)
        {
            return true;
        }

        public override void _DropData(Vector2 atPosition, Variant data)
        {
            InventorySlotView sourceSlotView = data.As<InventorySlotView>();
            GameManager.Instance.Player.InventoryManager.SwapItem(sourceSlotView.Inventory, sourceSlotView.Index, Inventory, Index);
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            RefreshStyle();
            if (isSelected)
            {
                Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                animTween.TweenProperty(this, "scale", new Vector2(0.8f, 0.8f), 0.1f);
                animTween.TweenProperty(this, "scale", Vector2.One, 0.1f);
                AddThemeStyleboxOverride("panel", _selectedStyle);
            }
            else
            {
                AddThemeStyleboxOverride("panel", _normalStyle);
            }
        }

        private void RefreshStyle()
        {
            if (_isSelected)
            {
                Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                animTween.TweenProperty(this, "scale", new Vector2(0.9f, 0.9f), 0.1f);
                animTween.TweenProperty(this, "scale", Vector2.One, 0.1f);
                AddThemeStyleboxOverride("panel", _selectedStyle);
            }
            else if (_isHover)
            {
                Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
                animTween.TweenProperty(this, "scale", new Vector2(0.9f, 0.9f), 0.03f);
                animTween.TweenProperty(this, "scale", Vector2.One, 0.1f);
                AddThemeStyleboxOverride("panel", _hoverStyle);
            }
            else
            {
                AddThemeStyleboxOverride("panel", _normalStyle);
            }
        }
    }
}
