using Solo.Scripts.Global;
using Solo.Scripts.System.ItemSystem;
using System;
using System.Collections.Generic;

namespace Solo.Scripts.System.InventorySystem
{
    public class Inventory
    {
        public int Capacity = 16;
        public List<ItemInstance> ItemInstanceList = new List<ItemInstance>();
        public Action<int> SlotChanged;//用于通知ui哪个格子变了, 修改ui格子数据
        
        public Inventory()
        {
            for(int i = 0; i < Capacity; i++)
            {
                ItemInstanceList.Add(null);
            }
        }

        public int AddItemInstance(ItemInstance instance, int count)
        {
            if (instance == null || count <= 0) return 0;

            int remainingCount = count; // 记录还剩下多少数量没放进去

            // ==========================================
            // 阶段 1：如果是可堆叠物品，先尝试往【已有同类物品且未满】的格子里塞
            // ==========================================
            if (instance.Data.MaxCount > 1)
            {
                for (int i = 0; i < Capacity; i++)
                {
                    ItemInstance curExistInstance = ItemInstanceList[i];
                    if (curExistInstance == null) continue;

                    // 检查类型是否相同
                    if (curExistInstance.Data.Type != instance.Data.Type) continue;

                    // 检查是否未满
                    if (curExistInstance.Count < curExistInstance.Data.MaxCount)
                    {
                        int canAddCount = curExistInstance.Data.MaxCount - curExistInstance.Count;
                        int addCount = remainingCount > canAddCount ? canAddCount : remainingCount;

                        curExistInstance.Count += addCount;
                        remainingCount -= addCount;

                        SlotChanged?.Invoke(i); // 数据变了，立刻通知 UI

                        if (remainingCount <= 0)
                            return count; // 全部塞完了，实际放入数量 = 申请加入的总数
                    }
                }
            }

            // ==========================================
            // 阶段 2：如果还没塞完（或者不可堆叠），寻找【空格子】放入
            // ==========================================
            for (int i = 0; i < Capacity; i++)
            {
                if (ItemInstanceList[i] != null) continue; // 不是空格子，跳过

                if (instance.Data.MaxCount > 1)
                {
                    // 可堆叠物品：创建新实例，塞入当前剩余的数量（最大不超过 MaxCount）
                    int addCount = remainingCount > instance.Data.MaxCount ? instance.Data.MaxCount : remainingCount;

                    ItemInstanceList[i] = new ItemInstance() { Data = instance.Data, Count = addCount };
                    remainingCount -= addCount;

                    SlotChanged?.Invoke(i);

                    if (remainingCount <= 0)
                        return count; // 全部塞完了
                }
                else
                {
                    // 不可堆叠物品（如装备）：每次只能占一个空格子，且数量永远是 1
                    ItemInstanceList[i] = instance;
                    remainingCount--; // 进格子一个，剩余申请量减 1

                    SlotChanged?.Invoke(i);

                    if (remainingCount <= 0)
                        return count;
                }
            }

            // ==========================================
            // 阶段 3：结算
            // ==========================================
            // 实际放入的数量 = 初始总数 - 最终剩下没塞进去的数量
            return count - remainingCount;
        }

        public bool SetSlot(int index, ItemInstance instance)
        {
            ItemInstanceList[index] = instance;
            return true;
        }

        public void SwapItem(int sourceIndex, int targetIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= ItemInstanceList.Count || targetIndex < 0 || targetIndex >= ItemInstanceList.Count)
                return;
            if (sourceIndex == targetIndex) 
                return;
            if (ItemInstanceList[sourceIndex] == null) 
                return;

            // 目标格子为空，直接移动过去
            if (ItemInstanceList[targetIndex] == null)
            {
                ItemInstanceList[targetIndex] = ItemInstanceList[sourceIndex];
                ItemInstanceList[sourceIndex] = null;
                SlotChanged?.Invoke(sourceIndex);
                SlotChanged?.Invoke(targetIndex);
                return;
            }

            // 到这一步，说明目标格子【一定有东西】了，可以安全地读取 Data.Type
            // 两个物品类型不同，交互位置（单纯的对调）
            if (ItemInstanceList[sourceIndex].Data.Type != ItemInstanceList[targetIndex].Data.Type)
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
