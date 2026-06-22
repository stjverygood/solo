using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.CraftSystem;
namespace Solo.Scripts.Entities.Players
{
    public partial class SelfView : Control
    {
        [Export] public Button EquipmentBtn;
        [Export] public Button BasicCraftBtn;
        [Export] public Button OtherCraftBtn;
        //[Export] public InventoryView ArmorInventoryView;
        [Export] public ArmorView ArmorView;
        [Export] public CraftView BasicCraftView;
        [Export] public CraftView OtherCraftView;

        public override void _Ready()
        {
            EquipmentBtn.Toggled += EquipmentBtn_Toggled;
            BasicCraftBtn.Toggled += BasicCraftBtn_Toggled;
            OtherCraftBtn.Toggled += OtherCraftBtn_Toggled;
            ArmorView.Visible = true;
            BasicCraftView.Visible = false;
            OtherCraftView.Visible = false;
            OtherCraftBtn.Visible = false;
        }

        public void ChangeView(SelfViewTarget type, CraftViewType? craftViewType = null)
        {
            // 先将所有按钮置为未选中（防止状态冲突）
            EquipmentBtn.ButtonPressed = false;
            BasicCraftBtn.ButtonPressed = false;
            OtherCraftBtn.ButtonPressed = false;

            switch (type)
            {
                case SelfViewTarget.EquipmentView:
                    // 直接修改属性，会自动触发 EquipmentBtn_Toggled 回调
                    OtherCraftBtn.Visible = false;
                    EquipmentBtn.ButtonPressed = true;
                    break;
                case SelfViewTarget.BasicCraftView:
                    OtherCraftBtn.Visible = false;
                    BasicCraftBtn.ButtonPressed = true;
                    break;
                case SelfViewTarget.OtherCraftView:
                    OtherCraftBtn.Visible = true;
                    OtherCraftBtn.ButtonPressed = true;
                    OtherCraftView.RefreshType((CraftViewType)craftViewType);
                    break;
            }
        }

        private void EquipmentBtn_Toggled(bool toggledOn)
        {
            if (toggledOn)
            {
                ArmorView.Visible = true;
                BasicCraftView.Visible = false;
                OtherCraftView.Visible = false;
            }
        }

        private void BasicCraftBtn_Toggled(bool toggledOn)
        {
            if (toggledOn)
            {
                ArmorView.Visible = false;
                BasicCraftView.Visible = true;
                OtherCraftView.Visible = false;
            }
        }

        private void OtherCraftBtn_Toggled(bool toggledOn)
        {
            ArmorView.Visible = false;
            BasicCraftView.Visible = false;
            OtherCraftView.Visible = true;
        }
    }
}
