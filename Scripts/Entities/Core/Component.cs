using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Entities.Core
{
    public class Component
    {
        protected IEntity _owner;
        public Component(IEntity owner)
        {
            _owner = owner;
        }
        public IEntity GetOwner()
        {
            return _owner;
        }
    }
}
