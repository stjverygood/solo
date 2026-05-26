using Solo.Scripts.System.ItemSystem;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.System.SaveSystem
{
    public class SaveData
    {
        //纯数据类, 记录一个存档的全部信息
        public string WorldSeedStr { get; set; }

        public float PlayerPosX { get; set; }
        public float PlayerPosY { get; set; }
        public string BagInventoryGuidStr { get; set; }
        public List<ItemInstance> BagInventoryList { get; set; }
        public string FastBarInventoryGuidStr { get; set; }
        public List<ItemInstance> FastBarInventoryList { get; set; }

        public SaveData()
        {
            WorldSeedStr = "test_seed_3";

            PlayerPosX = 0;
            PlayerPosY = 0;

            BagInventoryGuidStr = Guid.NewGuid().ToString();
            BagInventoryList = new List<ItemInstance>(16);
            for (int i = 0; i < 16; i++)
            {
                BagInventoryList.Add(null);
            }

            FastBarInventoryGuidStr = Guid.NewGuid().ToString();
            FastBarInventoryList = new List<ItemInstance>(4);
            for (int i = 0; i < 4; i++)
            {
                FastBarInventoryList.Add(null);
            }
        }
    }


}
