using System;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Core
{
    public class EntityDataManager
    {
        private static EntityDataManager? _instance;
        public static EntityDataManager Instance => _instance ??= new EntityDataManager();
        private Dictionary<Type, EntityData> _dataMap = new Dictionary<Type, EntityData>();

        private EntityDataManager()
        {
            _dataMap[typeof(ZombieData)] = new ZombieData()
            {
                MaxHp = 100,
                Atk = 40,
                Def = 30,
                MoveSpeed = 50,
                ViewRange = 100,
                ViewRangeSq = 100 * 100,
                AtkRange = 20,
                AtkRangeSq = 20 * 20,
                IdleDuration = 1,
                PatrolRange = 200,
            };
        }

        public T GetData<T>()
        {
            if (_dataMap[typeof(T)] is not T data)
            {
                throw new Exception("entity data null");
            }
            return data;
        }
    }
}
