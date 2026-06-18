using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.System.ItemSystem
{
    public partial class DropItem : Area2D, ITargetable
    {
        //public ItemType Type;
        public ItemInstance ItemInstance;
        public int Count;
        [Export] public Label TextLb;
        [Export] public Sprite2D IconSprite;
        private ShaderMaterial _shaderMaterial;

        public void Init(ItemInstance instance, Vector2 pos)
        {
            ItemInstance = instance;
            GlobalPosition = pos;
            TextLb.Text = ItemDataManager.Instance.GetItemData(ItemInstance.Type).Name;
            TextLb.Visible = false;
            Texture2D texture = GD.Load<Texture2D>(ItemDataManager.Instance.GetItemData(ItemInstance.Type).IconPath);
            Vector2 targetSize = new Vector2(16, 16);
            Vector2 texSize = texture.GetSize(); // 获取图片实际的像素大小
            IconSprite.Scale = new Vector2(targetSize.X / texSize.X, targetSize.Y / texSize.Y);// 计算缩放比例：目标尺寸 / 图片实际尺寸
            IconSprite.Texture = texture;
            if (IconSprite.Material is ShaderMaterial shaderMat)
            {
                _shaderMaterial = (ShaderMaterial)shaderMat.Duplicate();// 关键：复制一份材质，确保每个实例的材质相互独立
                IconSprite.Material = _shaderMaterial;// 记得把复制后的独立材质重新赋给当前的 Sprite2D
            }
            ShowOutline(false);
            GameManager.Instance.ChunkManager.AddItem(this, Position);
        }

        public void ApplyForce()
        {
            float duration = 0.5f; // 整个弹跳持续时间
            float jumpHeight = (float)GD.RandRange(30f, 50f); // 向上跳跃的高度
            Tween airTween = CreateTween();
            airTween.TweenProperty(IconSprite, "position:y", -jumpHeight, duration * 0.5f)
                    .SetTrans(Tween.TransitionType.Quad)
                    .SetEase(Tween.EaseType.Out);
            airTween.TweenProperty(IconSprite, "position:y", 0f, duration * 0.5f)
                    .SetTrans(Tween.TransitionType.Quad)
                    .SetEase(Tween.EaseType.In);
            airTween.TweenCallback(Callable.From(() =>
            {
                Tween bounceTween = CreateTween();
                bounceTween.TweenProperty(IconSprite, "position:y", -8f, 0.1f).SetEase(Tween.EaseType.Out);
                bounceTween.TweenProperty(IconSprite, "position:y", 0f, 0.1f).SetEase(Tween.EaseType.In);
            }));
        }

        public bool CanInteract()
        {
            return true;
        }
        public bool CanAtk()
        {
            return false;
        }
        public void ShowInteractTip(bool isShow)
        {
            if (isShow)
            {
                TextLb.Visible = true;
                //_shaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1));
                //_shaderMaterial.SetShaderParameter("outline_width", 1);
            }
            else
            {
                TextLb.Visible = false;
                //_shaderMaterial.SetShaderParameter("outline_width", 0.0f);
            }
        }
        public void ShowAtkTip(bool isShow)
        {
            return;
        }
        public Vector2 GetWorldPosition()
        {
            return GlobalPosition;
        }
        public void TakeDamage(float damage, ItemType? itemType)
        {
            return;
        }

        public void ShowOutline(bool isShow)
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

        public void Interact()
        {
            int remainCount = GameManager.Instance.Player.AddItemToInventory(ItemInstance);
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

        public bool IsVaild()
        {
            return IsInstanceValid(this);
        }

        public TargetType GetTargetType()
        {
            return TargetType.DropItem;
        }
    }
}


