using Solo.Scripts.Entities;

namespace Solo.Scripts.Global.Interfaces
{
    public interface IEntity
    {
        T GetComponent<T>() where T : Component;
        bool TryGetComponent<T>(out T component) where T : Component;
    }
}
