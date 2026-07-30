using Solo.Scripts.Entities.Core;
using System.Collections.Generic;

namespace Solo.Scripts.Global.Interfaces
{
    public interface IEntity
    {
        void Init(EntityType type, List<ComponentSaveData> componentSaveDataList);
        EntityCore Core { get; }
    }
}
