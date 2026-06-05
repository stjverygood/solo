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
            _BuildingDataMap.Add(BuildingType.Tree, new BuildingData() { Type = BuildingType.Tree, Width = 1, Height = 2, TexturePath = "res://Assets/AtlasTextures/Tree.tres", MaxHp = 100, DropItemList = new List<(ItemType, int, int)>() { (ItemType.Wood, 2, 6), } });
            _BuildingDataMap.Add(BuildingType.Stone, new BuildingData() { Type = BuildingType.Stone, Width = 2, Height = 2, TexturePath = "res://Assets/AtlasTextures/Stone.tres", MaxHp = 100, DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), } });
            _BuildingDataMap.Add(BuildingType.MainBase, new BuildingData() { Type = BuildingType.MainBase, Width = 4, Height = 4, TexturePath = "res://Assets/AtlasTextures/MainBase.tres", MaxHp = 100, DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), } });
        }

        public BuildingData GetBuildingData(BuildingType type)
        {
            return _BuildingDataMap[type];
        }
    }
}
