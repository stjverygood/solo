using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Entities.Core
{
    public class Component
    {
        protected IEntity _owner = null!;
        public void Init(IEntity owner)
        {
            _owner = owner;
        }
        public IEntity GetOwner()
        {
            return _owner;
        }
    }
}
