using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickWheelSetSlot : MonoBehaviour
{
    public int index;
    public Image image;
    public ItemSO itemSO;
    public QuickWheelController controller;
    Button button;

    public void HandleSlotClicked()
    {
        // dragging item -> bind to slot
        if (controller.IsDraggingToBind())
        {
            var so = controller.inventoryGridView.DraggingItem;
            if(!so.item.canBeFastUse)
            {
                Debug.Log("这玩意不能快捷使用");
                return;
            }
            itemSO = so.item;
            if (itemSO == null)
            {
                image.sprite = null;
                image.gameObject.SetActive(false);
            }
            else
            {
                image.sprite = itemSO.icon;
                image.gameObject.SetActive(true);
            }
            controller._model.SetItem(index, so.item);
            controller.inventoryGrid.PlaceNewItem(so.item,1,so.originX,so.originY,so.rotated);
            controller.inventoryGridView.StopDrag();
            return;
        }
    }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleSlotClicked);
    }
}
