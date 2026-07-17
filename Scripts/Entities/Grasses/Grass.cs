using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Entities.Grasses
{
    public partial class Grass : StaticBody2D, IEntity, ISaveable
    {
        private GrassData _data;
        public EntityCore Core { get; private set; } = new();
        public void Init(EntityData data, Vector2 worldPos)
        {
            _data = (GrassData)data;
            GlobalPosition = worldPos;
            HpComponent hpComponent = new HpComponent(this);
            hpComponent.OnValueChanged += (curHp, maxHp) =>
            {
                if (curHp == 0)
                {
                    QueueFree();
                }
            };
            hpComponent.Refresh(_data.MaxHp, _data.MaxHp);
            Core.AddComponent(hpComponent);

            DefComponent defComponent = new DefComponent(this);
            defComponent.Refresh(100);
            Core.AddComponent(defComponent);
        }

        public void Load(EntityData data, GrassSaveData saveData)
        {
            _data = (GrassData)data;
            GlobalPosition = new Vector2(saveData.WorldX, saveData.WorldY);
            HpComponent hpComponent = new HpComponent(this);
            hpComponent.OnValueChanged += (curHp, maxHp) =>
            {
                if (curHp == 0)
                {
                    QueueFree();
                }
            };
            hpComponent.Refresh(saveData.CurHp, _data.MaxHp);
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