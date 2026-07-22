using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;

namespace Solo.Scripts.Entities.Enemies
{
    public class EnemyData : EntityData
    {
        //普通近战敌人
        public float MaxHp;
        public float Atk;
        public float Def;
        public float MoveSpeed;
        public float ViewRange;
        public float ViewRangeSq;
        public float AtkRange;
        public float AtkRangeSq;
        public float IdleDuration;
        public float PatrolRange;
        public ProjectileType ProjectileType;
    }
}
