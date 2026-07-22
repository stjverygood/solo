using Godot;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using System.Collections.Generic;
namespace Solo.Scripts.Projectiles
{
    public partial class HammerWave : Area2D
    {
        [Export] private AnimatedSprite2D _animSprite = null!;
        private int _damage = 10;

        private List<ITargetable> _targetList = new List<ITargetable>();

        public void Init(Vector2 worldPosition, Vector2 atkDir)
        {
            GlobalPosition = worldPosition + atkDir * 100;
            _animSprite.Play("Wave1");
            BodyEntered += SwordWave_BodyEntered;
            BodyExited += SwordWave_BodyExited;
            _animSprite.FrameChanged += _animSprite_FrameChanged;
            _animSprite.AnimationFinished += OnAnimationFinished;
        }

        private void _animSprite_FrameChanged()
        {
            if (_animSprite.Frame == 3)
            {
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