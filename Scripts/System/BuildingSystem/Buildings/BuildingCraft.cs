using Godot;

namespace Solo.Scripts.System.BuildingSystem.Buildings
{
    public partial class BuildingCraft : Building
    {
        public override void Interact()
        {
            base.Interact();
            GD.Print("BuildingCraft Interact");

        }
    }
}
