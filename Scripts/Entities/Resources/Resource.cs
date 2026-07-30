using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using System.Collections.Generic;
namespace Solo.Scripts.Entities.Resources
{
    public partial class Resource : StaticBody2D, IEntity
    {
        public EntityCore Core { get; private set; } = null!;
        public EntityType Type;
        private ResourceData _data = null!;

        public void Init(EntityType type, List<ComponentSaveData> componentSaveDataList)
        {
            Core = new EntityCore(type);
            Core.InitComponent(this, componentSaveDataList);
        }

        //public void Init(EntityType type, Vector2 worldPos, EntitySaveData? entitySaveData = null)
        //{
        //    Type = type;
        //    _data = (ResourceData)EntityDataManager.Instance.GetData(type);
        //    ResourceSaveData? saveData = (ResourceSaveData?)entitySaveData;

        //    if (saveData == null)
        //        GlobalPosition = worldPos;
        //    else
        //        GlobalPosition = new Vector2(saveData.WorldX, saveData.WorldY);

        //    HpComponent hpComponent = new HpComponent(this);
        //    if (saveData == null)
        //        hpComponent.SetValue(_data.MaxHp, _data.MaxHp);
        //    else
        //        hpComponent.SetValue(saveData.CurHp, _data.MaxHp);
        //    hpComponent.OnValueChanged += (curValue, maxValue) =>
        //    {
        //        GD.Print($"{curValue}/{maxValue}");
        //        if (hpComponent.CurValue <= 0)
        //        {
        //            GameManager.Instance.World.SpawnDropItem(GlobalPosition, _data.DropInfoList);
        //            QueueFree();
        //        }
        //    };
        //    Core.AddComponent(hpComponent);
        //}

        //public EntitySaveData GetSaveData()
        //{
        //    return new ResourceSaveData()
        //    {
        //        Type = Type,
        //        WorldX = GlobalPosition.X,
        //        WorldY = GlobalPosition.Y,
        //        CurHp = Core.GetComponent<HpComponent>().CurValue
        //    };
        //}


    }
}
