using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(
    fileName = "NewFarAttack",
    menuName = "AttackAction/FarAttack",
    order = 1
    )]
public class FarAttack : AttackActionSO
{
    public override void Excute(ItemSO weaponItem, NearAttackContext ctx)
    {
        throw new System.NotImplementedException();
    }

    public override void Excute(ItemSO bullet,ItemSO weaponItem, FarAttackContext ctx, GameObject prefeb )
    {
        
        if (prefeb == null)
        {
            return;
        }
        var go = Instantiate(prefeb, ctx.startpoint.position,Quaternion.identity);
        Bullet _bullet = go.GetComponent<Bullet>();
        if (_bullet!=null)
        {
            _bullet.Init(bullet,ctx.attacker, bullet.ifDestroy, bullet.lifeTime, ctx.direction, ctx.flyingSpeed, bullet.damage, ctx.hitMask);
        }
    }
}
