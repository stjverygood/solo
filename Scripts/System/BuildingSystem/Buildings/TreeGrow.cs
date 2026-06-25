using Godot;
using Solo.Scripts.Global;
namespace Solo.Scripts.System.BuildingSystem.Buildings
{
    public partial class TreeGrow : Building
    {
        [Export] Texture2D _earthTexture;
        [Export] Texture2D _earthWetTexture;
        [Export] Sprite2D _earthSprite;

        //每10s有一定概率成长
        private float _growDuration = 3;
        private float _growTimer = 0;
        private bool _isWatered = false;
        private float _curGrowChance = 0;

        public override void Init(BuildingType type, Vector2 snapPos)
        {
            base.Init(type, snapPos);
            _earthSprite.Texture = _earthTexture;
        }

        public override void _PhysicsProcess(double delta)
        {
            if (_isWatered == false)
                return;
            _growTimer += (float)delta;
            if (_growTimer >= _growDuration)
            {
                _growTimer = 0;
                if (GD.Randf() < _curGrowChance)
                {
                    QueueFree();
                    PackedScene treePs = GD.Load<PackedScene>(BuildingDataManager.Instance.GetBuildingData(BuildingType.Tree).TscnPath);
                    Tree tree = treePs.Instantiate<Tree>();
                    GetTree().CurrentScene.AddChild(tree);
                    tree.Init(BuildingType.Tree, GlobalPosition);
                }
            }
        }

        public void Watering(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.WoodPot:
                    _isWatered = true;
                    _curGrowChance = 0.1f;
                    _earthSprite.Texture = _earthWetTexture;
                    break;
                case ItemType.IronPot:
                    _isWatered = true;
                    _curGrowChance = 0.2f;
                    _earthSprite.Texture = _earthWetTexture;
                    break;
                case ItemType.GoldPot:
                    _isWatered = true;
                    _curGrowChance = 0.3f;
                    _earthSprite.Texture = _earthWetTexture;
                    break;
                case ItemType.JadePot:
                    _isWatered = true;
                    _curGrowChance = 0.4f;
                    _earthSprite.Texture = _earthWetTexture;
                    break;
            }
        }
    }
}