using Solo.Scripts.Global;
using System.Collections.Generic;

namespace Solo.Scripts.Entities
{
    public class EntityDataManager
    {
        private static EntityDataManager? _instance;
        public static EntityDataManager Instance => _instance ??= new EntityDataManager();
        private Dictionary<EntityType, EntityData> _dataMap = new Dictionary<EntityType, EntityData>();

        private EntityDataManager()
        {

        }

        public T? GetData<T>(EntityType type)
        {
            if (_dataMap[type] is not T data)
            {
                return default(T);
            }
            return data;
        }
    }
}
