using Solo.Scripts.Entities.Core;

namespace Solo.Scripts.Global.Interfaces
{
    public interface IEntity
    {
        EntityCore Core { get; }
        //public ComponentHost;
        //T GetComponent<T>() where T : Component;
        //bool TryGetComponent<T>(out T component) where T : Component;
    }
}
