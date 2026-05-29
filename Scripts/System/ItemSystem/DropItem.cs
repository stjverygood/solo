using Godot;
using Solo.Scripts.System.InventorySystem;

namespace Solo.Scripts.System.ItemSystem
{
    public partial class DropItem : Node2D
    {
        [Export] public Label TextLb;
        [Export] public Sprite2D IconSprite;
        public ItemInstance ItemInstance;

        public override void _Ready()
        {

        }

        //public override void _Process(double delta)
        //{
        //}

        public void Init(ItemInstance itemInstance)
        {
            ItemInstance = itemInstance;
            TextLb.Text = ItemInstance.Type.ToString();
            TextLb.Visible = false;
            Texture2D texture = GD.Load<Texture2D>(ItemManager.Instance.GetItemData(ItemInstance.Type).IconPath);
            Vector2 targetSize = new Vector2(8, 8);
            Vector2 texSize = texture.GetSize(); // 获取图片实际的像素大小
            IconSprite.Scale = new Vector2(targetSize.X / texSize.X, targetSize.Y / texSize.Y);// 计算缩放比例：目标尺寸 / 图片实际尺寸
            IconSprite.Texture = texture;
        }

        public void ShowText(bool isShow)
        {
            TextLb.Visible = isShow;
        }

        public void AddToPlayerInventory(Inventory fastBar, Inventory bag)
        {
            ItemInstance.Count -= fastBar.AddItemInstance(ItemInstance);//优先添加到快捷栏
            if (ItemInstance.Count != 0)//有剩余就添加到背包
            {
                ItemInstance.Count -= bag.AddItemInstance(ItemInstance);
            }
            if (ItemInstance.Count == 0)//全部添加成功就销毁掉
            {
                QueueFree();
            }
        }
    }
}


