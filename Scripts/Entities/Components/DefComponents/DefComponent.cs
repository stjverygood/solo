using Solo.Scripts.Entities.Components.InventoryComponents;
using Solo.Scripts.Entities.Components.RealmComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Entities.Components.DefComponents
{
    public class DefComponent : Component
    {
        public event Action? OnDefChanged;

        public DefComponent(IEntity owner) : base(owner) { }

        private DefComponentData _data = null!;

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            if (componentData == null)
                throw new Exception("component null");
            _data = (DefComponentData)componentData;

            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                realmComp.OnCurRealmChanged += () =>
                {
                    OnDefChanged?.Invoke();
                };
            }
            if (_owner.Core.TryGetComponent<InventoryComponent>(out InventoryComponent? inventoryComp))
            {
                inventoryComp.EquipmentInventory.SlotChanged += (i) =>
                {
                    OnDefChanged?.Invoke();
                };
            }
        }

        public override ComponentSaveData? GetSaveData()
        {
            return null;
        }

        public float GetDef()
        {
            float def = _data.BaseDef;
            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                def += realmComp.GetDefBonus();
            }
            if (_owner.Core.TryGetComponent<InventoryComponent>(out InventoryComponent? inventoryComp))
            {
                def += inventoryComp.GetDefBonus();
            }
            return def;
        }
    }
}
