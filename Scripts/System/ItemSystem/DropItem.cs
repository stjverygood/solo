using Godot;
using Solo.Scripts.Global;

namespace Solo.Scripts.System.ItemSystem
{
    public partial class DropItem : Node2D
    {
        public ItemType Type;
        public int Count;
        [Export] public Label TextLb;
        [Export] public Sprite2D IconSprite;

        public void Init(ItemType type, int count, Vector2 pos)
        {
            Type = type;
            Count = count;
            Position = pos;
            TextLb.Text = Type.ToString();
            TextLb.Visible = false;
            Texture2D texture = GD.Load<Texture2D>(ItemDataManager.Instance.GetItemData(Type).IconPath);
            Vector2 targetSize = new Vector2(8, 8);
            Vector2 texSize = texture.GetSize(); // 获取图片实际的像素大小
            IconSprite.Scale = new Vector2(targetSize.X / texSize.X, targetSize.Y / texSize.Y);// 计算缩放比例：目标尺寸 / 图片实际尺寸
            IconSprite.Texture = texture;
            GameManager.Instance.ChunkManager.AddItem(this, Position);
        }

        public void ShowText(bool isShow)
        {
            TextLb.Visible = isShow;
        }

        public void Pickup()
        {
            int remainCount = GameManager.Instance.Player.AddItemToInventory(new ItemInstance() { Type = Type, Count = Count });
            if (remainCount == 0)
            {
                GameManager.Instance.ChunkManager.RemoveItem(this, Position);
                QueueFree();
            }
            else
            {
                Count = remainCount;
            }
        }
    }
}


