using Godot;
using System.Collections.Generic;

namespace Solo.Scripts.System.ChunkSystem
{
    public class ChunkData
    {
        public Vector2I Pos { get; private set; }
        public List<WorldObjectData> WorldObjects { get; set; }//记录区块内的物品
        public ChunkData(Vector2I pos)
        {
            Pos = pos;
            WorldObjects = new List<WorldObjectData>();
        }
    }
}
