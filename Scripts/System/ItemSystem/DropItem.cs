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
        private ShaderMaterial _shaderMaterial;

        public void Init(ItemType type, int count, Vector2 pos)
        {
            Type = type;
            Count = count;
            Position = pos;
            TextLb.Text = ItemDataManager.Instance.GetItemData(Type).Name;
            TextLb.Visible = false;
            Texture2D texture = GD.Load<Texture2D>(ItemDataManager.Instance.GetItemData(Type).IconPath);
            Vector2 targetSize = new Vector2(8, 8);
            Vector2 texSize = texture.GetSize(); // 获取图片实际的像素大小
            IconSprite.Scale = new Vector2(targetSize.X / texSize.X, targetSize.Y / texSize.Y);// 计算缩放比例：目标尺寸 / 图片实际尺寸
            IconSprite.Texture = texture;
            if (IconSprite.Material is ShaderMaterial shaderMat)
            {
                _shaderMaterial = (ShaderMaterial)shaderMat.Duplicate();// 关键：复制一份材质，确保每个实例的材质相互独立
                IconSprite.Material = _shaderMaterial;// 记得把复制后的独立材质重新赋给当前的 Sprite2D
            }
            ShowText(false);
            GameManager.Instance.ChunkManager.AddItem(this, Position);
        }

        public void ShowText(bool isShow)
        {
            if (isShow)
            {
                TextLb.Visible = true;
                _shaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1));
                _shaderMaterial.SetShaderParameter("outline_width", 1);
            }
            else
            {
                TextLb.Visible = false;
                _shaderMaterial.SetShaderParameter("outline_width", 0.0f);
            }

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


