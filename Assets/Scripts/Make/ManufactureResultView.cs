using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ManufactureResultView : MonoBehaviour
{
    [Header("逻辑")]
    public ManufactureManager manager;      // 里面有 GetPreviewResult() / CraftIfPossible()

    [Header("UI 引用")]
    public RectTransform cellsRoot;         // 小格子父节点（有 GridLayoutGroup）
    public RectTransform itemsRoot;         // 图标父节点（和 cellsRoot 完全重合）
    public GameObject cellPrefab;           // 结果区的单个格子 prefab（背景用）
    public GameObject itemIconPrefab;       // 物品图标 prefab（用背包里的那一个）

    private GridLayoutGroup layout;
    private RectTransform[,] cellRTs;       // 记录生成的格子，给图标定位用
    public Button manufactureButton;
    private void Awake()
    {
        if (cellsRoot != null)
            layout = cellsRoot.GetComponent<GridLayoutGroup>();
        //manufactureButton.onClick.AddListener(OnPointerClick);
    }

    private void OnEnable()
    {
        BuildEmptyGrid(1, 1);   // 先生成一个 1×1，避免空引用

        //RefreshPreview();

        if (manager != null)
        {
            manager.RefreshPreviewRequested += RefreshPreview;
        }
    }

    private void OnDisable()
    {
        if (manager != null)
        {
            manager.RefreshPreviewRequested -= RefreshPreview;
        }
    }

    // ================== 刷新预览 ==================

    public void RefreshPreview()
    {
        if (manager == null || layout == null || cellsRoot == null || itemsRoot == null)
            return;

        // 清掉旧图标
        foreach (Transform child in itemsRoot)
            Destroy(child.gameObject);

        var preview = manager.GetPreviewResult();
        if (preview.item == null || preview.count <= 0)
        {
            // 没有产物就不画
            BuildEmptyGrid(1, 1);
            return;
        }

        ItemSO item = preview.item;
        int count = preview.count;

        int w = Mathf.Max(1, item.gridWidth);
        int h = Mathf.Max(1, item.gridHeight);
        Debug.Log($"{w},{h}");
        // 重新生成 w×h 个小格子
        BuildEmptyGrid(w, h);

        // 生成一张大图标
        CreateIconForResult(item, count, w, h);
    }

    // ================== 生成格子 ==================

    private void BuildEmptyGrid(int w, int h)
    {
        if (cellsRoot == null || cellPrefab == null || layout == null)
            return;

        // 清掉旧格子
        foreach (Transform child in cellsRoot)
            Destroy(child.gameObject);

        cellRTs = new RectTransform[w, h];

        // 让 GridLayout 按列数来排布
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = w;

        for (int iy = 0; iy < h; iy++)
        {
            for (int ix = 0; ix < w; ix++)
            {
                GameObject go = Instantiate(cellPrefab, cellsRoot);
                Button button = go.GetComponent<Button>();
                //button.onClick.AddListener(OnPointerClick);
                RectTransform rt = go.GetComponent<RectTransform>();
                cellRTs[ix, iy] = rt;
            }
        }
    }

    // ================== 生成图标 ==================

    private void CreateIconForResult(ItemSO item, int count, int w, int h)
    {
        if (itemIconPrefab == null || itemsRoot == null || layout == null || cellRTs == null)
            return;

        GameObject iconGO = Instantiate(itemIconPrefab, itemsRoot);
        RectTransform rt = iconGO.GetComponent<RectTransform>();

        // sprite
        var img = iconGO.GetComponent<Image>();
        if (img != null)
            img.sprite = item.icon;

        // 数量文本（可选）
        var txt = iconGO.GetComponentInChildren<TextMeshProUGUI>();
        if (txt != null)
            txt.text = count > 1 ? count.ToString() : "";

        // 尺寸：覆盖 w×h 个格子（和背包一样的公式）
        Vector2 cellSize = layout.cellSize;
        Vector2 spacing = layout.spacing;

        float width = cellSize.x * w + spacing.x * (w - 1);
        float height = cellSize.y * h + spacing.y * (h - 1);
        rt.sizeDelta = new Vector2(width, height);

        // 这里假定结果物品不旋转，如需要也可以在 ItemSO 里加标记再转 90°
        rt.localRotation = Quaternion.identity;

        // 位置：取左上格 + 右下格的中心点
        RectTransform cellLT = cellRTs[0, 0];
        RectTransform cellRB = cellRTs[w - 1, h - 1];

        Vector3 worldCenterLT = cellLT.TransformPoint(cellLT.rect.center);
        Vector3 worldCenterRB = cellRB.TransformPoint(cellRB.rect.center);
        Vector3 worldCenter = (worldCenterLT + worldCenterRB) * 0.5f;

        Vector3 localCenter = itemsRoot.InverseTransformPoint(worldCenter);
        rt.anchoredPosition = localCenter;
    }

    // ================== 点击：尝试把产物放进背包 ==================

    public void OnPointerClick()
    {
        if (manager == null)
            return;

        // 沿用你之前的逻辑：成功则消耗材料并放入背包，预览会通过事件重新刷新
        if (manager.CraftIfPossible())
        {
            Debug.Log("创造");
            RefreshPreview();
        }
        
        
    }
}
