using Solo.Scripts.Entities.Core;
using Solo.Scripts.Levels;
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
        public string ChunkElevationNoiseSeedStr { get; set; }
        public string ChunkMoistureNoiseSeedStr { get; set; }
        public EntitySaveData? PlayerSaveData { get; set; }

        //public PlayerSaveData PlayerSaveData { get; set; }
        public List<ChunkSaveData> ChunkSaveDataList { get; set; }//所有加载过的区块列表, 加载存档时要转成Dictionary<Int2, ChunkSaveData>

        public List<Int2> InitedChunkPosList { get; set; }
        public List<ChunkEntitySaveData> ChunkEntitySaveDataList { get; set; }

        //public List<(int, int)> 

        public SaveData()
        {
            ChunkElevationNoiseSeedStr = "ChunkElevationNoiseSeedStr4";
            ChunkMoistureNoiseSeedStr = "ChunkMoistureNoiseSeedStr4";
            ChunkSaveDataList = new List<ChunkSaveData>();

            //PlayerData playerData = (PlayerData)EntityDataManager.Instance.GetData(EntityType.Player);
            //PlayerSaveData = new PlayerSaveData()
            //{

            //};
            //PlayerSaveData = new PlayerSaveData();
            ChunkEntitySaveDataList = new List<ChunkEntitySaveData>();
        }
    }


}
