using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public bool doHasPosition;
    public int x;
    public int y;
    public bool rotated;
}
public class InventoryGrid : MonoBehaviour
{
    [Header("背包尺寸")]
    public int width = 6;
    public int height = 10;

    private InventoryItem[,] cells;
    [SerializeField]
    private List<InventoryItem> items = new List<InventoryItem>();
    [SerializeField]
    private Dictionary<ItemSO, int> itemCount = new Dictionary<ItemSO, int>();
    [SerializeField]
    private Dictionary<ItemSO,List<Position>> itemsPos = new Dictionary<ItemSO,List<Position>>();
    public InventoryGridView inventory;
    public IReadOnlyList<InventoryItem> Items => items;
    public int Width => width;
    public int Height => height;

    public event Action<ItemSO> OnCountChange;
    
    private void Start()
    {
        cells = new InventoryItem[width, height];
    }
    public InventoryItem GetItemAt(int x,int y)
    {
        if (!InBounds(x, y)) return null;
        return cells[x, y];
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
    
    //位置空余检查工具函数
    public bool CanPlace(ItemSO item, int x,int y, bool rotated, InventoryItem ignoreItem = null)
    {
        if (item == null)
        {
            return false;
        }
        int w = rotated ? item.gridHeight : item.gridWidth;
        int h = rotated ? item.gridWidth : item.gridHeight;

        if (x < 0 || y < 0 || x + w > width || y + h > height)
            return false;

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (cells[x+i,y+j]!=null && cells[x+i,y+j]!= ignoreItem)
                {
                    return false;
                }
            }
        }
        return true;
    }

    //为新放入背包的东西查找背包中可放入的位置
    public Position FindPostitionToPut(ItemSO item)
    {
        Position position = new Position();
        bool foundPlace = false;
        position.doHasPosition = foundPlace;
        bool rotated = false;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                foundPlace = CanPlace(item, i, j, rotated);
                if (foundPlace)
                {
                    position.x = i;
                    position.y = j;
                    position.doHasPosition = foundPlace;
                    position.rotated = rotated;
                    return position;
                }
            }
        }
        rotated = !rotated;
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                foundPlace = CanPlace(item, i, j, rotated);
                if (foundPlace)
                {
                    position.x = i;
                    position.y = j;
                    position.doHasPosition = foundPlace;
                    position.rotated = rotated;
                    return position;
                }
            }
        }
        return position;
    }
    //直接往背包中尝试放入实例；
    public InventoryItem PlaceNewItemWithNoPosition(ItemSO item)
    {
        Position position = FindPostitionToPut(item);
        if (!position.doHasPosition)
        {
            return null;
        }
        return PlaceNewItem(item, 1, position.x, position.y, position.rotated);
    }
    //背包中确定位置放入实例
    public InventoryItem PlaceNewItem(ItemSO item, int count, int x, int y, bool rotated)
    {
        if (!CanPlace(item,x,y,rotated))
        {
            return null;
        }
        InventoryItem inst = new InventoryItem(item, Mathf.Max(1, count), x, y, rotated);
        Debug.Log($"成功创建{inst.item.name}");
        items.Add(inst);
        Position position = new Position();
        position.x = x;
        position.y = y;
        position.rotated = rotated;
        position.doHasPosition = true;
        AddPosition(item, position);
        CountAdd(item);
        FillCells(inst, inst.originX, inst.originY, true);
        if (inventory!=null&&inventory.gameObject.activeSelf!=false)
            inventory.RefreshAllItems();
        return inst;
    }
    private void FillCells(InventoryItem inst, int originX, int originY, bool occupy)
    {
        if (inst == null) return;
        int w = inst.Width;
        int h = inst.Height;

        for (int ix = 0; ix < w; ix++)
        {
            for (int iy = 0; iy < h; iy++)
            {
                int gx = originX + ix;
                int gy = originY + iy;

                if (!InBounds(gx, gy)) continue;

                cells[gx, gy] = occupy ? inst : null;
            }
        }
    }
    public bool MoveItem(InventoryItem inst, int newX, int newY, bool newRotated)
    {
        if (inst==null)
        {
            return false;
        }
        FillCells(inst, inst.originX, inst.originY, false);
        bool canPlace = CanPlace(inst.item, newX, newY, newRotated, null);
        if (!canPlace)
        {
            FillCells(inst, inst.originX, inst.originY, true);
            return false;
        }
        inst.originX = newX;
        inst.originY = newY;
        inst.rotated = newRotated;
        FillCells(inst, inst.originX, inst.originY, true);
        items.Add(inst);
        CountAdd(inst.item);
        AddPosition(inst.item, new Position { doHasPosition = true, rotated = inst.rotated, x = inst.originX, y = inst.originY });
        return true;
    }
    public void RemoveItem(InventoryItem inst)
    {
        if (inst == null)
        {
            return;
        }

        CountConsume(inst.item);
        PosConsum(inst.item, new Position { doHasPosition = true, rotated = inst.rotated, x = inst.originX, y = inst.originY });
        FillCells(inst, inst.originX, inst.originY, false);
        items.Remove(inst);
        inventory.RefreshAllItems();
    }
    public void MovingItem(InventoryItem inst)
    {
        if (inst == null)
        {
            return;
        }
        FillCells(inst, inst.originX, inst.originY, false);
        items.Remove(inst);
        CountConsume(inst.item);
        Position position = new Position();
        position.x = inst.originX;
        position.y = inst.originY;
        position.doHasPosition = true;
        position.rotated = inst.rotated;
        PosConsum(inst.item, position);
    }

    public void CountAdd(ItemSO item, int amount =1)
    {
        if (itemCount.ContainsKey(item))
        {
            itemCount[item]+= amount;
        }
        else
        {
            itemCount.Add(item, amount);
        }
        OnCountChange?.Invoke(item);
    }
    public bool CountConsume(ItemSO item, int amount = 1)
    {
        if (!itemCount.ContainsKey(item))
        {
            return false;
        }
        itemCount[item]-= amount;
        if (itemCount[item]<=0)
        {
            Debug.Log("用完了");
            itemCount.Remove(item);
        }
        OnCountChange?.Invoke(item);
        return true;
    }

    public int GetTotalCount(ItemSO item)
    {
        if (itemCount.ContainsKey(item))
        {
            return itemCount[item];
        }
        return 0;
    }

    public void AddPosition(ItemSO item, Position position)
    {
        if (itemsPos.ContainsKey(item))
        {
            itemsPos[item].Add(position);
        }
        else
        {
            List<Position> positions = new List<Position>();
            positions.Add(position);
            itemsPos.Add(item, positions);
        }
    }
    public void PosConsum(ItemSO item,Position position)
    {
        itemsPos[item].Remove(position);
        if (itemsPos[item].Count==0)
        {
            itemsPos.Remove(item);
        }
    }

    public Position GetPosOFItem(ItemSO item)
    {
        if (itemsPos.ContainsKey(item))
        {
            return itemsPos[item][0];
        }
        return null;
    }

    public bool TryConsumeOne(ItemSO item)
    {
        if (item == null) return false;
        if (GetTotalCount(item) <= 0) return false;

        // 1) 从 itemsPos 找一个该物品的位置
        if (!itemsPos.ContainsKey(item) || itemsPos[item].Count == 0) return false;
        var pos = itemsPos[item][0];

        // 2) 找到对应 InventoryItem 实例（originX/Y/rotated 匹配）
        InventoryItem inst = null;
        for (int i = 0; i < items.Count; i++)
        {
            var it = items[i];
            if (it != null && it.item == item &&
                it.originX == pos.x && it.originY == pos.y && it.rotated == pos.rotated)
            {
                inst = it;
                break;
            }
        }
        if (inst == null) return false;

        // 3) 移除实例（RemoveItem 会把四套数据一起同步掉）
        RemoveItem(inst);
        return true;
    }

}
