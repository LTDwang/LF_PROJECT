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
        GameObject item = Instantiate(prefab, dropPoint.transform.position, Quaternion.identity);
        item.GetComponent<ItemCanPick>().item = itemSO; 
        SpriteRenderer sprit = item.GetComponent<SpriteRenderer>();
        sprit.sprite = itemSO.icon;
    }
}
