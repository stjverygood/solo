using Godot;
using System;
using Solo.Scripts.Character;


namespace Solo.Global
{
    public partial class GameManager : Node
    {
        static private GameManager _instance;
        static public GameManager Instance => _instance;

        [Export] public PackedScene PlayerPs;


        public Player Player;
        public ChunkManager ChunkManager;
        //public MapManager MapManager;


        public override void _Ready()
        {
            _instance = this;
            GD.Print("Game Start ~~");

            //MapManager = MapManagerPs.Instantiate<MapManager>();
            //MapManager.Init();
            //GetTree().CurrentScene.AddChild(MapManager);

            Player = PlayerPs.Instantiate<Player>();
            //layer.GlobalPosition = MapManager.GetRandomCellWorldPosition(CellType.Earth);
            GetTree().CurrentScene.AddChild(Player);

            //MapManager.GenerateResource(1000);
        }

        public override void _Process(double delta)
        {
        }
    }

}
