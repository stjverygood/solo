using Godot;
using System;

namespace Solo.Scripts.System.ResourceSystem
{
    public partial class ResItem : Node2D
    {
        [Export] Node2D BodyRoot; 
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
            GD.Print("get damage : " + damage);
            Tween animTween = CreateTween().SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
            animTween.TweenProperty(BodyRoot, "skew", 0.3f, 0.1f);// 走动效果：左右晃动或轻微拉伸
            animTween.TweenProperty(BodyRoot, "skew", -0.3f, 0.1f);
            animTween.TweenProperty(BodyRoot, "skew", 0f, 0.1f);
            _curHp -= damage;
            if (_curHp <= 0)
            {
                _curHp = 0;
                QueueFree();
            }
        }
    }
}


