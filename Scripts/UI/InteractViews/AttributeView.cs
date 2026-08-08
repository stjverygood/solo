using Godot;
using Solo.Scripts.Components.AtkComponents;
using Solo.Scripts.Components.DefComponents;
using Solo.Scripts.Components.ExpComponents;
using Solo.Scripts.Components.HpComponents;
using Solo.Scripts.Components.QiComponents;
using Solo.Scripts.Components.RealmComponents;
using Solo.Scripts.Global;

namespace Solo.Scripts.UI.InteractViews
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

        public void Init()
        {
            RealmComponent realmComponent = GameManager.Instance.Player.Core.GetComponent<RealmComponent>();
            HpComponent hpComponent = GameManager.Instance.Player.Core.GetComponent<HpComponent>();
            QiComponent qiComponent = GameManager.Instance.Player.Core.GetComponent<QiComponent>();
            ExpComponent expComponent = GameManager.Instance.Player.Core.GetComponent<ExpComponent>();
            AtkComponent atkComponent = GameManager.Instance.Player.Core.GetComponent<AtkComponent>();
            DefComponent defComponent = GameManager.Instance.Player.Core.GetComponent<DefComponent>();

            RefreshRealm(realmComponent.GetName());
            realmComponent.OnCurRealmChanged += () => realmComponent.GetName();

            RefreshHp(hpComponent.CurHp, hpComponent.GetMaxHp());
            hpComponent.OnCurHpChanged += () => RefreshHp(hpComponent.CurHp, hpComponent.GetMaxHp());
            hpComponent.OnMaxHpChanged += () => RefreshHp(hpComponent.CurHp, hpComponent.GetMaxHp());

            RefreshQi(qiComponent.CurQi, qiComponent.GetMaxQi());
            qiComponent.OnCurQiChanged += () => RefreshQi(qiComponent.CurQi, qiComponent.GetMaxQi());
            qiComponent.OnMaxQiChanged += () => RefreshQi(qiComponent.CurQi, qiComponent.GetMaxQi());

            RefreshExp(expComponent.CurExp, expComponent.GetMaxExp());
            expComponent.OnCurExpChanged += () => RefreshExp(expComponent.CurExp, expComponent.GetMaxExp());
            expComponent.OnMaxExpChanged += () => RefreshExp(expComponent.CurExp, expComponent.GetMaxExp());

            RefreshAtk(atkComponent.GetAtk());
            atkComponent.OnAtkChanged += () => RefreshAtk(atkComponent.GetAtk());

            RefreshDef(defComponent.GetDef());
            defComponent.OnDefChanged += () => RefreshDef(defComponent.GetDef());

            _upgradeBtn.Pressed += () =>
            {
                realmComponent.Upgrade();
                if (realmComponent.CurRealm == RealmType.HuaShen)
                    _upgradeBtn.Disabled = true;
                hpComponent.Reset();
                qiComponent.Reset();
                expComponent.Reset();
                //var realmData = RealmDataManager.Instance.GetData(playerCore.GetComponent<RealmComponent>().Value);
                //playerCore.GetComponent<HpComponent>().SetValue(realmData.MaxHp + playerCore.GetComponent<InventoryComponent>().GetMaxHpBonus(), realmData.MaxHp + playerCore.GetComponent<InventoryComponent>().GetMaxHpBonus());
                //playerCore.GetComponent<QiComponent>().Refresh(realmData.MaxQi + playerCore.GetComponent<InventoryComponent>().GetMaxQiBonus(), realmData.MaxQi + playerCore.GetComponent<InventoryComponent>().GetMaxQiBonus());
                //playerCore.GetComponent<ExpComponent>().Refresh(0, realmData.MaxExp);
                //playerCore.GetComponent<AtkComponent>().Refresh(realmData.Atk + playerCore.GetComponent<InventoryComponent>().GetAtkBonus());
                //playerCore.GetComponent<DefComponent>().Refresh(realmData.Def + playerCore.GetComponent<InventoryComponent>().GetDefBonus());
            };
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
            _upgradeBtn.Disabled = curValue == maxValue ? false : true;
        }

        private void RefreshAtk(float value)
        {
            _atkLb.Text = $"{value:f0}";
        }

        private void RefreshDef(float value)
        {
            _defLb.Text = $"{value:f0}";
        }
    }
}
