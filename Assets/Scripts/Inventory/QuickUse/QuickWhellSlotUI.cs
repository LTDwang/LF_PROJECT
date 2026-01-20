// QuickWheelSlotUI.cs
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickWheelSlotUI : MonoBehaviour, IPointerClickHandler
{
    public int index;
    public Image icon;
    public TextMeshProUGUI countText;
    public Image highlight;

    private Action<int> _click;

    public void SetClickCallback(Action<int> cb) => _click = cb;

    public void Set(ItemSO item, int count, bool selected)
    {
        if (icon != null)
        {
            icon.enabled = item != null;
            icon.sprite = item != null ? item.icon : null;
        }

        if (countText != null)
        {
            countText.text = (item != null && count > 0) ? count.ToString() : "";
        }

        if (highlight != null)
        {
            highlight.enabled = selected;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _click?.Invoke(index);
    }
}
