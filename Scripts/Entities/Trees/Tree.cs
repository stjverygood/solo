using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;

public partial class Tree : StaticBody2D, IEntity
{
    [Export] private Label _hpLabel = null!;
    private ComponentHost _componentHost;
    public T GetComponent<T>() where T : Component => _componentHost.Get<T>();
    public bool TryGetComponent<T>(out T component) where T : Component => _componentHost.TryGet<T>(out component);

    public void Init(Vector2 worldPos)
    {
        _componentHost = new ComponentHost(this);
        GlobalPosition = worldPos;
        HpComponent hpComponent = new HpComponent();
        hpComponent.OnValueChanged += (curHp, maxHp) =>
        {
            _hpLabel.Text = $"{curHp:f0} / {maxHp:f0}";
            if (curHp == 0)
            {
                QueueFree();
            }
        };
        hpComponent.Refresh(100, 100);
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


}
