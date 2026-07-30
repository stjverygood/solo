using Solo.Scripts.Entities.Components.InventoryComponents;
using Solo.Scripts.Entities.Components.RealmComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Entities.Components.AtkComponents
{
    public class AtkComponent : Component
    {
        public event Action? OnAtkChanged;
        public AtkComponent(IEntity owner) : base(owner) { }
        private AtkComponentData _data = null!;


        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            if (componentData == null)
                throw new Exception("component null");
            _data = (AtkComponentData)componentData;
            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                realmComp.OnCurRealmChanged += () =>
                {
                    OnAtkChanged?.Invoke();
                };
            }
            if (_owner.Core.TryGetComponent<InventoryComponent>(out InventoryComponent? inventoryComp))
            {
                inventoryComp.EquipmentInventory.SlotChanged += (i) =>
                {
                    OnAtkChanged?.Invoke();
                };
            }
        }

        public override ComponentSaveData? GetSaveData()
        {
            return null;
        }

        public float GetAtk()
        {
            float atk = _data.BaseAtk;
            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                atk += realmComp.GetAtkBonus();
            }
            if (_owner.Core.TryGetComponent<InventoryComponent>(out InventoryComponent? inventoryComp))
            {
                atk += inventoryComp.GetAtkBonus();
            }
            return atk;
        }
    }
}
