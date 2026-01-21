using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    [Header("背包ui")]
    public Button longWeaponButton;
    public Button shortWeaponButton;
    [Header("装备ui")]
    public Image longWeaponIcon;
    public Image shortWeaponIcon;
    [Header("其他系统")]
    public InventoryGridView InventoryGridView;
    public InventoryGrid InventoryGrid;

    [Header("逻辑装备")]
    public InventoryItem longWeapon;
    public InventoryItem shortWeapon;

    // Start is called before the first frame update
    void Start()
    {
        longWeaponButton.onClick.AddListener(OnLongWeaponButtonClicked);
        shortWeaponButton.onClick.AddListener(OnShortWeaponButtonClicked);
    }

    void OnLongWeaponButtonClicked()
    {
        if (!InventoryGridView.IsDraggingItem)
        {
            return;
        }
        InventoryItem item = InventoryGridView.DraggingItem;
        if (item.item.itemType!=ItemType.Weaoon)
        {
            Debug.Log("不是武器");
            return;
        }
        if (item == shortWeapon )
        {
            Debug.Log("这个装备在另外一个武器栏了");
            return;
        }
        longWeapon = item;
        SetLongIcon(item.item.icon);
        InventoryGridView.StopDrag();
        InventoryGrid.MoveItem(item, item.originX, item.originY, item.rotated);
        InventoryGridView.RefreshAllItems();
    }
    void OnShortWeaponButtonClicked()
    {
        if (!InventoryGridView.IsDraggingItem)
        {
            return;
        }
        InventoryItem item = InventoryGridView.DraggingItem;
        if (item.item.itemType != ItemType.Weaoon)
        {
            Debug.Log("不是武器");
            return;
        }
        if (item == longWeapon)
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
        longWeaponButton.image.sprite = sprite;
        longWeaponIcon.sprite = sprite;
    }
    public void SetShortIcon(Sprite sprite)
    {
        shortWeaponButton.image.sprite = sprite;
        shortWeaponIcon.sprite = sprite;
    }
}
