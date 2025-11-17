using System;
using System.Collections.Generic;
using UnityEngine;

public class ManufactureGrid : MonoBehaviour
{
    [Header("材料区尺寸")]
    public int width = 3;
    public int height = 3;

    // 每个格子里存的是“占用这个格子”的 InventoryItem 引用
    private InventoryItem[,] cells;
    private List<InventoryItem> items = new List<InventoryItem>();

    // 材料区变化时回调（UI / Manager 用）
    public Action OnChanged;

    private void Awake()
    {
        cells = new InventoryItem[width, height];
    }

    public InventoryItem GetItemAt(int x, int y)
    {
        if (!InBounds(x, y)) return null;
        return cells[x, y];
    }

    public IEnumerable<InventoryItem> GetAllItems()
    {
        return items;
    }

    public void ClearAll()
    {
        cells = new InventoryItem[width, height];
        items.Clear();
        OnChanged?.Invoke();
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    // ========= 放置相关 =========

    public bool CanPlace(ItemSO item, int originX, int originY, bool rotated)
    {
        if (item == null) return false;

        int w = rotated ? item.gridHeight : item.gridWidth;
        int h = rotated ? item.gridWidth : item.gridHeight;

        // 1) 边界检查
        if (originX < 0 || originY < 0 ||
            originX + w > width || originY + h > height)
            return false;

        // 2) 检查覆盖区域是否都为空
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int gx = originX + x;
                int gy = originY + y;

                if (cells[gx, gy] != null)
                    return false;
            }
        }

        return true;
    }

    public InventoryItem PlaceItem(ItemSO item, int count, int originX, int originY, bool rotated)
    {
        if (!CanPlace(item, originX, originY, rotated)) return null;

        InventoryItem inst = new InventoryItem(item, count, originX, originY, rotated);

        items.Add(inst);

        int w = inst.Width;
        int h = inst.Height;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int gx = originX + x;
                int gy = originY + y;
                cells[gx, gy] = inst;
            }
        }

        OnChanged?.Invoke();
        return inst;
    }

    // ========= 删除相关 =========

    public void RemoveItem(InventoryItem inst)
    {
        if (inst == null) return;

        int w = inst.Width;
        int h = inst.Height;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int gx = inst.originX + x;
                int gy = inst.originY + y;

                if (InBounds(gx, gy) && cells[gx, gy] == inst)
                {
                    cells[gx, gy] = null;
                }
            }
        }

        items.Remove(inst);
        OnChanged?.Invoke();
    }

    // 从某个格子删掉“整件物品”
    public void RemoveItemAtCell(int x, int y)
    {
        if (!InBounds(x, y)) return;
        var inst = cells[x, y];
        if (inst != null)
        {
            RemoveItem(inst);
        }
    }
}
