using Godot;
using Solo.Scripts.Entities.Components.HpComponents;
using Solo.Scripts.Entities.Components.PositionComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.DropItems;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.ItemSystem;
using System;

namespace Solo.Scripts.Entities.Components.DropItemComponents
{
    public class DropItemComponent : Component
    {
        private DropItemComponentData _data = null!;
        public DropItemComponent(IEntity owner) : base(owner) { }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            if (componentData == null)
                throw new Exception();
            _data = (DropItemComponentData)componentData;
            _owner.Core.GetComponent<HpComponent>().OnCurHpChanged += () =>
            {
                if (_owner.Core.GetComponent<HpComponent>().CurHp <= 0)
                {
                    DropItem();
                }
            };
        }
        public override ComponentSaveData? GetSaveData()
        {
            return null;
        }

        private void DropItem()
        {
            foreach (DropItemDropInfo info in _data.DropInfoList)
            {
                for (int i = 0; i < info.Times; i++)
                {
                    if (GD.Randf() > info.Chance)
                        continue;
                    IEntity entity = GameManager.Instance.World.SpawnEntity(EntityType.DropItem);
                    entity.Init(EntityType.DropItem, null);
                    entity.Core.GetComponent<PositionComponent>().SetWorldPosition(_owner.Core.GetComponent<PositionComponent>().GetWorldPosition() + new Vector2(GD.RandRange(-5, 5), GD.RandRange(-5, 5)));
                    ((DropItem)entity).SetItemInstance(new ItemInstance() { Type = info.Type, Count = 1 });
                    ((DropItem)entity).ApplyForce();
                }
            }
        }
    }
}
