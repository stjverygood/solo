using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.System.BuildingSystem
{
    public partial class BuildingBase : Node2D
    {
        public BuildingType Type;
        [Export] public float MaxHp = 100;
        [Export] public ItemType DropItemType;
        [Export] public int MinDropCount;
        [Export] public int MaxDropCount;
        [Export] public Sprite2D Sprite;
        [Export] public PackedScene DropItemPs;
        [Export] public HpBarControl _hpBarControl;
        private ShaderMaterial _shaderMaterial;

        private float _curHp;


        public virtual void Init(BuildingType type, Vector2 snapPos)
        {
            Type = type;
            Position = snapPos;
            _curHp = MaxHp;
            _hpBarControl.Refresh(MaxHp, _curHp);
            _hpBarControl.Visible = false;
            if (Sprite.Material is ShaderMaterial shaderMat)
            {
                _shaderMaterial = (ShaderMaterial)shaderMat.Duplicate();// 关键：复制一份材质，确保每个实例的材质相互独立
                Sprite.Material = _shaderMaterial;// 记得把复制后的独立材质重新赋给当前的 Sprite2D
            }
            ShowOutline(false);
            GameManager.Instance.BuildingManager.Place(BuildingDataManager.Instance.GetBuildingData(Type), snapPos);
            GameManager.Instance.ChunkManager.AddItem(this, Position);
        }

        public override void _Process(double delta)
        {
        }

        public virtual void TakeDamage(ItemType? dmgItemType, float damage)
        {
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(Sprite, "skew", 0.3f, 0.1f);// 左右晃动
            animTween.TweenProperty(Sprite, "skew", -0.3f, 0.1f);
            animTween.TweenProperty(Sprite, "skew", 0f, 0.1f);
            _curHp -= damage;
            _hpBarControl.Refresh(MaxHp, _curHp);
            if (_curHp <= 0)
            {
                GameManager.Instance.ChunkManager.RemoveItem(this, GlobalPosition);
                _curHp = 0;
                DropItem dropItem = DropItemPs.Instantiate<DropItem>();
                GetTree().CurrentScene.AddChild(dropItem);
                dropItem.Init(DropItemType, 2, Position);
                QueueFree();
            }
        }

        public virtual void ShowOutline(bool isShow)
        {
            if (isShow)
            {
                _shaderMaterial.SetShaderParameter("outline_color", new Godot.Color(1, 1, 1));
                _shaderMaterial.SetShaderParameter("outline_width", 1);
            }
            else
            {
                _shaderMaterial.SetShaderParameter("outline_width", 0.0f);
            }
        }
    }
}