using Solo.Scripts.Entities.Components.RealmComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Entities.Components.ExpComponents
{
    public class ExpComponent : Component
    {
        public float CurExp { get; private set; }

        public event Action? OnCurExpChanged;
        public event Action? OnMaxExpChanged;

        public ExpComponent(IEntity owner) : base(owner) { }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            ExpComponentSaveData? saveData = (ExpComponentSaveData?)componentSaveData;
            if (saveData != null)
            {
                CurExp = saveData.CurExp;
            }
            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent? realmComp))
            {
                realmComp.OnCurRealmChanged += () =>
                {
                    OnMaxExpChanged?.Invoke();
                };
            }
        }

        public override ComponentSaveData? GetSaveData()
        {
            return new ExpComponentSaveData()
            {
                ComponentName = nameof(ExpComponent),
                CurExp = CurExp,
            };
        }

        public void Reset()
        {
            CurExp = 0;
            OnCurExpChanged?.Invoke();
        }

        public float GetMaxExp()
        {
            float maxExp = 0;
            if (_owner.Core.TryGetComponent<RealmComponent>(out RealmComponent realmComp))
            {
                maxExp += realmComp.GetMaxExp();
            }
            return maxExp;
        }

        public void Gain(float value)
        {
            CurExp = Math.Clamp(CurExp + value, 0, GetMaxExp());
            OnCurExpChanged?.Invoke();
        }
        public void Consume(float value)
        {
            CurExp = Math.Clamp(CurExp - value, 0, GetMaxExp());
            OnCurExpChanged?.Invoke();
        }


    }
}
