using UnityEngine;
using UnityEngine.UI;

public class ManufactureGridView : MonoBehaviour
{
    [Header("逻辑")]
    public ManufactureGrid materialGrid;
    public ManufactureManager manager;
    public InventoryGridView inventoryView;

    [Header("UI")]
    public RectTransform cellsRoot;  // 有 GridLayoutGroup 的容器
    public GameObject cellPrefab;    // 单个材料格的 prefab

    private void Start()
    {
        if (materialGrid == null || cellsRoot == null || cellPrefab == null)
        {
            Debug.LogWarning("ManufactureGridView: 引用没配好");
            return;
        }

        BuildCells();
    }

    private void BuildCells()
    {
        int w = materialGrid.width;
        int h = materialGrid.height;

        // 清空旧的
        foreach (Transform child in cellsRoot)
        {
            Destroy(child.gameObject);
        }

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                GameObject go = Instantiate(cellPrefab, cellsRoot);
                var slotUI = go.GetComponent<ManufactureMaterialSlotUI>();
                if (slotUI != null)
                {
                    slotUI.x = x;
                    slotUI.y = y;
                    slotUI.materialGrid = materialGrid;
                    slotUI.manager = manager;
                    slotUI.inventoryView = inventoryView;
                }
            }
        }
    }
}
