using Godot;
using Solo.Scripts.Global.Interfaces;
namespace Solo.Scripts.System.BuildingSystem.BuildingComponents
{
    public partial class DominComponent : Node2D, IBuildingComponent
    {
        public void Init()
        {
            GD.Print("DominComponent, Init");
        }
    }
}
