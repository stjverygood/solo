namespace Solo.Scripts.Global
{
    public enum GameState
    {
        StartMenu,//开始菜单
        WorldListMenu,//世界列表
        SettingMenu,
        Loading,//加载界面 : 加载世界, 退出并保存世界
        Play,//游戏中
        Stop,//按exc暂停游戏
    }

    public enum TileType
    {
        Grass,
        Water,
        Dirt,
    }

    public enum WorldObjectType
    {
        Tree,
        Stone,
    }

    //记录全游戏物品的type
    public enum ItemType
    {
        Wood,       // 木头
        Stone,      // 石头
        Iron,       // 铁
        Gold,       // 金

        WoodSword,  // 木剑
        WoodBow,    // 木弓
        WoodPickaxe,// 木镐
        WoodAxe,    // 木斧
        PotAxe,     // 木壶
        WoodHelmet, // 木盔
        WoodArmor,  // 木甲
        WoodBoot,   // 木鞋

        StoneSword,  // 石剑
        StoneBow,    // 石弓
        StonePickaxe,// 石镐
        StoneAxe,    // 石斧
        StonePot,    // 石壶
        StoneHelmet, // 石盔
        StoneArmor,  // 石甲
        StoneBoot,   // 石鞋

        IronSword,   // 铁剑
        IronBow,     // 铁弓
        IronPickaxe, // 铁镐
        IronAxe,     // 铁斧
        IronPot,     // 铁壶
        IronHelmet,  // 铁盔
        IronArmor,   // 铁甲
        IronBoot,    // 铁鞋

        GoldSword,   // 金剑
        GoldBow,     // 金弓
        GoldPickaxe, // 金镐
        GoldAxe,     // 金斧
        GoldPot,     // 金壶
        GoldHelmet,  // 金盔
        GoldArmor,   // 金甲
        GoldBoot,    // 金鞋


    }
}
