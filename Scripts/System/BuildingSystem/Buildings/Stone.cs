using Solo.Scripts.Global;
namespace Solo.Scripts.System.BuildingSystem.Buildings
{
    public partial class Stone : Building
    {
        //// Called when the node enters the scene tree for the first time.
        //public override void _Ready()
        //{
        //}

        //// Called every frame. 'delta' is the elapsed time since the previous frame.
        //public override void _Process(double delta)
        //{
        //}
        protected override float HandleDamage(float damage, ItemType? itemType)
        {
            switch (itemType)
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
            return damage;
        }

        protected override void Die()
        {
            base.Die();
        }
    }
}
