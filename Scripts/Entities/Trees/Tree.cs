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

        private ComponentHost _componentHost = null!;
        public T GetComponent<T>() where T : Component => _componentHost.Get<T>();
        public bool TryGetComponent<T>(out T component) where T : Component => _componentHost.TryGet<T>(out component);

        public void Init(EntityData data, Vector2 worldPos)
        {
            _data = (TreeData)data;
            GlobalPosition = worldPos;
            _componentHost = new ComponentHost(this);
            HpComponent hpComponent = new HpComponent();
            hpComponent.OnValueChanged += (curHp, maxHp) =>
            {
                _hpLabel.Text = $"{curHp:f0} / {maxHp:f0}";
                if (curHp == 0)
                {
                    QueueFree();
                }
            };
            hpComponent.Refresh(_data.MaxHp, _data.MaxHp);
            _componentHost.Add(hpComponent);

            DefComponent defComponent = new DefComponent();
            defComponent.Refresh(100);
            _componentHost.Add(defComponent);
        }

        public void Load(EntityData data, TreeSaveData saveData)
        {
            _data = (TreeData)data;
            GlobalPosition = new Vector2(saveData.WorldX, saveData.WorldY);
            _componentHost = new ComponentHost(this);
            HpComponent hpComponent = new HpComponent();
            hpComponent.OnValueChanged += (curHp, maxHp) =>
            {
                _hpLabel.Text = $"{curHp:f0} / {maxHp:f0}";
                if (curHp == 0)
                {
                    QueueFree();
                }
            };
            hpComponent.Refresh(saveData.CurHp, _data.MaxHp);
            _componentHost.Add(hpComponent);

            DefComponent defComponent = new DefComponent();
            defComponent.Refresh(100);
            _componentHost.Add(defComponent);
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
                CurHp = GetComponent<HpComponent>().CurValue
            };
        }
    }
}
