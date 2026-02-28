using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(
    fileName = "BulletFastUse",
    menuName = "FastUse/Bullet"
    )]
public class FastUseBullet : FastUseActionSO
{
    public override void Excute(GameObject player,ItemSO bullet)
    {
        Equipment equipment = player.GetComponent<Equipment>();
        if (equipment == null) {Debug.Log("no equipment"); ;return; }
        equipment.SetBullet(bullet);
        
    }
}
