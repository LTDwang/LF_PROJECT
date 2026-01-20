using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuickWheelModel
{
    [SerializeField] private List<ItemSO> boundItems = new List<ItemSO>();

    public int SlotCount { get; private set; }

    public QuickWheelModel(int slotCount)
    {
        SlotCount = Mathf.Clamp(slotCount, 4, 12);
        EnsureSize();
    }

    public void SetSlotCount(int slotCount)
    {
        SlotCount = Mathf.Clamp(slotCount, 4, 12);
        EnsureSize();
    }

    public ItemSO GetItem(int index)
    {
        EnsureSize();
        if (index < 0 || index >= boundItems.Count) return null;
        return boundItems[index];
    }

    public void SetItem(int index, ItemSO item)
    {
        EnsureSize();
        if (index < 0 || index >= boundItems.Count) return;
        boundItems[index] = item;
    }

    private void EnsureSize()
    {
        while (boundItems.Count < SlotCount) boundItems.Add(null);
        if (boundItems.Count > SlotCount) boundItems.RemoveRange(SlotCount, boundItems.Count - SlotCount);
    }
}
