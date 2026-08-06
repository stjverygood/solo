using Godot;
using Solo.Scripts.Entities.Components.HpComponents;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
namespace Solo.Scripts.Entities.Resources
{
    public partial class Resource : StaticBody2D, IEntity
    {
        public EntityCore Core { get; private set; } = null!;
        //public EntityType Type;
        //private ResourceData _data = null!;

        public void Init(EntityType type, EntitySaveData? entitySaveData)
        {
            Core = new EntityCore(type);
            Core.InitComponent(this, entitySaveData);

            Core.GetComponent<HpComponent>().OnCurHpChanged += () =>
            {
                if (Core.GetComponent<HpComponent>().CurHp <= 0)
                {
                    QueueFree();
                }
            };
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
