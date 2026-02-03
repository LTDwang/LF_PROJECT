using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropSystem : MonoBehaviour
{
    public GameObject dropPoint;
    public GameObject prefab;

    public void Drop(ItemSO itemSO)
    {
        ItemCanPick item = Instantiate(prefab, dropPoint.transform.position, Quaternion.identity).GetComponent<ItemCanPick>();
        item.item = itemSO;
        item.SetFigure();
    }
}
