using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.System.BuildingSystem.BuildingSystem
{
    internal partial class Tree : BuildingBase
    {
        public override void Init(BuildingType type, Vector2 snapPos)
        {
            MaxHp = 120;
            base.Init(type, snapPos);
        }

        public override void TakeDamage(ItemType? dmgItemType, float damage)
        {
            switch (dmgItemType)
            {
                case ItemType.WoodAxe:
                    damage *= 2;
                    break;
                case ItemType.IronAxe:
                    damage *= 3;
                    break;
                case ItemType.GoldAxe:
                    damage *= 4;
                    break;
                case ItemType.JadeAxe:
                    damage *= 5;
                    break;
                default:
                    damage *= 1;
                    break;
            }
            base.TakeDamage(dmgItemType, damage);
        }
    }
}
