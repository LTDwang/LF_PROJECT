using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private GameObject attacker;
    [SerializeField] 
    private bool ifDestroy = true;
    [SerializeField] 
    private float lifeTime = 3f;
    [SerializeField] 
    private Vector2 dir;
    [SerializeField] 
    private float originalSpeed;
    [SerializeField] 
    private float damage;
    [SerializeField] 
    LayerMask hitMask;
    [SerializeField]
    float radius = 0.5f;
    [SerializeField]
    Vector2 speed;
    [SerializeField]
    float aliveTime = 0;

    public void Init(GameObject attacker,bool ifDestroy,float lifeTime,Vector2 dir,float originalSpeed,float damage,LayerMask hitMask)
    {
        this.attacker = attacker;
        this.ifDestroy = ifDestroy;
        this.lifeTime = lifeTime;
        this.dir = dir;
        this.originalSpeed = originalSpeed;
        this.damage = damage;
        this.hitMask = hitMask;
        speed = originalSpeed * dir;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        float deltaTime = GameManager.ScaledFixedDeltaTime;
        if (ifDestroy)
        {
            aliveTime += deltaTime;
            if (aliveTime>=lifeTime)
            {
                Destroy(gameObject);
            }
        }
        Vector2 pos = transform.position;
        Vector2 dis = speed * deltaTime;
        float dist = dis.magnitude;
        speed += new Vector2(0, -9.81f) * deltaTime;
        if (dist>0f)
        {
            RaycastHit2D hit = Physics2D.CircleCast(pos, radius, dis, dist, hitMask);
            if (hit.collider!=null)
            {
                if (attacker!= null&&hit.collider.transform.root != attacker)
                {
                   
                }
                else
                {
                    //怪物受击逻辑还没写
                    if (ifDestroy)
                    {
                        Destroy(gameObject);
                    }
                    transform.position = hit.point;
                }
            }
            transform.position = pos + dis;
        }
    }
    
}
