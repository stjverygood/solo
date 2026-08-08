using Solo.Scripts.Components.Core;
using Solo.Scripts.Global.Interfaces;
using System;

namespace Solo.Scripts.Components.EnemyComponents
{
    public class EnemyComponent : Component
    {
        public EnemyComponentData Data;
        public EnemyComponent(IEntity owner) : base(owner)
        {
        }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            if (componentData == null)
                throw new Exception("EnemyComponent componentData == null");
            Data = (EnemyComponentData)componentData;
        }
        public override ComponentSaveData? GetSaveData()
        {
            return null;
        }
    }
}
