using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackActionSO : ScriptableObject
{
    public abstract void Excute(ItemSO weaponItem, NearAttackContext ctx);
    public abstract void Excute(ItemSO bullet,ItemSO weaponItem, FarAttackContext ctx, GameObject prefeb);
}
public struct FarAttackContext
{
    public GameObject attacker;
    public Vector2 direction;
    public LayerMask hitMask;
    public Transform startpoint;
    public float flyingSpeed;
    public ItemSO bullet;
}
public struct NearAttackContext
{
    public Vector2 origin;
    public Vector2 direction;
    public LayerMask hitMask;
}
