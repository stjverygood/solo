using Godot;
using Solo.Scripts.Components.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Components.PositionComponents
{
    public class PositionComponent : Component
    {
        public PositionComponent(IEntity owner) : base(owner) { }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            PositionComponentSaveData? saveData = (PositionComponentSaveData?)componentSaveData;
            if (saveData != null)
            {
                InitWorldPosition(new Vector2(saveData.WorldX, saveData.WorldY));
            }

        }

        public void InitWorldPosition(Vector2 worldPos)
        {
            ((Node2D)_owner).GlobalPosition = worldPos;
            GameManager.Instance.World.InitChunkPos(GetWorldPosition(), _owner);
        }

        public Vector2 GetWorldPosition()
        {
            return ((Node2D)_owner).GlobalPosition;
        }

        public override ComponentSaveData? GetSaveData()
        {
            return new PositionComponentSaveData()
            {
                ComponentName = nameof(PositionComponent),
                WorldX = ((Node2D)_owner).GlobalPosition.X,
                WorldY = ((Node2D)_owner).GlobalPosition.Y,
            };
        }
    }
}
