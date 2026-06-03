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

        public void Init()
        {
            foreach (Node child in _slotGc.GetChildren())// 先清空grid
                child.QueueFree();

            switch (Type)
            {
                case CraftViewType.Basic://徒手的只能做木套
                    CraftItemList.Add(new CraftItem(ItemType.MainBase, new List<(ItemType, int)>() { (ItemType.MainBaseStone, 0), (ItemType.Wood, 0), (ItemType.Stone, 0) }));
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
            _craftBtn.Pressed += () =>
            {
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
