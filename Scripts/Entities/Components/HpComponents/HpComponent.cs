using Solo.Scripts.Entities.Components.InventoryComponents;
using Solo.Scripts.Entities.Components.RealmComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Entities.Components.HpComponents
{
    public class HpComponent : Component
    {
        public float CurHp { get; private set; }
        public event Action? OnCurHpChanged;
        public event Action? OnMaxHpChanged;
        private HpComponentData _data = null!;

        public HpComponent(IEntity owner) : base(owner) { }


        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            if (componentData == null)
                throw new Exception("component null");
            _data = (HpComponentData)componentData;
            HpComponentSaveData? saveData = (HpComponentSaveData?)componentSaveData;

            if (saveData == null)
            {
                CurHp = GetMaxHp();
            }
            else
            {
                CurHp = saveData.CurHp;
            }

            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                realmComp.OnCurRealmChanged += () =>
                {
                    CurHp = Math.Clamp(CurHp, 0, GetMaxHp());
                    OnCurHpChanged?.Invoke();
                    OnMaxHpChanged?.Invoke();
                };
            }
            if (_owner.Core.TryGetComponent<InventoryComponent>(out InventoryComponent? inventoryComp))
            {
                inventoryComp.EquipmentInventory.SlotChanged += (i) =>
                {
                    CurHp = Math.Clamp(CurHp, 0, GetMaxHp());
                    OnCurHpChanged?.Invoke();
                    OnMaxHpChanged?.Invoke();
                };
            }
        }

        public override ComponentSaveData? GetSaveData()
        {
            return new HpComponentSaveData()
            {
                ComponentName = nameof(HpComponent),
                CurHp = CurHp
            };
        }


        public void Reset()
        {
            CurHp = GetMaxHp();
            OnCurHpChanged?.Invoke();
        }

        public float GetMaxHp()
        {
            float maxHp = _data.BaseMaxHp;
            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                maxHp += realmComp.GetMaxHpBonus();
            }
            if (_owner.Core.TryGetComponent<InventoryComponent>(out InventoryComponent? inventoryComp))
            {
                maxHp += inventoryComp.GetMaxHpBonus();
            }
            return maxHp;
        }

        public void Gain(float value)
        {
            CurHp = Math.Clamp(CurHp + value, 0, GetMaxHp());
            OnCurHpChanged?.Invoke();
        }
        public void Consume(float value)
        {
            CurHp = Math.Clamp(CurHp - value, 0, GetMaxHp());
            OnCurHpChanged?.Invoke();
        }
    }
}
