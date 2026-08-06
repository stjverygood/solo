using Solo.Scripts.Entities.Components.AtkComponents;
using Solo.Scripts.Entities.Components.DefComponents;
using Solo.Scripts.Entities.Components.DropItemComponents;
using Solo.Scripts.Entities.Components.EnemyComponents;
using Solo.Scripts.Entities.Components.ExpComponents;
using Solo.Scripts.Entities.Components.HpComponents;
using Solo.Scripts.Entities.Components.InventoryComponents;
using Solo.Scripts.Entities.Components.PositionComponents;
using Solo.Scripts.Entities.Components.QiComponents;
using Solo.Scripts.Entities.Components.RealmComponents;
using Solo.Scripts.Entities.Components.StartPositionComponents;
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
            foreach (KeyValuePair<Type, ComponentData?> pair in entityData.ComponentDataMap)
            {
                if (pair.Key == typeof(RealmComponent))
                    _componentMap[typeof(RealmComponent)] = new RealmComponent(entity);
                else if (pair.Key == typeof(HpComponent))
                    _componentMap[typeof(HpComponent)] = new HpComponent(entity);
                else if (pair.Key == typeof(QiComponent))
                    _componentMap[typeof(QiComponent)] = new QiComponent(entity);
                else if (pair.Key == typeof(ExpComponent))
                    _componentMap[typeof(ExpComponent)] = new ExpComponent(entity);
                else if (pair.Key == typeof(AtkComponent))
                    _componentMap[typeof(AtkComponent)] = new AtkComponent(entity);
                else if (pair.Key == typeof(DefComponent))
                    _componentMap[typeof(DefComponent)] = new DefComponent(entity);
                else if (pair.Key == typeof(InventoryComponent))
                    _componentMap[typeof(InventoryComponent)] = new InventoryComponent(entity);
                else if (pair.Key == typeof(PositionComponent))
                    _componentMap[typeof(PositionComponent)] = new PositionComponent(entity);
                else if (pair.Key == typeof(StartPositionComponent))
                    _componentMap[typeof(StartPositionComponent)] = new StartPositionComponent(entity);
                else if (pair.Key == typeof(DropItemComponent))
                    _componentMap[typeof(DropItemComponent)] = new DropItemComponent(entity);
                else if (pair.Key == typeof(EnemyComponent))
                    _componentMap[typeof(EnemyComponent)] = new EnemyComponent(entity);

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
                    compSaveDataMap[saveData.TypeName] = saveData;
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
