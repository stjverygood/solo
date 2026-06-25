namespace Solo.Scripts.Global
{
    public enum GameState
    {
        StartMenu,//开始菜单
        SaveListMenu,//世界列表
        SettingMenu,
        Loading,//加载界面 : 加载世界, 退出并保存世界
        Play,//游戏中
        Pause,//按exc暂停游戏
    }

    //地形类型
    public enum TileType
    {
        Grass,
        Water,
        Stone,
    }

    //单位类型
    public enum UnitType
    {
        Wolf,//风狼
        Fox,
    }

    public enum ArmorSlotType
    {
        Helmet,
        Armor,
        Boot,
    }

    public enum SelfViewTarget
    {
        EquipmentView,
        BasicCraftView,
        OtherCraftView,
    }

    public enum TargetType
    {
        DropItem,

        Player,

        //单位
        Wolf,
        Fox,

        //建筑
        Tree,
        Stone,
        MainBase,
        Flag,//法旗
        BuildingCraft,//建筑合成台
        ToolCraft,//工具合成台
        ArmorCraft,//防具合成台
        Crucible,//熔炉
        ItemBox,//储物箱
    }

    //建筑类型
    public enum BuildingType
    {
        Water,//水上无法拍建筑
        Tree,
        Stone,
        MainBase,//聚灵之源
        Flag,//法旗
        BuildingCraft,//建筑合成台
        ToolCraft,//工具合成台
        ArmorCraft,//防具合成台
        Crucible,//熔炉
        ItemBox,//储物箱
        TreeGrow,//树苗
    }

    //记录全游戏物品的type
    public enum ItemType
    {
        MainBaseStone,//太古源石, 用于合成主基地
        Silk,//丝绸
        Leather,//皮革, 这里可以拓展成各种皮革, 各种妖兽皮
        Grass,//草
        Rope,//绳子
        Stone,//石头


        Wood,//木头
        IronRaw,//粗铁
        Iron,//钢, 乌黑色
        CopperRaw,//粗铜
        Copper,//铜
        SilverRaw,//粗银
        Silver,//银, 银白色
        Gold,//金, 直接捡的, 不用冶炼

        Arrow,//箭, 后面考虑是否要区分等级
        Fireball,

        Banana,


        //灵木->玄铁->庚金->仙玉
        WoodSword,  // 木剑
        WoodBow,    // 木弓
        WoodPickaxe,// 木镐
        WoodAxe,    // 木斧
        WoodPot,     // 木壶
        WoodRod,//木鱼竿
        WoodHelmet, // 木盔, 防具影响移动速度
        WoodArmor,  // 木甲
        WoodBoot,   // 木鞋

        IronSword,   // 铁剑
        IronBow,     // 铁弓
        IronPickaxe, // 铁镐
        IronAxe,     // 铁斧
        IronPot,     // 铁壶
        IronRod,//铁鱼竿
        IronHelmet,  // 铁盔
        IronArmor,   // 铁甲
        IronBoot,    // 铁鞋

        GoldSword,   // 金剑
        GoldBow,     // 金弓
        GoldPickaxe, // 金镐
        GoldAxe,     // 金斧
        GoldPot,     // 金壶
        GoldRod,//金鱼竿
        GoldHelmet,  // 金盔
        GoldArmor,   // 金甲
        GoldBoot,    // 金鞋

        JadeSword,   // 玉剑
        JadeBow,     // 玉弓
        JadePickaxe, // 玉镐
        JadeAxe,     // 玉斧
        JadePot,     // 玉壶
        JadeRod,//玉鱼竿
        JadeHelmet,  // 玉盔
        JadeArmor,   // 玉甲
        JadeBoot,    // 玉鞋

        //建筑物品
        MainBase,//聚灵之源
        Flag,//法旗
        BuildingCraft,//建筑合成台
        ToolCraft,//工具合成台
        ArmorCraft,//防具合成台
        MagicCraft,//法术合成台
        Crucible,//熔炉
        ItemBox,//储物箱
        TreeGrow,//树苗
    }

    //public enum ProjectileType
    //{
    //    Arrow,
    //    Fireball,
    //}
}
