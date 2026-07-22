using Solo.Scripts.Entities.Core;

namespace Solo.Scripts.Entities.MeleeEnemies
{
    public class MeleeEnemyData : EntityData
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
    }
}
