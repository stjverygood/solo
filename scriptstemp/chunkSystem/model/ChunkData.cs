using Solo.Scripts.Global;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solo.Scripts.ChunkSystem.Model
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
