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
            _unitDataMap.Add(UnitType.Wolf, new UnitData() { UnitName = "风狼", MaxHp = 100, MoveSpeed = 50, IdleDuration = 3, PatrolDuration = 1, PatrolRadius = 200, ViewCollisionShapeRadius = 64, AtkRange = 32, CdDuration = 2, NaviAgentRadius = 8, HostileUnitTypeList = new List<UnitType>() { }, FearUnitTypeList = new List<UnitType>() { }, DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), } });
        }

        public UnitData GetUnitData(UnitType type)
        {
            return _unitDataMap[type];
        }
    }
}
