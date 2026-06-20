using Solo.Scripts.Global;
namespace Solo.Scripts.System.BuildingSystem.Buildings
{
    public partial class Tree : Building
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
            return damage;
        }

        protected override void Die()
        {
            base.Die();
        }
    }
}
