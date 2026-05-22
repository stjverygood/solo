using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solo.Scripts.System.ChunkSystem.Data
{
    public class WorldObjectData
    {
        public string Type { get; set; }        // "Tree" 或 "Stone"
        public Vector2 Pos { get; set; }   // 相对于区块左上角的偏移坐标
    }
}
