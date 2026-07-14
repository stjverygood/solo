using Solo.Scripts.Global.Interfaces;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Core
{
    public class ComponentHost
    {
        private readonly IEntity _owner;
        private readonly Dictionary<Type, Component> _componentMap = new();

        public ComponentHost(IEntity owner)
        {
            _owner = owner;
        }

        public void Add(Component component)
        {
            _componentMap[component.GetType()] = component;
            component.Init(_owner);
        }

        public T Get<T>() where T : Component
        {
            if (_componentMap.TryGetValue(typeof(T), out Component? component) == false)
            {
                throw new InvalidOperationException($"缺少组件 {typeof(T).Name}");
            }
            return (T)component;
        }

        public bool TryGet<T>(out T component) where T : Component
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
