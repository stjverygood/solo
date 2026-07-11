using Godot;
using Solo.Scripts.System.RealmSystem;
namespace Solo.Scripts.Entities.Players.UI
{
    public partial class AttributeView : PanelContainer
    {

        [Export] private Label _realmLb;
        [Export] private Label _atkLb;
        [Export] private Label _defLb;
        [Export] private Label _hpLb;
        [Export] private Label _qiLb;
        [Export] private Label _expLb;


        public void Init(AttributeManager attributeManager, InventoryManager inventoryManager)
        {
            //todo 绑定equipmentchanged
            inventoryManager.EquipmentInventory.SlotChanged += (index) =>
            {
                _atkLb.Text = $"{RealmDataManager.Instance.GetData(attributeManager.CurRealmType).Atk + inventoryManager.GetAtkBonus():f0}";
                _defLb.Text = $"{RealmDataManager.Instance.GetData(attributeManager.CurRealmType).Def + inventoryManager.GetDefBonus():f0}";
                _hpLb.Text = $"{attributeManager.CurHp:f0} / {RealmDataManager.Instance.GetData(attributeManager.CurRealmType).MaxHp + inventoryManager.GetMaxHpBonus():f0}";
                _qiLb.Text = $"{attributeManager.CurQi:f0} / {RealmDataManager.Instance.GetData(attributeManager.CurRealmType).MaxQi + inventoryManager.GetMaxQiBonus():f0}";
            };


            attributeManager.CurRealmChanged += () =>
            {
                _realmLb.Text = $"当前境界 : {RealmDataManager.Instance.GetData(attributeManager.CurRealmType).Name}";
                _atkLb.Text = $"{RealmDataManager.Instance.GetData(attributeManager.CurRealmType).Atk + inventoryManager.GetAtkBonus():f0}";
                _defLb.Text = $"{RealmDataManager.Instance.GetData(attributeManager.CurRealmType).Def + inventoryManager.GetDefBonus():f0}";
            };
            attributeManager.CurHpChanged += () =>
            {
                _hpLb.Text = $"{attributeManager.CurHp:f0} / {RealmDataManager.Instance.GetData(attributeManager.CurRealmType).MaxHp + inventoryManager.GetMaxHpBonus():f0}";
            };
            attributeManager.CurQiChanged += () =>
            {
                _qiLb.Text = $"{attributeManager.CurQi:f0} / {RealmDataManager.Instance.GetData(attributeManager.CurRealmType).MaxQi + inventoryManager.GetMaxQiBonus():f0}";
            };
            attributeManager.CurExpChanged += () =>
            {
                _expLb.Text = $"{attributeManager.CurExp:f0} / {RealmDataManager.Instance.GetData(attributeManager.CurRealmType).MaxExp:f0}";
            };
        }
    }
}
