using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    [Header("背包ui")]
    public Button farWeaponButton;
    public Button shortWeaponButton;
    [Header("装备ui")]
    public Image farWeaponIcon;
    public Image shortWeaponIcon;
    public Image bullet;

    [Header("其他系统")]
    public InventoryGridView InventoryGridView;
    public InventoryGrid InventoryGrid;
    public BulletRender bulletRender;

    [Header("逻辑装备")]
    public InventoryItem farWeapon;
    public InventoryItem shortWeapon;
    [Header("子弹")]
    public Dictionary<ItemSO, int> bulletsCount;
    public ItemSO currentBullet;

    [Header("现在在手上拿着的东西")]
    public ItemSO inHand;

    public ItemSO ItemInHand => inHand;
    public ItemSO CurrentBullet => currentBullet;
    void Start()
    {
        farWeaponButton.onClick.AddListener(OnfarWeaponButtonClicked);
        shortWeaponButton.onClick.AddListener(OnShortWeaponButtonClicked);
        bulletsCount = new Dictionary<ItemSO, int>();
        InventoryGrid.OnCountChange += RefreshBulletsCount;
    }

    void OnfarWeaponButtonClicked()
    {
        if (!InventoryGridView.IsDraggingItem)
        {
            return;
        }
        InventoryItem item = InventoryGridView.DraggingItem;
        if (item.item.itemType!=ItemType.FarWeapon)
        {
            Debug.Log("不是远程武器");
            return;
        }
        if (item == shortWeapon )
        {
            Debug.Log("这个装备在另外一个武器栏了");
            return;
        }
        farWeapon = item;
        Debug.Log($"装备{item.item.id}");
        SetLongIcon(item.item.icon);
        InventoryGridView.StopDrag();
        InventoryGrid.MoveItem(item, item.originX, item.originY, item.rotated);
        InventoryGridView.RefreshAllItems();
        bulletsCount.Clear();
        //统计各个子弹数量
        foreach (var bullet in farWeapon.item.validBullets)
        {
            if (bullet==null)
            {
                Debug.Log("null");
            }
            Debug.Log($"111{bullet.id}");
            if (!bulletsCount.ContainsKey(bullet))
            {
                bulletsCount.Add(bullet, 0);
            }
            bulletsCount[bullet] = InventoryGrid.GetTotalCount(bullet);
        }
        //设置默认子弹
        SelectDefaultBullet();
        bulletRender.BulletChangeRender(currentBullet, bulletsCount[currentBullet]);
    }
    void OnShortWeaponButtonClicked()
    {
        if (!InventoryGridView.IsDraggingItem)
        {
            return;
        }
        InventoryItem item = InventoryGridView.DraggingItem;
        if (item.item.itemType != ItemType.NearWeapon)
        {
            Debug.Log("不是近程武器");
            return;
        }
        if (item == farWeapon)
        {
            Debug.Log("这个装备在另外一个武器栏了");
            return;
        }
        shortWeapon = item;
        SetShortIcon(item.item.icon);
        InventoryGridView.StopDrag();
        InventoryGrid.MoveItem(item, item.originX, item.originY, item.rotated);
        InventoryGridView.RefreshAllItems();
    }
    public void SetLongIcon(Sprite sprite)
    {
        farWeaponButton.image.sprite = sprite;
        farWeaponIcon.sprite = sprite;
    }
    public void SetShortIcon(Sprite sprite)
    {
        shortWeaponButton.image.sprite = sprite;
        shortWeaponIcon.sprite = sprite;
    }
    public void SetBullet(ItemSO item)
    {
        if (item.itemType!= ItemType.Bullet)
        {
            return;
        }
        if (farWeapon == null)
        {
            Debug.Log("没装远程武器");
            return;
        }
        if (farWeapon.item != item.validWeapon)
        {
            Debug.Log("子弹不适配");
            return;
        }
        currentBullet = item;
        bulletRender.BulletChangeRender(currentBullet, bulletsCount[currentBullet]);
    }
    private void SelectDefaultBullet()
    {
        if (farWeapon == null)
        {
            return;
        }
        var valid = farWeapon.item.validBullets;
        if (valid == null || valid.Count == 0)
        {
            SetBullet(null);
            return;
        }

        //逻辑：优先沿用现用的弹种
        if (currentBullet!= null && valid.Contains(currentBullet) && bulletsCount.TryGetValue(currentBullet,out var keepCount) && keepCount > 0)
        {
            SetBullet(currentBullet);
            return;
        }
        //如果现用弹种打完了，检查后面的
        foreach (var item in valid)
        {
            if (bulletsCount.TryGetValue(item,out int count) && count > 0)
            {
                SetBullet(item);
                return;
            }
        }
        SetBullet(valid[0]);
    }
    void RefreshBulletsCount(ItemSO bullet)
    {
        if (bullet.itemType!=ItemType.Bullet||farWeapon.item==null||!farWeapon.item.validBullets.Contains(bullet))
        {
            return;
        }
        int count = InventoryGrid.GetTotalCount(bullet);
        bulletRender.BulletCountRefresh(count);
        bulletsCount[bullet] = count;
    }
   
}
