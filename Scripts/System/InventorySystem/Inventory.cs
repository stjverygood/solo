using Solo.Scripts.System.ItemSystem;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.System.InventorySystem
{
    public class Inventory
    {
        private int _capacity;
        public List<ItemInstance> ItemInstanceList = new List<ItemInstance>();
        public Action<int> SlotChanged;//用于通知ui哪个格子变了, 修改ui格子数据

        public Inventory(int capacity)
        {
            _capacity = capacity;
            for (int i = 0; i < _capacity; i++)
            {
                ItemInstanceList.Add(null);
            }
        }

        public int AddItemInstance(ItemInstance instance)//自动添加, 比如捡东西, 双击其他背包的物品, 返回成功添加的物品数量
        {
            int remainCount = instance.Count;//记录当前剩余数量
            if (instance.Data.MaxCount != 1)//能堆叠, instance是可合并的
            {
                for (int i = 0; i < _capacity; i++)//先遍历一次, 尝试合并
                {
                    ItemInstance curExistInstance = ItemInstanceList[i];
                    if (curExistInstance == null || curExistInstance.Data.Type != instance.Data.Type)
                        continue;

                    if (curExistInstance.Count < instance.Data.MaxCount)//未满
                    {
                        int canAddCount = instance.Data.MaxCount - curExistInstance.Count;//能加的
                        int addCount = remainCount > canAddCount ? canAddCount : remainCount;//实际加的
                        remainCount -= addCount;
                        curExistInstance.Count += addCount;
                        SlotChanged?.Invoke(i);
                        if (remainCount == 0)
                            return instance.Count;
                    }
                }
                for (int i = 0; i < _capacity; i++)//未能合并的, 遍历找到空格子, 新创建instance
                {
                    if (ItemInstanceList[i] != null)
                        continue;
                    ItemInstanceList[i] = new ItemInstance() { Data = instance.Data, Count = remainCount };
                    SlotChanged?.Invoke(i);
                    return instance.Count;
                }
            }
            else//不可堆叠, 实例由外部已经new出来了, 这里浅拷贝就行
            {
                for (int i = 0; i < _capacity; i++)//未能合并的, 只能自己先创建instance
                {
                    if (ItemInstanceList[i] != null)
                        continue;
                    ItemInstanceList[i] = instance;
                    SlotChanged?.Invoke(i);
                    return 1;
                }
            }
            return instance.Count - remainCount;
        }

        public bool SetSlot(int index, ItemInstance instance)
        {
            ItemInstanceList[index] = instance;
            return true;
        }

        public void SwapItem(int sourceIndex, int targetIndex)
        {
            if (ItemInstanceList[sourceIndex] == null)
                return;

            if (ItemInstanceList[targetIndex] == null)// 目标格子为空，直接移动过去
            {
                ItemInstanceList[targetIndex] = ItemInstanceList[sourceIndex];
                ItemInstanceList[sourceIndex] = null;
                SlotChanged?.Invoke(sourceIndex);
                SlotChanged?.Invoke(targetIndex);
                return;
            }

            if (ItemInstanceList[sourceIndex].Data.Type != ItemInstanceList[targetIndex].Data.Type)// 两个物品类型不同，交互位置（单纯的对调）
            {
                ItemInstance temp = ItemInstanceList[sourceIndex];
                ItemInstanceList[sourceIndex] = ItemInstanceList[targetIndex];
                ItemInstanceList[targetIndex] = temp;
                SlotChanged?.Invoke(sourceIndex);
                SlotChanged?.Invoke(targetIndex);
            }
            else// 两个物品类型相同，尝试堆叠
            {
                int maxCount = ItemInstanceList[targetIndex].Data.MaxCount;
                int targetCount = ItemInstanceList[targetIndex].Count;
                int sourceCount = ItemInstanceList[sourceIndex].Count;
                int canAddCount = maxCount - targetCount;// 目标格子还能放多少个
                if (canAddCount > 0)
                {
                    int addCount = sourceCount > canAddCount ? canAddCount : sourceCount;// 实际能移动的数量
                    ItemInstanceList[targetIndex].Count += addCount;
                    ItemInstanceList[sourceIndex].Count -= addCount;
                    if (ItemInstanceList[sourceIndex].Count <= 0)
                        ItemInstanceList[sourceIndex] = null;
                    SlotChanged?.Invoke(sourceIndex);
                    SlotChanged?.Invoke(targetIndex);
                }
            }
        }
    }
}
