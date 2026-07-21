using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Global;
using Solo.Scripts.System.RealmSystem;

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
            var playerCore = GameManager.Instance.Player.Core;

            RefreshRealm(playerCore.GetComponent<RealmComponent>().Value);
            playerCore.GetComponent<RealmComponent>().OnValueChanged += RefreshRealm;

            RefreshHp(playerCore.GetComponent<HpComponent>().CurValue, playerCore.GetComponent<HpComponent>().MaxValue);
            playerCore.GetComponent<HpComponent>().OnValueChanged += RefreshHp;

            RefreshQi(playerCore.GetComponent<QiComponent>().CurValue, playerCore.GetComponent<QiComponent>().MaxValue);
            playerCore.GetComponent<QiComponent>().OnValueChanged += RefreshQi;

            RefreshExp(playerCore.GetComponent<ExpComponent>().CurValue, playerCore.GetComponent<ExpComponent>().MaxValue);
            playerCore.GetComponent<ExpComponent>().OnValueChanged += RefreshExp;

            RefreshAtk(playerCore.GetComponent<AtkComponent>().Value);
            playerCore.GetComponent<AtkComponent>().OnValueChanged += RefreshAtk;

            RefreshDef(playerCore.GetComponent<DefComponent>().Value);
            playerCore.GetComponent<DefComponent>().OnValueChanged += RefreshDef;

            _upgradeBtn.Pressed += () =>
            {
                playerCore.GetComponent<RealmComponent>().Upgrade();
                if (playerCore.GetComponent<RealmComponent>().Value == RealmType.HuaShen)
                    _upgradeBtn.Disabled = true;
                var realmData = RealmDataManager.Instance.GetData(playerCore.GetComponent<RealmComponent>().Value);
                playerCore.GetComponent<HpComponent>().Refresh(realmData.MaxHp + playerCore.GetComponent<InventoryComponent>().GetMaxHpBonus(), realmData.MaxHp + playerCore.GetComponent<InventoryComponent>().GetMaxHpBonus());
                playerCore.GetComponent<QiComponent>().Refresh(realmData.MaxQi + playerCore.GetComponent<InventoryComponent>().GetMaxQiBonus(), realmData.MaxQi + playerCore.GetComponent<InventoryComponent>().GetMaxQiBonus());
                playerCore.GetComponent<ExpComponent>().Refresh(0, realmData.MaxExp);
                playerCore.GetComponent<AtkComponent>().Refresh(realmData.Atk + playerCore.GetComponent<InventoryComponent>().GetAtkBonus());
                playerCore.GetComponent<DefComponent>().Refresh(realmData.Def + playerCore.GetComponent<InventoryComponent>().GetDefBonus());
            };
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
