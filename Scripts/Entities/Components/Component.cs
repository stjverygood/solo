using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Entities.Components
{
    public class Component
    {
        private IEntity? _owner;
        public void Init(IEntity owner)
        {
            _owner = owner;
        }
        public IEntity? GetOwner()
        {
            return _owner;
        }
    }
}
