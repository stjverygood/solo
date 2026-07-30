using Godot;
using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using System.Collections.Generic;

namespace Solo.Scripts.Entities.Components.DynamicComponents
{
    public class DynamicComponent : Component
    {
        private Vector2I _chunkPos;

        public DynamicComponent(IEntity owner) : base(owner) { }

        public override void Init(ComponentData? componentData, ComponentSaveData? componentSaveData)
        {

        }
        public override ComponentSaveData? GetSaveData()
        {
            return null;
        }



        public void RefreshChunkPos(Vector2 worldPos)
        {
            Vector2I curChunkPos = GameManager.Instance.ChunkManager.WorldToChunkPos(worldPos);
            if (_chunkPos != curChunkPos)
            {
                var entityMap = GameManager.Instance.World.EntityMap;
                if (entityMap.ContainsKey(_chunkPos))
                    entityMap[_chunkPos].Remove(_owner);
                _chunkPos = curChunkPos;
                if (!entityMap.ContainsKey(_chunkPos))
                    entityMap[_chunkPos] = new List<IEntity>();
                if (!entityMap[_chunkPos].Contains(_owner))
                    entityMap[_chunkPos].Add(_owner);

            }
        }
    }
}
