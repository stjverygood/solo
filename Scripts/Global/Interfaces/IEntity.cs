namespace Solo.Scripts.Global.Interfaces
{
    public interface IEntity
    {
        T? GetComponent<T>();
    }
}
