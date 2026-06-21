using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;
using System.Collections.Generic;

namespace Solo.Scripts.System.CraftSystem
{
    //物品合成台的类型
    public enum CraftViewType
    {
        Basic,//直接能打开
        Building,//合成建筑的, 需要在建筑合成台交互
        Seed,//合成种子的
        Weapon, //剑, 弓
        Tool,//镐,斧,壶, 鱼竿
        Defend,//盔甲鞋
    }

    public partial class CraftView : Control
    {
        [Export] public CraftViewType Type;
        [Export] private GridContainer _slotGc;
        [Export] private PackedScene _slotViewPs;
        [Export] private ButtonGroup _btnGroup;
        [Export] private Label _selectedItemNameLb;
        [Export] private Label _selectedItemRequiredLb;
        [Export] private Button _craftBtn;
        private List<CraftItem> CraftItemList = new List<CraftItem>();
        private CraftItem _curCraftItem;

        public override void _Ready()
        {
            _craftBtn.Pressed += () =>
            {
                if (_curCraftItem == null) return;
                foreach ((ItemType, int) t in _curCraftItem.RequiredItemList)
                {
                    int remain = t.Item2;
                    remain -= GameManager.Instance.Player.BagInventory.RemoveItemByType(t.Item1, remain);
                    GameManager.Instance.Player.FastBarInventory.RemoveItemByType(t.Item1, remain);
                }
                GameManager.Instance.Player.AddItemToInventory(new ItemInstance() { Type = _curCraftItem.Type, Count = 1, CurDur = ItemDataManager.Instance.GetItemData(_curCraftItem.Type).MaxDur });
                CheckCanCraft();
            };
        }

        public void RefreshType(CraftViewType type)
        {
            Type = type;
            foreach (Node child in _slotGc.GetChildren())// 先清空grid
                child.QueueFree();
            CraftItemList.Clear();

            switch (Type)
            {
                case CraftViewType.Basic://徒手的只能做木套
                    CraftItemList.Add(new CraftItem(ItemType.MainBase, new List<(ItemType, int)>() { (ItemType.MainBaseStone, 0), (ItemType.Wood, 0), (ItemType.Stone, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.BuildingCraft, new List<(ItemType, int)>() { (ItemType.MainBaseStone, 0), (ItemType.Wood, 0), (ItemType.Stone, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.Rope, new List<(ItemType, int)>() { (ItemType.Grass, 1) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodSword, new List<(ItemType, int)>() { (ItemType.Wood, 3), (ItemType.Rope, 5) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodBow, new List<(ItemType, int)>() { (ItemType.Wood, 4), (ItemType.Rope, 5) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodPickaxe, new List<(ItemType, int)>() { (ItemType.Wood, 4), (ItemType.Rope, 5) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodAxe, new List<(ItemType, int)>() { (ItemType.Wood, 3), (ItemType.Rope, 5) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodPot, new List<(ItemType, int)>() { (ItemType.Wood, 4), (ItemType.Rope, 5) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodRod, new List<(ItemType, int)>() { (ItemType.Wood, 3), (ItemType.Rope, 5) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodHelmet, new List<(ItemType, int)>() { (ItemType.Wood, 4), (ItemType.Rope, 5), (ItemType.Silk, 5) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodArmor, new List<(ItemType, int)>() { (ItemType.Wood, 3), (ItemType.Rope, 5), (ItemType.Silk, 5) }));
                    CraftItemList.Add(new CraftItem(ItemType.WoodBoot, new List<(ItemType, int)>() { (ItemType.Wood, 4), (ItemType.Rope, 5), (ItemType.Silk, 5) }));
                    break;
                case CraftViewType.Building://建筑合成台
                    CraftItemList.Add(new CraftItem(ItemType.MainBase, new List<(ItemType, int)>() { (ItemType.MainBaseStone, 0), (ItemType.Wood, 0), (ItemType.Stone, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.Flag, new List<(ItemType, int)>() { (ItemType.MainBaseStone, 0), (ItemType.Wood, 0), (ItemType.Stone, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.ToolCraft, new List<(ItemType, int)>() { (ItemType.MainBaseStone, 0), (ItemType.Wood, 0), (ItemType.Stone, 0) }));
                    break;
                case CraftViewType.Weapon://高级武器合成台
                    CraftItemList.Add(new CraftItem(ItemType.IronSword, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.IronBow, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldSword, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldBow, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadeSword, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadeBow, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    break;
                case CraftViewType.Tool://高级工具合成台
                    CraftItemList.Add(new CraftItem(ItemType.IronPickaxe, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.IronAxe, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.IronPot, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.IronRod, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldPickaxe, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldAxe, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldPot, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldRod, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadePickaxe, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadeAxe, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadePot, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadeRod, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    break;
                case CraftViewType.Defend://高级防具合成台
                    CraftItemList.Add(new CraftItem(ItemType.IronHelmet, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.IronArmor, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.IronBoot, new List<(ItemType, int)>() { (ItemType.Iron, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldHelmet, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldArmor, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.GoldBoot, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadeHelmet, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadeArmor, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    CraftItemList.Add(new CraftItem(ItemType.JadeBoot, new List<(ItemType, int)>() { (ItemType.Gold, 0) }));
                    break;
            }

            for (int i = 0; i < CraftItemList.Count; i++)
            {
                CraftSlotView slotView = _slotViewPs.Instantiate<CraftSlotView>();
                slotView.Init(i, _btnGroup, CraftItemList[i]);
                slotView.Toggled += (selectedSlotView) =>
                {
                    //被选中的逻辑
                    _curCraftItem = selectedSlotView.CraftItem;
                    _selectedItemNameLb.Text = ItemDataManager.Instance.GetItemData(selectedSlotView.CraftItem.Type).Name;
                    string requiredStr = "所需材料 : ";
                    foreach ((ItemType, int) t in selectedSlotView.CraftItem.RequiredItemList)
                    {
                        requiredStr += $"{ItemDataManager.Instance.GetItemData(t.Item1).Name} * {t.Item2}, ";
                    }
                    _selectedItemRequiredLb.Text = requiredStr;
                    CheckCanCraft();
                };
                _slotGc.AddChild(slotView);
                if (i == 0)
                    slotView.SetSelected();
            }
        }

        private void CheckCanCraft()
        {
            foreach ((ItemType, int) t in _curCraftItem.RequiredItemList)
            {
                int count = GameManager.Instance.Player.FastBarInventory.GetItemCount(t.Item1) + GameManager.Instance.Player.BagInventory.GetItemCount(t.Item1);
                if (count < t.Item2)
                {
                    _craftBtn.Disabled = true;
                    return;
                }
            }
            _craftBtn.Disabled = false;
        }
    }
}
