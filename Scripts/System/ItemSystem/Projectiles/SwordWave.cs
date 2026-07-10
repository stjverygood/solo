using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using System.Collections.Generic;

namespace Solo.Scripts.System.ItemSystem
{
    public partial class SwordWave : Area2D
    {
        [Export] private AnimatedSprite2D _animSprite;
        private int _damage = 10;

        private List<ITargetable> _targetList = new List<ITargetable>();

        public void Init(Vector2 worldPosition, Vector2 atkDir)
        {
            GlobalPosition = worldPosition + atkDir * 10;
            Rotation = atkDir.Angle();
            _animSprite.Play(GD.Randf() < 0.5f ? "Wave1" : "Wave2");
            BodyEntered += SwordWave_BodyEntered;
            BodyExited += SwordWave_BodyExited;
            _animSprite.FrameChanged += _animSprite_FrameChanged;
            _animSprite.AnimationFinished += OnAnimationFinished;
        }

        private void _animSprite_FrameChanged()
        {
            if (_animSprite.Frame == 1)
            {
                GD.Print($"frame 1 : {_targetList.Count}");
                GameManager.Instance.Player.TriggerScreenShake(2);
                foreach (ITargetable targetable in _targetList)
                {
                    if (targetable.IsVaild())
                    {
                        if (targetable is Player)
                            continue;
                        targetable.TakeDamage(this, _damage, null);
                    }
                }
            }
        }

        private void SwordWave_BodyEntered(Node2D body)
        {
            if (body is ITargetable target)
            {
                _targetList.Add(target);
            }
        }
        private void SwordWave_BodyExited(Node2D body)
        {
            if (body is ITargetable target)
            {
                _targetList.Remove(target);
            }
        }

        private void OnAnimationFinished()
        {
            QueueFree();
        }
    }
}