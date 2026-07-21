using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Global;
using Solo.Scripts.System.RealmSystem;

namespace Solo.Scripts.UI.HUDs
{
    public partial class FastAttributeView : Control
    {
        [Export] private Label _realmLb = null!;
        [Export] private Label _hpLb = null!;
        [Export] private Label _qiLb = null!;
        [Export] private Label _expLb = null!;

        public void Init(RealmComponent realmComponet, HpComponent hpComponent, QiComponent qiComponent, ExpComponent expComponent)
        {
            RefreshRealm(realmComponet.Value);
            realmComponet.OnValueChanged += RefreshRealm;

            RefreshHp(hpComponent.CurValue, hpComponent.MaxValue);
            hpComponent.OnValueChanged += RefreshHp;

            RefreshQi(qiComponent.CurValue, qiComponent.MaxValue);
            qiComponent.OnValueChanged += RefreshQi;

            RefreshExp(expComponent.CurValue, expComponent.MaxValue);
            expComponent.OnValueChanged += RefreshExp;
        }

        private void RefreshRealm(RealmType value)
        {
            _realmLb.Text = $"当前境界 : {RealmDataManager.Instance.GetData(value).Name}";
        }

        private void RefreshHp(float curValue, float maxValue)
        {
            _hpLb.Text = $"{curValue:f0} / {maxValue:f0}";
        }

        private void RefreshQi(float curValue, float maxValue)
        {
            _qiLb.Text = $"{curValue:f0} / {maxValue:f0}";
        }

        private void RefreshExp(float curValue, float maxValue)
        {
            _expLb.Text = $"{curValue:f0} / {maxValue:f0}";
        }
    }

}
