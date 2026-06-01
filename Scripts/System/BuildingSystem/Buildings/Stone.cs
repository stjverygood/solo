using Solo.Scripts.Global;

namespace Solo.Scripts.System.BuildingSystem
{
    public partial class Stone : BuildingBase
    {
        //public override void _Ready()
        //{
        //}

        //public override void _Process(double delta)
        //{
        //}

        public override void TakeDamage(ItemType? dmgItemType, float damage)
        {
            switch (dmgItemType)
            {
                case ItemType.WoodPickaxe:
                    damage *= 1;
                    break;
                case ItemType.IronPickaxe:
                    damage *= 2;
                    break;
                case ItemType.GoldPickaxe:
                    damage *= 3;
                    break;
                case ItemType.JadePickaxe:
                    damage *= 4;
                    break;
                default:
                    damage *= 0.1f;
                    break;
            }
            base.TakeDamage(dmgItemType, damage);
        }
    }
}
