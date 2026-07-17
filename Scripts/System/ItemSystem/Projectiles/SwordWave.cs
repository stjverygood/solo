using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using System.Collections.Generic;

namespace Solo.Scripts.System.ItemSystem
{
    public partial class SwordWave : Area2D
    {
        [Export] private AnimatedSprite2D _animSprite = null!;
        private IEntity _spawner = null!;
        private float _atk = 0;//一开始就要记录

        private List<IEntity> _targetEntityList = new List<IEntity>();

        public void Init(IEntity spawner, Vector2 atkDir)
        {
            _spawner = spawner;
            GlobalPosition = ((Node2D)_spawner).GlobalPosition + atkDir * 10;
            _atk = _spawner.Core.GetComponent<AtkComponent>().Value;
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
                //GD.Print($"frame 1 : {_targetEntityList.Count}");
                GameManager.Instance.Player.TriggerScreenShake(2);
                foreach (IEntity entity in _targetEntityList)
                {

                    if (IsInstanceValid(((Node2D)entity)) == false)
                        continue;
                    if (entity == _spawner)
                        continue;
                    if (entity.Core.TryGetComponent(out DefComponent defComponent) == false)
                        continue;
                    if (entity.Core.TryGetComponent(out HpComponent hpComponent) == false)
                        continue;
                    float damage = GameManager.Instance.CalculateDamage(_atk, defComponent.Value);
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