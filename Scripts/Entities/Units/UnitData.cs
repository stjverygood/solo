using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Units
{
    public class UnitData
    {
        public string UnitName = "";
        public float MaxHp = -1;
        public float MoveSpeed = -1;
        public float PatrolRadius = -1;
        public List<UnitType> HostileUnitTypeList;
        public List<UnitType> FearUnitTypeList;
    }
}
