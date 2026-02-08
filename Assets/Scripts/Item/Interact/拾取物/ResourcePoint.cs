using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourcePoint : MonoBehaviour, IInteractable//用于制作采集物
{
    InteractableTrigger trigger;
    public List<ItemSO> products = new List<ItemSO>(); // 交互后会产生的产物
    public string prompt = "按F交互";
    public string Prompt => prompt;
    public bool canInteract = true;
    public Transform _player;
    public int force = 100;
    public GameObject prefeb;
    private void OnEnable()
    {
        trigger = GetComponentInChildren<InteractableTrigger>();
        if (trigger) trigger.enabled = true;
    }

    private void OnDisable()
    {
        trigger = GetComponentInChildren<InteractableTrigger>();
        if (trigger) trigger.enabled = false;
    }
    public bool CanInteract(Transform player) => canInteract;

    public void ClearPlayer(Transform player)
    {
        if (player == _player)
        {
            _player = null;
        }
    }

    public virtual void Interact(Transform player)
    {
        Debug.Log("交互了");
        Produce();
        Destroy(gameObject);
        
    }

    public void SetPlayer(Transform player)
    {
        _player = player;
    }

    protected virtual void Produce()
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
            produce = GameObject.Instantiate(prefeb,producePoint,Quaternion.identity);
            produce.transform.SetParent(null);
            float x, y;
            x = Random.Range(-1.0f, 1.0f);
            y = Random.Range(0f, 1f);
            Vector2 dic = new Vector2(x, y);
            produce.GetComponent<Rigidbody2D>().isKinematic = false;
            Debug.Log("Set False");
            produce.GetComponent<Rigidbody2D>().velocity=dic.normalized * 2.5f;
            produce.GetComponent<ItemCanPick>().item = itemSO;
            produce.GetComponent<ItemCanPick>().SetFigure();
            Debug.Log(produce.GetComponent<Rigidbody2D>().velocity);
            //ItemManagement.Instance.ProductItem(itemSO.id);
        }
    }
}
