using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Units
{
    public class UnitDataManager
    {
        private static UnitDataManager _instance;
        public static UnitDataManager Instance => _instance ??= new UnitDataManager();
        private Dictionary<UnitType, UnitData> _unitDataMap = new Dictionary<UnitType, UnitData>();

        private UnitDataManager()
        {
            _unitDataMap.Add(UnitType.Wolf, new UnitData() { TargetType = TargetType.Wolf, UnitName = "风狼", TexturePath = "res://Assets/AtlasTextures/Wolf.tres", MaxHp = 200, MoveSpeed = 50, IdleDuration = 5, PatrolDuration = 2, PatrolRadius = 200, ViewCollisionShapeRadius = 64, AtkRange = 32, CdDuration = 1f, Size = 2, AtkTargetTypeList = new List<TargetType>() { TargetType.Player, TargetType.Fox }, FearTargetTypeList = new List<TargetType>() { }, DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), } });
            _unitDataMap.Add(UnitType.Fox, new UnitData() { TargetType = TargetType.Fox, UnitName = "灵狐", TexturePath = "res://Assets/AtlasTextures/Fox.tres", MaxHp = 100, MoveSpeed = 50, IdleDuration = 3, PatrolDuration = 1, PatrolRadius = 200, ViewCollisionShapeRadius = 64, AtkRange = 40, CdDuration = 0.3f, Size = 1, AtkTargetTypeList = new List<TargetType>() { TargetType.Player, TargetType.Tree }, FearTargetTypeList = new List<TargetType>() { TargetType.Wolf }, DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), } });
        }

        public UnitData GetUnitData(UnitType type)
        {
            return _unitDataMap[type];
        }
    }
}
