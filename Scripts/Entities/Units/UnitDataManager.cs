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
            _unitDataMap.Add(UnitType.Wolf, new UnitData() { UnitName = "风狼", MaxHp = 100, MoveSpeed = 500, PatrolRadius = 200, HostileUnitTypeList = new List<UnitType>() { UnitType.Player }, FearUnitTypeList = new List<UnitType>() { }, DropItemList = new List<(ItemType, int, int)>() { (ItemType.Stone, 1, 3), } });

        }

        public UnitData GetUnitData(UnitType type)
        {
            return _unitDataMap[type];
        }
    }
}
