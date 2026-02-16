using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletRender : MonoBehaviour
{
    public Image bulletSprite;
    public Text bulletCount;
     
    public void BulletCountRefresh(int count)
    {
        if (count <= 0)
        {
            count = 0;
            bulletSprite.color = Color.gray;
        }
        else
        {
            bulletSprite.color = Color.white;
        }
        bulletCount.text = $"{count}";
    }
    public void BulletChangeRender(ItemSO bullet, int count)
    {
        if (bullet == null)
        {
            bulletSprite.sprite = null;
            BulletCountRefresh(0);
            return;
        }
        bulletSprite.sprite = bullet.icon;
        BulletCountRefresh(count);
    }
}
