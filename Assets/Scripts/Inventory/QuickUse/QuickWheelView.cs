// QuickWheelView.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuickWheelView : MonoBehaviour
{
    [Header("UI")]
    public GameObject wheelRoot;
    public RectTransform wheelCenter;
    public RectTransform slotsRoot;
    public QuickWheelSlotUI slotPrefab;

    [Header("Layout")]
    [Range(4, 12)] public int slotCount = 8;
    public float radius = 160f;

    public event Action<int> OnSlotClicked;

    private readonly List<QuickWheelSlotUI> _slotUIs = new List<QuickWheelSlotUI>();

    public bool IsOpen => wheelRoot != null && wheelRoot.activeSelf;

    public void Build()
    {
        _slotUIs.Clear();

        if (slotsRoot == null || slotPrefab == null) return;

        // clear children
        for (int i = slotsRoot.childCount - 1; i >= 0; i--)
            Destroy(slotsRoot.GetChild(i).gameObject);

        // build slots in a circle
        for (int i = 0; i < slotCount; i++)
        {
            var ui = Instantiate(slotPrefab, slotsRoot);
            ui.index = i;

            var rt = ui.GetComponent<RectTransform>();
            if (rt != null)
            {
                float t = (float)i / slotCount;
                float ang = t * Mathf.PI * 2f;
                rt.anchoredPosition = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
            }

            _slotUIs.Add(ui);
        }
    }

    public void SetOpen(bool open)
    {
        if (wheelRoot != null) wheelRoot.SetActive(open);
    }

    // 纯渲染：view 不知道背包，不做扣除/使用，只画
    public void Render(Func<int, ItemSO> getItem, Func<ItemSO, int> getCount, int selectedIndex)
    {
        for (int i = 0; i < _slotUIs.Count; i++)
        {
            var item = getItem != null ? getItem(i) : null;
            int count = (item != null && getCount != null) ? getCount(item) : 0;
            _slotUIs[i].Set(item, count, i == selectedIndex);
        }
    }

    private void HandleSlotClicked(int index)
    {
        OnSlotClicked?.Invoke(index);
    }
}

