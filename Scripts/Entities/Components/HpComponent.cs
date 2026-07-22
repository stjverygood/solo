using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Entities.Components
{
    public class HpComponent : Component
    {
        public float MaxValue { get; private set; }
        public float CurValue { get; private set; }

        public event Action<float, float>? OnValueChanged;

        public HpComponent(IEntity owner) : base(owner) { }

        //初始化, 升级, 穿戴装备时都要刷新
        public void SetValue(float curValue, float maxValue)
        {
            CurValue = curValue;
            MaxValue = maxValue;
            OnValueChanged?.Invoke(CurValue, MaxValue);
        }

        public void Gain(float value)
        {
            CurValue = Math.Clamp(CurValue + value, 0, MaxValue);
            OnValueChanged?.Invoke(CurValue, MaxValue);
        }
        public void Consume(float value)
        {
            CurValue = Math.Clamp(CurValue - value, 0, MaxValue);
            OnValueChanged?.Invoke(CurValue, MaxValue);
        }
    }
}
