using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Entities.Components
{
    public class ExpComponent : Component
    {
        public float MaxValue { get; private set; }
        public float CurValue { get; private set; }

        public event Action<float, float>? OnValueChanged;

        public ExpComponent(IEntity owner) : base(owner) { }

        //初始化, 升级, 穿戴装备时都要刷新
        public void Refresh(float curValue, float maxValue)
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
