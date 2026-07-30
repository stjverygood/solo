using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.System.InventorySystem
{
    public class Inventory
    {
        public List<InventorySlot> SlotList = new List<InventorySlot>();
        public Action<int>? SlotChanged;//用于通知ui哪个格子变了, 修改ui格子数据

        public Inventory() { }


        public int AddItem(ItemInstance instance)
        {
            int remainCount = instance.Count;//记录当前剩余数量
            if (ItemDataManager.Instance.GetData(instance.Type).MaxCount != 1)//能堆叠, instance是可合并的
            {
                for (int i = 0; i < SlotList.Count; i++)//先遍历一次, 尝试合并
                {
                    ItemInstance curExistInstance = SlotList[i].ItemInstance;
                    if (curExistInstance == null || curExistInstance.Type != instance.Type)
                        continue;

                    if (curExistInstance.Count < ItemDataManager.Instance.GetData(instance.Type).MaxCount)//未满
                    {
                        int canAddCount = ItemDataManager.Instance.GetData(instance.Type).MaxCount - curExistInstance.Count;//能加的
                        int addCount = remainCount > canAddCount ? canAddCount : remainCount;//实际加的
                        remainCount -= addCount;
                        curExistInstance.Count += addCount;
                        SlotChanged?.Invoke(i);
                        if (remainCount == 0)
                            return instance.Count;
                    }
                }
                for (int i = 0; i < SlotList.Count; i++)//未能合并的, 遍历找到空格子, 新创建instance
                {
                    if (SlotList[i].ItemInstance != null)
                        continue;
                    SlotList[i].ItemInstance = new ItemInstance() { Type = instance.Type, Count = remainCount };
                    SlotChanged?.Invoke(i);
                    return instance.Count;
                }
            }
            else//不可堆叠
            {
                for (int i = 0; i < SlotList.Count; i++)
                {
                    if (SlotList[i].ItemInstance != null)
                        continue;
                    SlotList[i].ItemInstance = new ItemInstance() { Type = instance.Type, Count = instance.Count, CurDur = instance.CurDur };
                    SlotChanged?.Invoke(i);
                    return 1;
                }
            }
            return instance.Count - remainCount;
        }
        public int RemoveItem(ItemType itemType, int count)//返回移除了多少个
        {
            int remainCount = count;
            for (int i = 0; i < SlotList.Count; i++)
            {
                ItemInstance instance = SlotList[i].ItemInstance;
                if (instance == null || instance.Type != itemType)
                    continue;

                if (instance.Count > remainCount)
                {
                    instance.Count -= remainCount;
                    remainCount = 0;
                    SlotChanged?.Invoke(i);
                    break;
                }
                else
                {
                    remainCount -= instance.Count;
                    SlotList[i].ItemInstance = null;
                    SlotChanged?.Invoke(i);
                }
            }
            return count - remainCount;
        }
        public void RemoveItemByIndex(int index, int count)
        {
            SlotList[index].ItemInstance.Count -= count;
            if (SlotList[index].ItemInstance.Count == 0)
            {
                SlotList[index].ItemInstance = null;
                SlotChanged?.Invoke(index);
                return;
            }
            SlotChanged?.Invoke(index);
        }
        public int GetItemCount(ItemType itemType)//查询某个物品类型的总数量
        {
            int count = 0;
            foreach (InventorySlot slot in SlotList)
            {
                if (slot.ItemInstance == null)
                    continue;
                if (itemType == slot.ItemInstance.Type)
                {
                    count += slot.ItemInstance.Count;
                }
            }
            return count;
        }
        public void SwapItem(int sourceIndex, int targetIndex)
        {
            ItemInstance tempInstance = SlotList[sourceIndex].ItemInstance;
            SlotList[sourceIndex].ItemInstance = SlotList[targetIndex].ItemInstance;
            SlotList[targetIndex].ItemInstance = tempInstance;
            SlotChanged?.Invoke(sourceIndex);
            SlotChanged?.Invoke(targetIndex);
        }
        public ItemInstance GetItem(int index)
        {
            return SlotList[index].ItemInstance;
        }

        public bool CanSetSlot(int index, ItemInstance instance)
        {
            if (instance != null)
            {
                if (SlotList[index].Type == InventorySlotType.Helmet && instance.Type != ItemType.WoodHelmet && instance.Type != ItemType.IronHelmet && instance.Type != ItemType.GoldHelmet && instance.Type != ItemType.JadeHelmet)
                    return false;
                if (SlotList[index].Type == InventorySlotType.Armor && instance.Type != ItemType.WoodArmor && instance.Type != ItemType.IronArmor && instance.Type != ItemType.GoldArmor && instance.Type != ItemType.JadeArmor)
                    return false;
                if (SlotList[index].Type == InventorySlotType.Boot && instance.Type != ItemType.WoodBoot && instance.Type != ItemType.IronBoot && instance.Type != ItemType.GoldBoot && instance.Type != ItemType.JadeBoot)
                    return false;
            }
            return true;
        }
        public void SetSlot(int index, ItemInstance instance)
        {
            SlotList[index].ItemInstance = instance;
            SlotChanged?.Invoke(index);
        }
        public void ClearSlot(int index)
        {
            SlotList[index].ItemInstance = null;
            SlotChanged?.Invoke(index);
        }
    }
}
