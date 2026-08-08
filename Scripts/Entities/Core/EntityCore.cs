using Solo.Scripts.Components.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Core
{
    public class EntityCore
    {
        public EntityType Type;
        private readonly Dictionary<Type, Component> _componentMap = new();

        public EntityCore(EntityType type)
        {
            Type = type;
        }

        public void InitComponent(IEntity entity, EntitySaveData? entitySaveData)
        {
            EntityData entityData = EntityDataManager.Instance.GetData(Type);
            // 所有Component构造函数签名统一为(IEntity), 用反射创建, 新增组件无需改这里
            foreach (Type type in entityData.ComponentDataMap.Keys)
            {
                object? instance = Activator.CreateInstance(type, entity);
                if (instance == null)
                    throw new Exception();
                _componentMap[type] = (Component)instance;
            }

            if (entitySaveData == null)
            {
                foreach (KeyValuePair<Type, Component> typeCompPair in _componentMap)
                {
                    entityData.ComponentDataMap.TryGetValue(typeCompPair.Key, out ComponentData? compData);
                    typeCompPair.Value.Init(compData, null);
                }
            }
            else
            {
                Dictionary<string, ComponentSaveData> compSaveDataMap = new Dictionary<string, ComponentSaveData>();
                foreach (ComponentSaveData saveData in entitySaveData.ComponentSaveDataList)
                {
                    compSaveDataMap[saveData.ComponentName] = saveData;
                }
                foreach (KeyValuePair<Type, Component> typeCompPair in _componentMap)
                {
                    entityData.ComponentDataMap.TryGetValue(typeCompPair.Key, out ComponentData? compData);
                    compSaveDataMap.TryGetValue(typeCompPair.Value.GetType().Name, out ComponentSaveData? compSaveData);
                    typeCompPair.Value.Init(compData, compSaveData);
                }
            }
        }
        //public void AddComponent(Component component)
        //{
        //    _componentMap[component.GetType()] = component;
        //}
        public EntitySaveData GetEntitySaveData()
        {
            List<ComponentSaveData> compSaveDataList = new();
            foreach (KeyValuePair<Type, Component> pair in _componentMap)
            {
                ComponentSaveData? compSaveData = pair.Value.GetSaveData();
                if (compSaveData != null)
                    compSaveDataList.Add(compSaveData);
            }
            return new EntitySaveData()
            {
                Type = Type,
                ComponentSaveDataList = compSaveDataList,
            };
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
