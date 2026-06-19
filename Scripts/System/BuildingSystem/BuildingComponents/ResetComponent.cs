using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
namespace Solo.Scripts.System.BuildingSystem.BuildingComponents
{

    public partial class ResetComponent : Node2D, IBuildingComponent, IInteractable
    {
        public void Init()
        {
            GD.Print("ResetComponent, Init");
        }

        public void Interact()
        {
            GameManager.Instance.Player.StartPoint = GlobalPosition;
            GD.Print("ResetComponent, 重置出生点成功~~");
        }
    }
}
