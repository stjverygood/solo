using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.System.ResourceSystem
{
    public partial class ResObj : Node2D
    {
        [Export] public Node2D BodyRoot;
        [Export] public ResObjType Type;
        [Export] public ItemType DropItemType;
        [Export] public PackedScene DropItemPs;
        public int MaxHp = 30;
        private int _curHp;

        public override void _Ready()
        {
            _curHp = MaxHp;

        }

        public override void _Process(double delta)
        {
        }

        public void TakeDamage(int damage)
        {
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(BodyRoot, "skew", 0.3f, 0.1f);// 左右晃动
            animTween.TweenProperty(BodyRoot, "skew", -0.3f, 0.1f);
            animTween.TweenProperty(BodyRoot, "skew", 0f, 0.1f);
            _curHp -= damage;
            if (_curHp <= 0)
            {
                _curHp = 0;
                DropItem dropItem = DropItemPs.Instantiate<DropItem>();
                GetTree().CurrentScene.AddChild(dropItem);
                dropItem.GlobalPosition = GlobalPosition;
                dropItem.Init(new ItemInstance() { Type = DropItemType, Count = 2 });//ItemManager.Instance.GetItemData(DropItemType)
                QueueFree();
            }
        }
    }
}


