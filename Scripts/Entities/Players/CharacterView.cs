using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.CraftSystem;
namespace Solo.Scripts.Entities.Players.UI
{
    public partial class CharacterView : Control
    {
        [Export] private Button _attributeViewBtn = null!;
        [Export] private Button _fastCraftViewBtn = null!;
        [Export] public AttributeView AttributeView = null!;
        [Export] public CraftView FastCraftView = null!;

        private int _curIndex = 0;

        public void Init()
        {
            ButtonGroup btnGroup = new ButtonGroup();
            _attributeViewBtn.ButtonGroup = btnGroup;
            _fastCraftViewBtn.ButtonGroup = btnGroup;
            btnGroup.Pressed += (BaseButton currentBtn) =>
            {
                AttributeView.Visible = (currentBtn == _attributeViewBtn);
                FastCraftView.Visible = (currentBtn == _fastCraftViewBtn);
            };

            AttributeView.Init();
            FastCraftView.Init(CraftType.Fast);
            _attributeViewBtn.ButtonPressed = true;
        }


    }
}
