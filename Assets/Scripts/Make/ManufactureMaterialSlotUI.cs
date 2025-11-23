using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ManufactureMaterialSlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("格子坐标（在 ManufactureGrid 中的 x,y）")]
    public int x;
    public int y;

    [Header("引用")]
    public ManufactureGrid materialGrid;
    public ManufactureManager manager;
    public InventoryGridView inventoryView;

    [Header("UI")]
    public Image iconImage;
    public TextMeshProUGUI countText; // 目前不做数量，可以留空

    private void Awake()
    {
        if (iconImage == null)
            iconImage = GetComponent<Image>();
    }

    private void Start()
    {
        Refresh();
        if (materialGrid != null)
        {
            materialGrid.OnChanged += Refresh;
        }
    }

    private void OnDestroy()
    {
        if (materialGrid != null)
        {
            materialGrid.OnChanged -= Refresh;
        }
    }

    public void Refresh()
    {
        if (materialGrid == null || iconImage == null) return;

        var inst = materialGrid.GetItemAt(x, y);
        if (inst == null)
        {
            iconImage.enabled = false;
            if (countText != null) countText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = inst.item.icon;
            if (countText != null)
                countText.text = inst.count > 1 ? inst.count.ToString() : "";
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (materialGrid == null) return;

        // 1) 正在从背包拖物品过来 → 尝试在这里放下
        if (inventoryView != null && inventoryView.IsDraggingItem)
        {
            var so = inventoryView.GetDraggingItemSO();
            if (so == null) return;

            // 简化：材料区不支持旋转（后面需要再加旋转逻辑）
            bool rotated = false;

            // 尝试以当前格子为“物品的左上角”放置
            bool can = materialGrid.CanPlace(so, x, y, rotated);
            if (!can)
                return;

            // 先在材料区占格
            var inst = materialGrid.PlaceItem(so, 1, x, y, rotated);
            if (inst == null)
                return;

            // 再从背包里扣掉 1 个 + 结束拖拽
            bool ok = inventoryView.ConsumeOneFromDraggingForExternal();
            if (!ok)
            {
                // 理论上不会发生；保险起见回滚一下
                materialGrid.RemoveItem(inst);
                return;
            }

            return;
        }

        // 2) 没有拖拽物品 → 点击已有材料格：把整个物品删掉（简单版：不返还给背包）
        var existing = materialGrid.GetItemAt(x, y);
        if (existing != null)
        {
            materialGrid.RemoveItem(existing);
        }
    }
}
