using Godot;
using Solo.Scripts.Entities.Components.ExpComponents;
using Solo.Scripts.Entities.Components.HpComponents;
using Solo.Scripts.Entities.Components.QiComponents;
using Solo.Scripts.Entities.Components.RealmComponents;

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
            RefreshRealm(realmComponet.GetName());
            realmComponet.OnCurRealmChanged += () => realmComponet.GetName();

            RefreshHp(hpComponent.CurHp, hpComponent.GetMaxHp());
            hpComponent.OnCurHpChanged += () => RefreshHp(hpComponent.CurHp, hpComponent.GetMaxHp());
            hpComponent.OnMaxHpChanged += () => RefreshHp(hpComponent.CurHp, hpComponent.GetMaxHp());

            RefreshQi(qiComponent.CurQi, qiComponent.GetMaxQi());
            qiComponent.OnCurQiChanged += () => RefreshQi(qiComponent.CurQi, qiComponent.GetMaxQi());
            qiComponent.OnMaxQiChanged += () => RefreshQi(qiComponent.CurQi, qiComponent.GetMaxQi());

            RefreshExp(expComponent.CurExp, expComponent.GetMaxExp());
            expComponent.OnCurExpChanged += () => RefreshExp(expComponent.CurExp, expComponent.GetMaxExp());
            expComponent.OnMaxExpChanged += () => RefreshExp(expComponent.CurExp, expComponent.GetMaxExp());
        }

        private void RefreshRealm(string realmName)
        {
            _realmLb.Text = $"当前境界 : {realmName}";
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
