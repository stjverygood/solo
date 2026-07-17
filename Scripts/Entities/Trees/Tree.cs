using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
namespace Solo.Scripts.Entities.Trees
{
    public partial class Tree : StaticBody2D, IEntity, ISaveable
    {
        [Export] private Label _hpLabel = null!;
        private TreeData _data = null!;


        public EntityCore Core { get; private set; } = new();

        public void Init(EntityData data, Vector2 worldPos)
        {
            _data = (TreeData)data;
            GlobalPosition = worldPos;
            HpComponent hpComponent = new HpComponent(this);
            hpComponent.OnValueChanged += (curHp, maxHp) =>
            {
                _hpLabel.Text = $"{curHp:f0} / {maxHp:f0}";
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

        public void Load(EntityData data, TreeSaveData saveData)
        {
            _data = (TreeData)data;
            GlobalPosition = new Vector2(saveData.WorldX, saveData.WorldY);
            HpComponent hpComponent = new HpComponent(this);
            hpComponent.OnValueChanged += (curHp, maxHp) =>
            {
                _hpLabel.Text = $"{curHp:f0} / {maxHp:f0}";
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
            return new TreeSaveData()
            {
                Type = EntityType.Tree,
                WorldX = GlobalPosition.X,
                WorldY = GlobalPosition.Y,
                CurHp = Core.GetComponent<HpComponent>().CurValue
            };
        }


    }
}
