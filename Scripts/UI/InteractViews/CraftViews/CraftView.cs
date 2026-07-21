using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Global;
using Solo.Scripts.System.CraftSystem;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.UI.CraftViews
{
    public partial class CraftView : Control
    {
        public CraftType Type;

        [Export] private Label _craftNameLb = null!;
        [Export] private GridContainer _slotGc = null!;
        [Export] private ButtonGroup _btnGroup = null!;
        [Export] private PackedScene _craftItemViewPs = null!;

        [Export] private GridContainer _requiredItemGc = null!;
        [Export] private PackedScene _requiredItemViewPs = null!;

        [Export] private Label _curCraftItemNameLb = null!;
        [Export] private Label _curCraftItemCountLb = null!;
        [Export] private HSlider _countSlider = null!;
        [Export] private Button _craftBtn = null!;

        private ItemType _curCraftItemType;
        private int _curCraftCount;

        public void Init(CraftType type)
        {
            Type = type;

            //todo : 1.列表
            foreach (Node child in _slotGc.GetChildren())// 先清空grid
                child.QueueFree();
            //2. 默认选中第一个
            _countSlider.ValueChanged += (value) =>
            {
                GD.Print(" " + _curCraftItemType + " " + value);
                _curCraftCount = (int)value;
                RefreshRequiredItemList();
            };

            CraftData data = CraftDataManager.Instance.GetData(Type);
            _craftNameLb.Text = data.Name;

            for (int i = 0; i < data.ItemList.Count; i++)
            {

                CraftItemView itemView = _craftItemViewPs.Instantiate<CraftItemView>();
                itemView.Init(data.ItemList[i], i, _btnGroup);
                itemView.Toggled += (selectedCraftItemView) =>
                {
                    //被选中的逻辑

                    _curCraftItemType = selectedCraftItemView.Type;
                    _curCraftCount = 1;
                    _curCraftItemNameLb.Text = ItemDataManager.Instance.GetData(_curCraftItemType).Name;
                    _curCraftItemCountLb.Text = "*1";
                    RefreshCountSlider();//刷新滑块, 计算最大值, 当前值重置成1
                    RefreshRequiredItemList();//刷新需要材料gc
                };
                _slotGc.AddChild(itemView);
                if (i == 0)
                    itemView.SetSelected();
            }

            _craftBtn.Pressed += () =>
            {
                if (GameManager.Instance.IsDebugMode == false)//非调试模式要扣物品
                {
                    foreach ((ItemType, int) tuple in ItemDataManager.Instance.GetData(_curCraftItemType).CraftRequiredItemList)
                        GameManager.Instance.Player.Core.GetComponent<InventoryComponent>().RemoveItem(tuple.Item1, tuple.Item2 * _curCraftCount);
                }

                GameManager.Instance.Player.Core.GetComponent<InventoryComponent>().AddItem(new ItemInstance() { Type = _curCraftItemType, Count = _curCraftCount, CurDur = ItemDataManager.Instance.GetData(_curCraftItemType).MaxDur });
                RefreshCountSlider();
                RefreshRequiredItemList();
            };
        }

        public void RefreshCountSlider()
        {
            //GD.Print(craftItemType + " RefreshCountSlider");
            //遍历每个材料, 看看背包能合成多少个, 如何取最少材料的那个
            int minCount = int.MaxValue;
            foreach ((ItemType, int) tuple in ItemDataManager.Instance.GetData(_curCraftItemType).CraftRequiredItemList)
            {
                int itemCount = GameManager.Instance.Player.Core.GetComponent<InventoryComponent>().GetItemCount(tuple.Item1);
                int canCraftCount = itemCount / tuple.Item2;
                if (canCraftCount < minCount)
                    minCount = canCraftCount;
            }
            _countSlider.MinValue = 1;
            _countSlider.MaxValue = minCount;
            _countSlider.Value = 1;
        }

        public void RefreshRequiredItemList()
        {
            foreach (Node child in _requiredItemGc.GetChildren())// 先清空grid
                child.QueueFree();

            bool canCraft = true;

            foreach ((ItemType, int) tuple in ItemDataManager.Instance.GetData(_curCraftItemType).CraftRequiredItemList)
            {
                RequiredItemView requiredItemView = _requiredItemViewPs.Instantiate<RequiredItemView>();
                requiredItemView.Init(tuple.Item1, tuple.Item2 * _curCraftCount);
                _requiredItemGc.AddChild(requiredItemView);

                int itemCount = GameManager.Instance.Player.Core.GetComponent<InventoryComponent>().GetItemCount(tuple.Item1);
                if (itemCount < tuple.Item2 * _curCraftCount)
                {
                    canCraft = false;
                    requiredItemView.SetIsEnough(false);
                }
                else
                {
                    requiredItemView.SetIsEnough(true);
                }
            }


            if (canCraft == true)
            {
                _craftBtn.Disabled = false;
                _curCraftItemNameLb.Modulate = Color.Color8(62, 137, 72);
                _curCraftItemCountLb.Modulate = Color.Color8(62, 137, 72);
            }
            else
            {
                if (GameManager.Instance.IsDebugMode)
                    _craftBtn.Disabled = false;
                else
                    _craftBtn.Disabled = true;
                _curCraftItemNameLb.Modulate = Color.Color8(228, 59, 68);
                _curCraftItemCountLb.Modulate = Color.Color8(228, 59, 68);
            }
        }
    }
}
