using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManagement : MonoBehaviour//用来记录目前地图中有多少生成的对应item并控制是否能继续生成，主要应用于永久采集物
{
    #region 单例内容
    private static ItemManagement itemManagement;
    public static ItemManagement Instance
    {
        get
        {
            if (itemManagement != null)
            {
                return itemManagement;
            }
            itemManagement = FindObjectOfType<ItemManagement>();
            if (itemManagement!=null)
            {
                return itemManagement;
            }
            GameObject _object = new GameObject("ItemManagement");
            itemManagement = _object.AddComponent<ItemManagement>();
            return itemManagement;
        }
    }
    #endregion
    [SerializeField]
    private Dictionary<string, int> itemCount = new Dictionary<string, int>();//用来记录目前地图中有多少生成的对应item
    public int maxCount = 5;//地图中单一物品最大数量
    public void ProductItem(string name,int count=1)
    {
        if (itemCount.ContainsKey(name))
        {
            itemCount[name] += count;
        }
        else
        {
            itemCount[name] = count;
        }
        
    }
    public void CustomItem(string name, int count = 1)
    {
        if (itemCount.ContainsKey(name))
        {
            itemCount[name] -= count;
            if (itemCount[name]==0)
            {
                itemCount.Remove(name);
            }
        }
        Debug.Log("Customed");
    }
    public bool CanStillProduce(string name)
    {
        if (!itemCount.ContainsKey(name))
        {
            return true;
        }
        if (itemCount[name]<maxCount)
        {
            return true;
        }
        return false;
    }
}
