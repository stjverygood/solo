using Solo.Scripts.Entities.Core;

namespace Solo.Scripts.Global.Interfaces
{
    public interface IEntity
    {
        void Init(EntityType type, EntitySaveData? entitySaveData);
        EntityCore Core { get; }
    }
}
