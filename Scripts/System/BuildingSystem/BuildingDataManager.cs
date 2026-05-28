using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.BuildingSystem
{
    public class BuildingDataManager
    {
        private static BuildingDataManager _instance;
        public static BuildingDataManager Instance => _instance ??= new BuildingDataManager();
        private Dictionary<BuildingType, BuildingData> _BuildingDataMap = new Dictionary<BuildingType, BuildingData>();

        private BuildingDataManager()
        {
            _BuildingDataMap.Add(BuildingType.Tree, new BuildingData() { Type = BuildingType.Tree, Width = 1, Height = 2 });
            _BuildingDataMap.Add(BuildingType.Stone, new BuildingData() { Type = BuildingType.Stone, Width = 2, Height = 2 });
            _BuildingDataMap.Add(BuildingType.Core, new BuildingData() { Type = BuildingType.Core, Width = 4, Height = 4 });
        }

        public BuildingData GetBuildingData(BuildingType type)
        {
            return _BuildingDataMap[type];
        }
    }
}
