using Godot;
using Solo.Scripts.Global;
using Solo.Scripts.System.BuildingSystem;
using Solo.Scripts.System.BuildingSystem.Buildings;

public partial class BuildingPreview : Node2D
{
    public BuildingType Type;
    [Export] private Sprite2D _sprite;
    private Vector2 _curDir = Vector2.Down;
    private bool _canPlace = false;

    public void Init(ItemType itemType, Vector2 mousePos)
    {
        switch (itemType)
        {
            case ItemType.MainBase:
                Type = BuildingType.MainBase;
                break;
            case ItemType.Flag:
                Type = BuildingType.Flag;
                break;
            case ItemType.BuildingCraft:
                Type = BuildingType.BuildingCraft;
                break;
            case ItemType.ToolCraft:
                Type = BuildingType.ToolCraft;
                break;
            case ItemType.ArmorCraft:
                Type = BuildingType.ArmorCraft;
                break;
            case ItemType.Crucible:
                Type = BuildingType.Crucible;
                break;
            case ItemType.ItemBox:
                Type = BuildingType.ItemBox;
                break;
        }
        BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(Type);
        _sprite.Texture = GD.Load<Texture2D>(buildingData.TexturePath);
        RefreshPosition(mousePos);
    }

    public void RefreshPosition(Vector2 mousePos)
    {
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(Type, mousePos);
        _canPlace = GameManager.Instance.BuildingManager.CanPlaced(Type, snapPos);
        if (_canPlace)
        {
            bool isInQiRange = false;
            foreach (MainBase mainBase in GameManager.Instance.MainBaseList)
            {
                if (snapPos.DistanceTo(mainBase.GlobalPosition) <= mainBase.QiRange)
                {
                    isInQiRange = true;
                    break; // 只要满足一个就可以跳出循环
                }
            }

            //// 如果主基地没满足，继续检查所有旗帜 (假设你的旗帜列表叫 FlagList，类型为 Flag)
            //if (!isInQiRange && GameManager.Instance.FlagList != null)
            //{
            //    foreach (Flag flag in GameManager.Instance.FlagList)
            //    {
            //        if (snapPos.DistanceTo(flag.GlobalPosition) <= flag.QiRange)
            //        {
            //            isInQiRange = true;
            //            break;
            //        }
            //    }
            //}
            _canPlace = isInQiRange;
        }

        GlobalPosition = snapPos;
        _sprite.Modulate = _canPlace ? new Color(0, 1, 0, 0.4f) : new Color(1, 0, 0, 0.4f);
    }

    public bool Build(Vector2 mousePos)
    {
        Vector2 snapPos = GameManager.Instance.BuildingManager.SnapToCell(Type, mousePos);
        //bool canPlace = GameManager.Instance.BuildingManager.CanPlaced(Type, snapPos);
        if (!_canPlace)
            return false;
        GameManager.Instance.BuildingManager.Place(Type, snapPos);

        BuildingData buildingData = BuildingDataManager.Instance.GetBuildingData(Type);
        switch (Type)
        {
            case BuildingType.MainBase:
                PackedScene mainBasePs = GD.Load<PackedScene>(buildingData.TscnPath);
                MainBase mainBase = mainBasePs.Instantiate<MainBase>();
                mainBase.Init(Type, GlobalPosition);//没有特殊功能的, 用buildingBase脚本, 走普通初始化函数
                GetTree().CurrentScene.AddChild(mainBase);
                break;
            case BuildingType.BuildingCraft:
                PackedScene buildingCraftPs = GD.Load<PackedScene>(buildingData.TscnPath);
                BuildingCraft buildingCraft = buildingCraftPs.Instantiate<BuildingCraft>();
                buildingCraft.Init(Type, GlobalPosition);//没有特殊功能的, 用buildingBase脚本, 走普通初始化函数
                GetTree().CurrentScene.AddChild(buildingCraft);
                break;
        }
        return true;
    }

    //private bool Check
}
