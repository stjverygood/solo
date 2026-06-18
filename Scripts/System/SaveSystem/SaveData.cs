using Solo.Scripts.System.ItemSystem;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.System.SaveSystem
{
    public struct Int2
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Int2(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class SaveData
    {
        //纯数据类, 记录一个存档的全部信息
        public string WorldSeedStr { get; set; }

        public PlayerSaveData PlayerSaveData { get; set; }

        public string BagInventoryGuidStr { get; set; }
        public List<ItemInstance> BagInventoryList { get; set; }
        public string FastBarInventoryGuidStr { get; set; }
        public int FastBarIndex { get; set; }
        public List<ItemInstance> FastBarInventoryList { get; set; }

        //public Dictionary<Int2, ChunkSaveData> ChunkDataMap { get; set; }//所有加载过的区块
        public List<ChunkSaveData> ChunkSaveDataList { get; set; }//所有加载过的区块列表, 加载存档时要转成Dictionary<Int2, ChunkSaveData>

        public SaveData()
        {
            WorldSeedStr = "test_seed_3";
            ChunkSaveDataList = new List<ChunkSaveData>();

            PlayerSaveData = new PlayerSaveData();

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
            FastBarIndex = 0;
        }
    }


}
