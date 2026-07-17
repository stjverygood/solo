using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using System;

namespace Solo.Scripts.Entities.Components
{
    public class RealmComponent : Component
    {
        public RealmType Value { get; private set; }

        public event Action<RealmType>? OnValueChanged;

        //初始化, 升级, 穿戴装备时都要刷新
        public void Refresh(RealmType value)
        {
            Value = value;
            OnValueChanged?.Invoke(Value);
        }

        public void Upgrade()
        {
            if (Value == RealmType.HuaShen)
                return;
            Value++;
            OnValueChanged?.Invoke(Value);
        }
    }
}
