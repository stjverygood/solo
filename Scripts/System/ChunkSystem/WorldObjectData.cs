using Godot;

namespace Solo.Scripts.System.ChunkSystem
{
    public class WorldObjectData
    {
        public string Type { get; set; }        // "Tree" 或 "Stone"
        public Vector2 Pos { get; set; }   // 相对于区块左上角的偏移坐标
    }
}
