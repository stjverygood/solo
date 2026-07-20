using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;
using System;

namespace Solo.Scripts.System.CraftSystem
{
    public partial class CraftItemView : Control
    {
        public ItemType Type;
        public int Index;

        [Export] private Button _toggleBtn = null!;
        [Export] private TextureRect _iconTr = null!;


        public Action<CraftItemView> Toggled;

        public void Init(ItemType type, int index, ButtonGroup btnGroup)
        {
            Type = type;
            Index = index;
            _iconTr.Texture = GD.Load<Texture2D>(ItemDataManager.Instance.GetData(Type).IconPath);
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