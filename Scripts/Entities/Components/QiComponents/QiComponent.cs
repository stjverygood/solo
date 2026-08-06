using Solo.Scripts.Entities.Components.InventoryComponents;
using Solo.Scripts.Entities.Components.RealmComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Entities.Components.QiComponents
{
    public class QiComponent : Component
    {
        public float CurQi { get; private set; }

        public event Action? OnCurQiChanged;
        public event Action? OnMaxQiChanged;

        public QiComponent(IEntity owner) : base(owner) { }
        private QiComponentData _data = null!;

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            if (componentData != null)
                _data = (QiComponentData)componentData;
            QiComponentSaveData? saveData = (QiComponentSaveData?)componentSaveData;
            if (saveData == null)
                CurQi = GetMaxQi();
            else
                CurQi = saveData.CurQi;

            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                realmComp.OnCurRealmChanged += () =>
                {
                    CurQi = Math.Clamp(CurQi, 0, GetMaxQi());
                    OnCurQiChanged?.Invoke();
                    OnMaxQiChanged?.Invoke();
                };
            }
            if (_owner.Core.TryGetComponent<InventoryComponent>(out InventoryComponent? inventoryComp))
            {
                inventoryComp.EquipmentInventory.SlotChanged += (i) =>
                {
                    CurQi = Math.Clamp(CurQi, 0, GetMaxQi());
                    OnCurQiChanged?.Invoke();
                    OnMaxQiChanged?.Invoke();
                };
            }
        }

        public override ComponentSaveData? GetSaveData()
        {
            return new QiComponentSaveData()
            {
                ComponentName = nameof(QiComponent),
                CurQi = CurQi
            };
        }

        public void Reset()
        {
            CurQi = GetMaxQi();
            OnCurQiChanged?.Invoke();
        }

        public float GetMaxQi()
        {
            float maxQi = _data.BaseMaxQi;
            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                maxQi += realmComp.GetMaxQiBonus();
            }
            if (_owner.Core.TryGetComponent<InventoryComponent>(out InventoryComponent? inventoryComp))
            {
                maxQi += inventoryComp.GetMaxQiBonus();
            }
            return maxQi;
        }

        public void Gain(float value)
        {
            CurQi = Math.Clamp(CurQi + value, 0, GetMaxQi());
            OnCurQiChanged?.Invoke();
        }
        public void Consume(float value)
        {
            CurQi = Math.Clamp(CurQi - value, 0, GetMaxQi());
            OnCurQiChanged?.Invoke();
        }


    }
}
