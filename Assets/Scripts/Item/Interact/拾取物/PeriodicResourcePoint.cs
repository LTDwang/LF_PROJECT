using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicResourcePoint : PermanentResourcePoint
{
    [Header("冷却时间消失还是变化形态")]
    public bool disappear = false;
    [Header("冷却时显示的对象")]
    public GameObject growing;
    [Header("自身sprit")]
    public SpriteRenderer sprite;
    public GameObject mature;//判定框
    [Header("生长一次能采集几次")]
    public int maxCount = 1;
    [Header("冷却时间")]
    public float coolDownTime = 5f;
    [Header("已冷却时间")]
    public float time;
    [Header("正在冷却？")]
    public bool cooling = false;
    [SerializeField]
    private int availibleCount;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        availibleCount = maxCount;
        growing.SetActive(false);
        sprite.enabled = true;
        mature.SetActive(true);
        time = coolDownTime;
    }
    private void Update()
    {
        if (cooling)
        {
            time += GameManager.ScaledDeltaTime;
            if (time>= coolDownTime)
            {
                Mature();
            }
        }
    }
    protected override void Produce()
    {
        if (cooling)
        {
            return;
        }
        GameObject produce;
        foreach (ItemSO itemSO in products)
        {
            if (!ItemManagement.Instance.CanStillProduce(itemSO.id))
            {
                continue;
            }
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
            ItemManagement.Instance.ProductItem(itemSO.id);
        }
        availibleCount--;
        if (availibleCount<=0)
        {
            sprite.enabled = false;
            mature.SetActive(false);
            if (!disappear)
            {
                growing.SetActive(true);
            }
            time = 0;
            cooling = true;
        }
    }
    public void Mature()
    {
        sprite.enabled = true;
        mature.SetActive(true);
        growing.SetActive(false);
        time = coolDownTime;
        cooling = false;
        availibleCount = maxCount;
    }
}
