using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Entities.Grasses
{
    public partial class Grass : StaticBody2D, IEntity, ISaveable
    {
        public static EntityData DefaultData => new GrassData { MaxHp = 100 };

        private GrassData _data;
        public EntityCore Core { get; private set; } = new();
        public void Init(Vector2 worldPos, EntitySaveData? saveData = null)
        {
            _data = (GrassData)EntityDataManager.Instance.GetData(EntityType.Grass);
            GlobalPosition = saveData != null ? new Vector2(saveData.WorldX, saveData.WorldY) : worldPos;

            float hpValue = saveData is GrassSaveData gs ? gs.CurHp : _data.MaxHp;

            HpComponent hpComponent = new HpComponent(this);
            hpComponent.OnValueChanged += (cur, max) =>
            {
                if (cur == 0) QueueFree();
            };
            hpComponent.SetValue(hpValue, _data.MaxHp);
            Core.AddComponent(hpComponent);

            DefComponent defComponent = new DefComponent(this);
            defComponent.Refresh(100);
            Core.AddComponent(defComponent);
        }

        public override void _Ready()
        {
        }

        public override void _Process(double delta)
        {
        }

        public EntitySaveData GetSaveData()
        {
            return new GrassSaveData()
            {
                Type = EntityType.Grass,
                WorldX = GlobalPosition.X,
                WorldY = GlobalPosition.Y,
                CurHp = Core.GetComponent<HpComponent>().CurValue
            };
        }
    }
}