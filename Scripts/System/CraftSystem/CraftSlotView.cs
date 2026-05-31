using Godot;
using Solo.Scripts.System.ItemSystem;
using System;

namespace Solo.Scripts.System.CraftSystem
{
    public partial class CraftSlotView : Control
    {

        [Export] private Button _toggleBtn;
        [Export] private TextureRect _iconTr;
        public Action<CraftSlotView> Toggled;
        public int Index;
        public CraftItem CraftItem;

        public void Init(int index, ButtonGroup btnGroup, CraftItem craftItem)
        {
            Index = index;
            CraftItem = craftItem;
            _iconTr.Texture = GD.Load<Texture2D>(ItemDataManager.Instance.GetItemData(CraftItem.Type).IconPath);
            _toggleBtn.ButtonGroup = btnGroup;
            _toggleBtn.Toggled += _toggleBtn_Toggled;
        }

        private void _toggleBtn_Toggled(bool toggledOn)
        {
            if (toggledOn)
            {
                Toggled?.Invoke(this);
            }
        }

        public void SetSelected()
        {
            _toggleBtn.ButtonPressed = true;
            Toggled?.Invoke(this);
        }
    }
}