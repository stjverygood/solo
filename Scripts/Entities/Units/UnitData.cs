using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Units
{
    public class UnitData
    {
        public TargetType TargetType;
        public string UnitName = "";
        public string TexturePath = "";
        public float MaxHp = -1;
        public float MoveSpeed = -1;
        public float IdleDuration = -1;
        public float PatrolDuration = -1;
        public float PatrolRadius = -1;
        public float ViewCollisionShapeRadius = -1;
        public float AtkRange = -1;
        public float CdDuration = -1;
        //public float NaviAgentRadius = -1;
        public float Size = -1;

        public List<TargetType> AtkTargetTypeList;
        public List<TargetType> FearTargetTypeList;
        public List<(ItemType, int, int)> DropItemList;//掉落物类型, 最小掉落数量, 最大掉落数量
    }
}
