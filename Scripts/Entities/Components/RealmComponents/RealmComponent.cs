using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Entities.Components.RealmComponents
{
    public class RealmComponent : Component
    {
        public RealmType CurRealm { get; private set; }
        public event Action? OnCurRealmChanged;
        private RealmComponentData _data = null!;


        public RealmComponent(IEntity owner) : base(owner) { }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            if (componentData == null)
                throw new Exception();
            _data = (RealmComponentData)componentData;
            RealmComponentSaveData? saveData = (RealmComponentSaveData?)componentSaveData;
            if (saveData == null)
                CurRealm = RealmType.LianQi1;
            else
                CurRealm = saveData.CurRealm;
        }

        public override ComponentSaveData? GetSaveData()
        {
            return new RealmComponentSaveData()
            {
                ComponentName = nameof(RealmComponent),
                CurRealm = CurRealm
            };
        }

        public void Reset()
        {
            CurRealm = RealmType.LianQi1;
            OnCurRealmChanged?.Invoke();
        }

        public string GetName()
        {
            return _data.RealmDataMap[CurRealm].Name;
        }
        public float GetMaxHpBonus()
        {
            return _data.RealmDataMap[CurRealm].MaxHpBonus;
        }
        public float GetMaxQiBonus()
        {
            return _data.RealmDataMap[CurRealm].MaxQiBonus;
        }
        public float GetMaxExp()
        {
            return _data.RealmDataMap[CurRealm].MaxExp;
        }
        public float GetAtkBonus()
        {
            return _data.RealmDataMap[CurRealm].AtkBonus;
        }
        public float GetDefBonus()
        {
            return _data.RealmDataMap[CurRealm].DefBonus;
        }


        ////初始化, 升级, 穿戴装备时都要刷新
        //public void Refresh(RealmType value)
        //{
        //    Value = value;
        //    OnValueChanged?.Invoke(Value);
        //}

        public void Upgrade()
        {
            if (CurRealm == RealmType.HuaShen)
                return;
            CurRealm++;
            OnCurRealmChanged?.Invoke();
        }
    }
}
