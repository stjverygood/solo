using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.RealmSystem;
using System;
namespace Solo.Scripts.Entities.Players.UI
{
    public partial class AttributeView : PanelContainer
    {

        [Export] private Label _realmLb = null!;
        [Export] private Label _atkLb = null!;
        [Export] private Label _defLb = null!;
        [Export] private Label _hpLb = null!;
        [Export] private Label _qiLb = null!;
        [Export] private Label _expLb = null!;

        [Export] private Button _upgradeBtn = null!;

        public event Action OnUpgraded;

        public void Init()
        {
            _upgradeBtn.Pressed += () =>
            {
                OnUpgraded?.Invoke();
            };
        }

        public void RefreshRealm(RealmType value)
        {
            _realmLb.Text = $"当前境界 : {RealmDataManager.Instance.GetData(value).Name}";
        }

        public void RefreshHp(float curValue, float maxValue)
        {
            _hpLb.Text = $"{curValue:f0} / {maxValue:f0}";
        }

        public void RefreshQi(float curValue, float maxValue)
        {
            _qiLb.Text = $"{curValue:f0} / {maxValue:f0}";
        }

        public void RefreshExp(float curValue, float maxValue)
        {
            _expLb.Text = $"{curValue:f0} / {maxValue:f0}";
            _upgradeBtn.Disabled = curValue == maxValue ? false : true;
        }

        public void RefreshAtk(float value)
        {
            _atkLb.Text = $"{value:f0}";
        }

        public void RefreshDef(float value)
        {
            _defLb.Text = $"{value:f0}";
        }
    }
}
