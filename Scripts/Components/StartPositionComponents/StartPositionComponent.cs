using Godot;
using Solo.Scripts.Components.Core;
using Solo.Scripts.Global.Interfaces;


namespace Solo.Scripts.Components.StartPositionComponents
{
    public class StartPositionComponent : Component
    {
        public Vector2 StartPositon;

        public StartPositionComponent(IEntity owner) : base(owner) { }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {

        }

        public override ComponentSaveData? GetSaveData()
        {
            return new StartPositionComponentSaveData()
            {
                ComponentName = nameof(StartPositionComponent),
                WorldX = StartPositon.X,
                WorldY = StartPositon.Y,
            };
        }
    }
}
