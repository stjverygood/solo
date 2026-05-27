using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.System.BuildingSystem
{
    public partial class Building : Node2D
    {
        public BuildingData Data;

        public override void _Ready()
        {
            Data = BuildingDataManager.Instance.GetBuildingData(BuildingType.Stone);
        }

        public override void _Process(double delta)
        {
        }
    }
}