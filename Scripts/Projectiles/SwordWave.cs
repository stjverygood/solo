using Godot;
using Solo.Scripts.Components.AtkComponents;
using Solo.Scripts.Components.DefComponents;
using Solo.Scripts.Components.HpComponents;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using System.Collections.Generic;

namespace Solo.Scripts.Projectiles
{
    public partial class SwordWave : Area2D
    {
        [Export] private AnimatedSprite2D _animSprite = null!;
        private List<IEntity> _targetEntityList = new List<IEntity>();
        private ProjectileContext _context = null!;

        public void Init(ProjectileContext context)
        {
            _context = context;
            Vector2 dir = (_context.TargetPos - _context.StartPos).Normalized();
            GlobalPosition = ((Node2D)_context.Projecter).GlobalPosition + dir * 10;
            Rotation = dir.Angle();
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
                //GD.Print($"frame 1 : {_targetEntityList.Count}");
                GameManager.Instance.Player.TriggerScreenShake(2);
                foreach (IEntity entity in _targetEntityList)
                {

                    if (IsInstanceValid(((Node2D)entity)) == false)
                        continue;
                    if (entity == _context.Projecter)
                        continue;
                    if (entity.Core.TryGetComponent(out HpComponent hpComponent) == false)
                        continue;

                    float def = 0;
                    if (entity.Core.TryGetComponent(out DefComponent defComponent))
                    {
                        def = defComponent.GetDef();
                    }
                    float damage = GameManager.Instance.CalculateDamage(_context.Projecter.Core.GetComponent<AtkComponent>().GetAtk(), def);
                    hpComponent.Consume(damage);
                }
            }
        }

        private void SwordWave_BodyEntered(Node2D body)
        {
            if (body is IEntity entity)
            {
                _targetEntityList.Add(entity);
            }
        }
        private void SwordWave_BodyExited(Node2D body)
        {
            if (body is IEntity entity)
            {
                _targetEntityList.Remove(entity);
            }
        }

        private void OnAnimationFinished()
        {
            QueueFree();
        }
    }
}