using Solo.Scripts.Entities.Core;
using System;

namespace Solo.Scripts.Entities.Components
{
    public class DefComponent : Component
    {
        public float Value { get; private set; }

        public event Action<float>? OnValueChanged;

        //初始化, 升级, 穿戴装备时都要刷新
        public void Refresh(float value)
        {
            Value = value;
            OnValueChanged?.Invoke(Value);
        }
    }
}
