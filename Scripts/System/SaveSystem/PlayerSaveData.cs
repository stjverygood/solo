using Solo.Scripts.Global;
using Solo.Scripts.System.InventorySystem;
using Solo.Scripts.System.RealmSystem;
using System.Collections.Generic;

namespace Solo.Scripts.System.SaveSystem
{
    public class PlayerSaveData
    {
        public float StartPosX { get; set; } = 0;//出生位置, 默认是(0, 0), 只能靠主基地重置
        public float StartPosY { get; set; } = 0;
        public float PosX { get; set; } = 0;//玩家上次位置
        public float PosY { get; set; } = 0;
        //public float MaxHp { get; set; } = 100;//血量

        //public float MaxMp { get; set; } = 100;//饥饿值
        //public float CurMp { get; set; } = 100;

        public RealmType CurRealmType { get; set; }
        public float CurHp { get; set; }
        public float CurQi { get; set; }
        public float CurExp { get; set; }

        public int FastBarIndex { get; set; }

        public List<InventorySlot> FastBarInventorySlotList { get; set; }
        public List<InventorySlot> BagInventorySlotList { get; set; }
        public List<InventorySlot> EquipmentInventorySlotList { get; set; }

        public PlayerSaveData()
        {
            CurRealmType = RealmType.LianQi1;
            CurHp = RealmDataManager.Instance.GetData(CurRealmType).MaxHp;
            CurQi = RealmDataManager.Instance.GetData(CurRealmType).MaxQi;
            CurExp = 0;

            FastBarIndex = 0;

            FastBarInventorySlotList = new List<InventorySlot>(4);
            for (int i = 0; i < 4; i++)
            {
                FastBarInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Common });
            }


            BagInventorySlotList = new List<InventorySlot>(16);
            for (int i = 0; i < 16; i++)
            {
                BagInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Common });
            }

            EquipmentInventorySlotList = new List<InventorySlot>(3);
            EquipmentInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Helmet });
            EquipmentInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Armor });
            EquipmentInventorySlotList.Add(new InventorySlot() { Type = InventorySlotType.Boot });
        }
    }
}
