using Solo.Scripts.Components.Core;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.Components.PickableComponents
{
    internal class PickableComponent : Component
    {
        public ItemInstance ItemInstance = null!;

        public PickableComponent(IEntity owner) : base(owner)
        {
        }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            if (componentSaveData is PickableComponentSaveData saveData)
            {
                ItemInstance = saveData.ItemInstance;
            }
        }

        public override ComponentSaveData? GetSaveData()
        {
            return new PickableComponentSaveData()
            {
                ComponentName = nameof(PickableComponent),
                ItemInstance = ItemInstance
            };
        }
    }
}
