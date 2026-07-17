using System;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Core
{
    public class EntityCore
    {
        private readonly Dictionary<Type, Component> _componentMap = new();

        public EntityCore() { }

        public void AddComponent(Component component)
        {
            _componentMap[component.GetType()] = component;
        }

        public T GetComponent<T>() where T : Component
        {
            if (_componentMap.TryGetValue(typeof(T), out Component? component) == false)
            {
                throw new InvalidOperationException($"缺少组件 {typeof(T).Name}");
            }
            return (T)component;
        }

        public bool TryGetComponent<T>(out T component) where T : Component
        {
            if (_componentMap.TryGetValue(typeof(T), out Component? raw))
            {
                component = (T)raw;
                return true;
            }
            component = null!;
            return false;
        }
    }
}
