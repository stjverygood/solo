using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Entities.Components.PositionComponents
{
    public class PositionComponent : Component
    {
        public PositionComponent(IEntity owner) : base(owner) { }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {

        }

        public Vector2 SetWorldPosition(Vector2 worldPos)
        {
            return ((Node2D)_owner).GlobalPosition = worldPos;
        }
        public Vector2 GetWorldPosition()
        {
            return ((Node2D)_owner).GlobalPosition;
        }

        public override ComponentSaveData? GetSaveData()
        {
            return new PositionComponentSaveData()
            {
                TypeName = nameof(PositionComponent),
                WorldX = ((Node2D)_owner).GlobalPosition.X,
                WorldY = ((Node2D)_owner).GlobalPosition.Y,
            };
        }
    }
}
