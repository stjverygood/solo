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

        public void Init(Vector2 worldPos, TreeData treeData, EntitySaveData? saveData = null)
        {
            _data = treeData;
            GlobalPosition = saveData != null ? new Vector2(saveData.WorldX, saveData.WorldY) : worldPos;

            float hpValue = saveData is TreeSaveData treeSaveData ? treeSaveData.CurHp : _data.MaxHp;

            HpComponent hpComponent = new HpComponent(this);
            hpComponent.OnValueChanged += (cur, max) =>
            {
                _hpLabel.Text = $"{cur:f0} / {max:f0}";
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
