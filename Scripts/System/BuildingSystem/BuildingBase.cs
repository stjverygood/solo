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
        [Export] public Node2D BodyRoot;
        [Export] public PackedScene DropItemPs;
        private float _curHp;


        public virtual void Init(BuildingType type, Vector2 snapPos)
        {
            Type = type;
            Position = snapPos;
            _curHp = MaxHp;
            GameManager.Instance.BuildingManager.Place(BuildingDataManager.Instance.GetBuildingData(Type), snapPos);
            GameManager.Instance.ChunkManager.AddItem(this, Position);
        }

        public override void _Process(double delta)
        {
        }

        public virtual void TakeDamage(ItemType? dmgItemType, float damage)
        {
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(BodyRoot, "skew", 0.3f, 0.1f);// 左右晃动
            animTween.TweenProperty(BodyRoot, "skew", -0.3f, 0.1f);
            animTween.TweenProperty(BodyRoot, "skew", 0f, 0.1f);
            _curHp -= damage;
            GD.Print("damage : " + damage);
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
    }
}