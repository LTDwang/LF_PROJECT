using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDrop : MonoBehaviour
{
    public InventoryGridView inventoryGridView;
    public DropSystem dropSystem;
    Button button;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(Drop);
    }

    void Drop()
    {
        ItemSO item = inventoryGridView.GetDraggingItemSO();
        if (item != null)
        {
            dropSystem.Drop(item);
            bool ok = inventoryGridView.ConsumeOneFromDraggingForExternal();
        }
    }
}
