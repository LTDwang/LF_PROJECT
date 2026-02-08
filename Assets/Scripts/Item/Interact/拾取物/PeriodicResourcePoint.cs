using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicResourcePoint : PermanentResourcePoint
{
    [Header("交互后消失还是变化形态")]
    public bool disappear = false;
    //[Header("变化形态的话要设置")]
    private void Update()
    {
        
    }
    protected override void Produce()
    {
        GameObject produce;
        foreach (ItemSO itemSO in products)
        {
            /*if (!ItemManagement.Instance.CanStillProduce(itemSO.id))
            {
                continue;
            }*/ //一般拾取物不做限制
            Vector2 producePoint = new Vector2();
            producePoint.x = transform.position.x;
            producePoint.y = transform.position.y + 0.5f;
            produce = GameObject.Instantiate(prefeb, producePoint, Quaternion.identity);
            produce.transform.SetParent(null);
            float x, y;
            x = Random.Range(-1.0f, 1.0f);
            y = Random.Range(0f, 1f);
            Vector2 dic = new Vector2(x, y);
            produce.GetComponent<Rigidbody2D>().isKinematic = false;
            Debug.Log("Set False");
            produce.GetComponent<Rigidbody2D>().velocity = dic.normalized * 2.5f;
            produce.GetComponent<ItemCanPick>().item = itemSO;
            produce.GetComponent<ItemCanPick>().SetFigure();
            Debug.Log(produce.GetComponent<Rigidbody2D>().velocity);
            //ItemManagement.Instance.ProductItem(itemSO.id);
        }
    }
}
