using Godot;
using Solo.Scripts.System.CraftSystem;
namespace Solo.Scripts.Entities.Players
{
    public partial class SelfView : Control
    {
        [Export] public Button EquipmentBtn;
        [Export] public Button BasicCraftBtn;
        [Export] public Control EquipmentView;
        [Export] public CraftView BasicCraftView;

        public override void _Ready()
        {
            EquipmentBtn.Toggled += EquipmentBtn_Toggled;
            BasicCraftBtn.Toggled += BasicCraftBtn_Toggled;
            EquipmentView.Visible = true;
            BasicCraftView.Visible = false;
        }

        private void EquipmentBtn_Toggled(bool toggledOn)
        {
            if (toggledOn)
            {
                EquipmentView.Visible = true;
                BasicCraftView.Visible = false;
            }
        }

        private void BasicCraftBtn_Toggled(bool toggledOn)
        {
            if (toggledOn)
            {
                EquipmentView.Visible = false;
                BasicCraftView.Visible = true;
            }
        }
    }
}
