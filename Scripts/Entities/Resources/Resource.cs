using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
namespace Solo.Scripts.Entities.Resources
{
    public partial class Resource : StaticBody2D, IEntity, ISaveable
    {
        public EntityCore Core { get; private set; } = new();
        public EntityType Type;
        private ResourceData _data = null!;


        public void Init(EntityType type, Vector2 worldPos, EntitySaveData? entitySaveData = null)
        {
            Type = type;
            _data = (ResourceData)EntityDataManager.Instance.GetData(type);
            ResourceSaveData? saveData = (ResourceSaveData?)entitySaveData;

            if (saveData == null)
                GlobalPosition = worldPos;
            else
                GlobalPosition = new Vector2(saveData.WorldX, saveData.WorldY);

            HpComponent hpComponent = new HpComponent(this);
            if (saveData == null)
                hpComponent.SetValue(_data.MaxHp, _data.MaxHp);
            else
                hpComponent.SetValue(saveData.CurHp, _data.MaxHp);
            Core.AddComponent(hpComponent);
        }

        public EntitySaveData GetSaveData()
        {
            return new ResourceSaveData()
            {
                Type = Type,
                WorldX = GlobalPosition.X,
                WorldY = GlobalPosition.Y,
                CurHp = Core.GetComponent<HpComponent>().CurValue
            };
        }
    }
}
