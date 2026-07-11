using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.CraftSystem;
namespace Solo.Scripts.Entities.Players.UI
{
    public partial class CharacterView : Control
    {
        [Export] private Button _attributeViewBtn;
        [Export] private Button _fastCraftViewBtn;
        [Export] private AttributeView _attributeView;
        [Export] private CraftView _fastCraftView;

        private int _curIndex = 0;

        public void Init(AttributeManager attributeManager, InventoryManager inventoryManager)
        {
            ButtonGroup btnGroup = new ButtonGroup();
            _attributeViewBtn.ButtonGroup = btnGroup;
            _fastCraftViewBtn.ButtonGroup = btnGroup;
            btnGroup.Pressed += (BaseButton currentBtn) =>
            {
                _attributeView.Visible = (currentBtn == _attributeViewBtn);
                _fastCraftView.Visible = (currentBtn == _fastCraftViewBtn);
            };

            _attributeView.Init(attributeManager, inventoryManager);
            _fastCraftView.Init(CraftType.Fast);
            _attributeViewBtn.ButtonPressed = true;
        }


    }
}
