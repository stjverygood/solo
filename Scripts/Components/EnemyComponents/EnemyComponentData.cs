using Solo.Scripts.Components.Core;
using Solo.Scripts.Global;

namespace Solo.Scripts.Components.EnemyComponents
{
    public class EnemyComponentData : ComponentData
    {
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
