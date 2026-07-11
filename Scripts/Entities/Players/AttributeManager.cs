using Solo.Scripts.Global;
using System;

namespace Solo.Scripts.Entities.Players
{
    public class AttributeManager
    {
        public AttributeManager()
        {

        }

        public event Action CurRealmChanged;
        public event Action CurHpChanged;
        public event Action CurQiChanged;
        public event Action CurExpChanged;

        private RealmType _curRealmType;
        public RealmType CurRealmType
        {
            get => _curRealmType;
            set
            {
                _curRealmType = value;
                CurRealmChanged?.Invoke();
            }
        }

        private float _curHp;
        public float CurHp
        {
            get => _curHp;
            set
            {
                _curHp = value;
                CurHpChanged?.Invoke();
            }
        }

        private float _curQi;
        public float CurQi
        {
            get => _curQi;
            set
            {
                _curQi = value;
                CurQiChanged?.Invoke();
            }
        }

        private float _curExp = 0;
        public float CurExp
        {
            get => _curExp;
            set
            {
                _curExp = value;
                CurExpChanged?.Invoke();
            }
        }

        //public void Upgrade()
        //{
        //    CurRealmType++;
        //}
    }
}