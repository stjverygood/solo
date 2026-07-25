using Godot;
using Solo.Scripts.Entities.Components;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Entities.Players;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
namespace Solo.Scripts.Entities.Expballs
{
    public partial class ExpBall : Area2D, IEntity, ISaveable
    {
        public EntityType Type;
        private IEntity? _curTargetEntity;
        private float _exp;

        public EntityCore Core => throw new global::System.NotImplementedException();

        public void Init(EntityType type, Vector2 worldPos, EntitySaveData? entitySaveData = null)
        {
            Type = type;
            ExpBallSaveData? saveData = (ExpBallSaveData?)entitySaveData;

            if (saveData == null)
                GlobalPosition = worldPos + new Vector2(GD.RandRange(-10, 10), GD.RandRange(-10, 10));
            else
                GlobalPosition = new Vector2(saveData.WorldX, saveData.WorldY);

            if (saveData != null)
            {
                _exp = saveData.Exp;
            }

            BodyEntered += ExpBall_BodyEntered;
            BodyExited += ExpBall_BodyExited;
        }

        public void SetExp(float exp)
        {
            _exp = exp;
        }

        public EntitySaveData GetSaveData()
        {
            return new ExpBallSaveData()
            {
                Type = Type,
                WorldX = GlobalPosition.X,
                WorldY = GlobalPosition.Y,
                Exp = _exp
            };
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_curTargetEntity == null)
                return;

            if (GlobalPosition.DistanceSquaredTo(((Node2D)_curTargetEntity).GlobalPosition) < 5)
            {
                _curTargetEntity.Core.GetComponent<ExpComponent>().Gain(_exp);
                QueueFree();
            }

            GlobalPosition += (((Node2D)_curTargetEntity).GlobalPosition - GlobalPosition).Normalized() * 50 * (float)delta;
        }

        private void ExpBall_BodyEntered(Node2D body)
        {
            if (body is Player player)
            {
                _curTargetEntity = player;
            }
        }

        private void ExpBall_BodyExited(Node2D body)
        {
            if (body is Player player)
            {
                _curTargetEntity = null;
            }
        }
    }
}
