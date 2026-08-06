using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;

namespace Solo.Scripts.Entities.Components.PositionComponents
{
    public class PositionComponent : Component
    {
        public Vector2I CurChunkPos;
        public PositionComponent(IEntity owner) : base(owner) { }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {
            PositionComponentSaveData? saveData = (PositionComponentSaveData?)componentSaveData;
            if (saveData != null)
                SetWorldPosition(new Vector2(saveData.WorldX, saveData.WorldY));
        }

        public void SetWorldPosition(Vector2 worldPos)
        {
            ((Node2D)_owner).GlobalPosition = worldPos;
            GameManager.Instance.World.RefreshEntityChunk(_owner);
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
