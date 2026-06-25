using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.System.BuildingSystem
{
    public class BuildingDataManager
    {
        private static BuildingDataManager _instance;
        public static BuildingDataManager Instance => _instance ??= new BuildingDataManager();
        private Dictionary<BuildingType, BuildingData> _buildingDataMap = new Dictionary<BuildingType, BuildingData>();

        private BuildingDataManager()
        {
            _buildingDataMap.Add(BuildingType.Water, new BuildingData()
            {
                Width = 1,
                Height = 1,
            });
            _buildingDataMap.Add(BuildingType.Tree, new BuildingData()
            {
                TargetType = TargetType.Tree,
                Width = 1,
                Height = 1,
                //TextureHeight = 2,
                MaxHp = 100,
                TexturePath = "res://Assets/AtlasTextures/Tree.tres",
                TscnPath = "res://Scenes/System/BuildingSystem/Buildings/Tree.tscn",
                DropItemList = new List<(ItemType, int, int)>() { (ItemType.Wood, 2, 6), },
            });
            _buildingDataMap.Add(BuildingType.Stone, new BuildingData()
            {
                TargetType = TargetType.Stone,
                Width = 2,
                Height = 1,
                //TextureHeight = 2,
                MaxHp = 100,
                TexturePath = "res://Assets/AtlasTextures/Stone.tres",
                TscnPath = "res://Scenes/System/BuildingSystem/Buildings/Stone.tscn",
                DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), },
            });
            _buildingDataMap.Add(BuildingType.MainBase, new BuildingData()
            {
                TargetType = TargetType.MainBase,
                Width = 4,
                Height = 2,
                //TextureHeight = 4,
                MaxHp = 100,
                TexturePath = "res://Assets/AtlasTextures/MainBase.tres",
                TscnPath = "res://Scenes/System/BuildingSystem/Buildings/MainBase.tscn",
                DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), },
            });
            _buildingDataMap.Add(BuildingType.BuildingCraft, new BuildingData()
            {
                TargetType = TargetType.BuildingCraft,
                Width = 4,
                Height = 2,
                //TextureHeight = 3,
                MaxHp = 100,
                TexturePath = "res://Assets/AtlasTextures/BuildingCraft.tres",
                TscnPath = "res://Scenes/System/BuildingSystem/Buildings/BuildingCraft.tscn",
                DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), },
            });
            _buildingDataMap.Add(BuildingType.Flag, new BuildingData()
            {
                TargetType = TargetType.BuildingCraft,
                Width = 1,
                Height = 1,
                MaxHp = 100,
                TexturePath = "res://Assets/AtlasTextures/Flag.tres",
                TscnPath = "res://Scenes/System/BuildingSystem/Buildings/Flag.tscn",
                DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), },
            });
            _buildingDataMap.Add(BuildingType.ToolCraft, new BuildingData()
            {
                TargetType = TargetType.BuildingCraft,
                Width = 2,
                Height = 1,
                MaxHp = 100,
                TexturePath = "res://Assets/AtlasTextures/ToolCraft.tres",
                TscnPath = "res://Scenes/System/BuildingSystem/Buildings/ToolCraft.tscn",
                DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), },
            });
            _buildingDataMap.Add(BuildingType.ArmorCraft, new BuildingData()
            {
                TargetType = TargetType.BuildingCraft,
                Width = 2,
                Height = 1,
                MaxHp = 100,
                TexturePath = "res://Assets/AtlasTextures/ArmorCraft.tres",
                TscnPath = "res://Scenes/System/BuildingSystem/Buildings/ArmorCraft.tscn",
                DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), },
            });
            _buildingDataMap.Add(BuildingType.TreeGrow, new BuildingData()
            {
                TargetType = TargetType.BuildingCraft,
                Width = 1,
                Height = 1,
                MaxHp = 100,
                TexturePath = "res://Assets/AtlasTextures/TreeGrow.tres",
                TscnPath = "res://Scenes/System/BuildingSystem/Buildings/TreeGrow.tscn",
                DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), },
            });
        }

        public BuildingData GetBuildingData(BuildingType type)
        {
            return _buildingDataMap[type];
        }
    }
}
