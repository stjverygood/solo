using Solo.Scripts.Entities.Core;
using Solo.Scripts.Global;
using Solo.Scripts.Global.Interfaces;
using Solo.Scripts.System.InventorySystem;
using Solo.Scripts.System.ItemSystem;

namespace Solo.Scripts.Entities.Components
{
    public class InventoryComponent : Component
    {
        public Inventory FastBarInventory = null!;//快捷栏
        public Inventory BagInventory = null!;//背包
        public Inventory EquipmentInventory = null!;//装备栏

        public InventoryComponent(IEntity owner) : base(owner) { }

        public void SwapItem(Inventory sourceInventory, int sourceIndex, Inventory targetInventory, int targetIndex)
        {
            ItemInstance sourceItem = sourceInventory.GetItem(sourceIndex);
            ItemInstance targetItem = targetInventory.GetItem(targetIndex);
            if (!sourceInventory.CanSetSlot(sourceIndex, targetItem) ||
                !targetInventory.CanSetSlot(targetIndex, sourceItem))
            {
                return;
            }
            sourceInventory.SetSlot(sourceIndex, targetItem);
            targetInventory.SetSlot(targetIndex, sourceItem);
        }

        public void RemoveItem(Inventory sourceInventory, int sourceIndex)
        {
            sourceInventory.ClearSlot(sourceIndex);
        }

        public ItemType? GetCurItemType(int index)
        {
            ItemInstance itemInstance = FastBarInventory.GetItem(index);
            return itemInstance?.Type;
        }

        public int GetItemCount(ItemType itemType)
        {
            return FastBarInventory.GetItemCount(itemType) + BagInventory.GetItemCount(itemType);
        }

        public int AddItem(ItemInstance itemInstance)
        {
            itemInstance.Count -= FastBarInventory.AddItem(itemInstance);//优先添加到快捷栏
            if (itemInstance.Count != 0)//有剩余就添加到背包
            {
                itemInstance.Count -= BagInventory.AddItem(itemInstance);
            }
            return itemInstance.Count;
        }
        public int RemoveItem(ItemType type, int count)
        {
            int remainingCount = count; // 记录还需要扣除多少个
            remainingCount -= BagInventory.RemoveItem(type, remainingCount);
            if (remainingCount > 0)
            {
                remainingCount -= FastBarInventory.RemoveItem(type, remainingCount);
            }
            return remainingCount;
        }


        public float GetAtkBonus()
        {
            int atkBonus = 0;
            for (int i = 0; i < EquipmentInventory.SlotList.Count; i++)
            {
                if (EquipmentInventory.SlotList[i].ItemInstance == null)
                    continue;
                atkBonus += ItemDataManager.Instance.GetData(EquipmentInventory.SlotList[i].ItemInstance.Type).AtkBonus;
            }
            return atkBonus;
        }
        public int GetDefBonus()
        {
            int defBonus = 0;
            for (int i = 0; i < EquipmentInventory.SlotList.Count; i++)
            {
                if (EquipmentInventory.SlotList[i].ItemInstance == null)
                    continue;
                defBonus += ItemDataManager.Instance.GetData(EquipmentInventory.SlotList[i].ItemInstance.Type).DefBonus;
            }
            return defBonus;
        }
        public float GetMaxHpBonus()
        {
            float maxHpBonus = 0;
            for (int i = 0; i < EquipmentInventory.SlotList.Count; i++)
            {
                if (EquipmentInventory.SlotList[i].ItemInstance == null)
                    continue;
                maxHpBonus += ItemDataManager.Instance.GetData(EquipmentInventory.SlotList[i].ItemInstance.Type).MaxHpBonus;
            }
            return maxHpBonus;
        }
        public float GetMaxQiBonus()
        {
            float maxQiBonus = 0;
            for (int i = 0; i < EquipmentInventory.SlotList.Count; i++)
            {
                if (EquipmentInventory.SlotList[i].ItemInstance == null)
                    continue;
                maxQiBonus += ItemDataManager.Instance.GetData(EquipmentInventory.SlotList[i].ItemInstance.Type).MaxQiBonus;
            }
            return maxQiBonus;
        }
    }
}
