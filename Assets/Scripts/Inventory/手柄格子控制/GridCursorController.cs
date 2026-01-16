using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GridCursorController : MonoBehaviour
{
    [Header("Target Grid View (当前聚焦的网格视图)")]
    public InventoryGridView gridView;  // 先只接背包；后面可以扩展切换材料格等

    [Header("Cursor Highlight UI (一个Image框)")]
    public GameObject cursorHighlight; // 一个Image的RectTransform，用来显示当前格子高亮
    public GameObject highlight;//游戏中高亮格子实例

    [Header("Ghost Icon (拿起后跟随的半透明图标)")]
    public RectTransform ghostRoot;      // 建议放在和ItemsRoot同级的Overlay
    public GameObject itemIconPrefab;    // 复用你背包的Icon prefab（Image + TMP数量）

    [Header("Move Settings")]
    public float repeatDelay = 0.18f;    // 长按移动重复间隔
    public float deadzone = 0.35f;

    // ---- runtime state ----
    public int cx=0, cy=0;                  // 当前光标格子坐标
    private bool rotated;                // 手上物品旋转状态
    private InventoryItem picked;        // 手上拿着的实例（从网格里移除的那个）
    private Vector2Int pickedOrigin;     // 取消时放回原位用
    private GameObject ghostGO;
    private RectTransform ghostRT;

    // ---- input ----
    private InputActionMap map;
    private InputAction moveAction, submitAction, cancelAction, rotateAction;
    private float moveCooldown;

    void Start()
    {
        highlight = null;
        BuildActions();
        map.Enable();

        ClampCursor();
        UpdateCursorVisual();
    }

    void OnDisable()
    {
        //map?.Disable();
        //map?.Dispose();
        //map = null;
    }

    void Update()
    {
        if (gridView == null||gridView.inputMode==InventoryInputMode.Mouse) return;

        // 处理移动（支持摇杆长按重复）
        Vector2 mv = moveAction.ReadValue<Vector2>();
        if (mv.sqrMagnitude >= deadzone * deadzone)
        {
            moveCooldown -= Time.unscaledDeltaTime;
            if (moveCooldown <= 0f)
            {
                int dx = Mathf.Abs(mv.x) > Mathf.Abs(mv.y) ? (mv.x > 0 ? 1 : -1) : 0;
                int dy = Mathf.Abs(mv.y) >= Mathf.Abs(mv.x) ? (mv.y > 0 ? -1 : 1) : 0; // UI里上是-1

                MoveCursor(dx, dy);
                moveCooldown = repeatDelay;
            }
        }
        else
        {
            moveCooldown = 0f;
        }

        // 如果手上拿着物品，实时更新预览（红绿格）+ ghost跟随
        if (picked != null)
        {
            UpdateGhostIcon();
        }
        else
        {
            gridView.ClearPreview();
            DestroyGhost();
        }
    }

    private void BuildActions()
    {
        map = new InputActionMap("GridCursor");

        // Move: 手柄左摇杆/Dpad + 键盘WSAD/方向键
        moveAction = map.AddAction("Move", InputActionType.Value, binding: "<Gamepad>/leftStick");
        moveAction.AddCompositeBinding("Dpad")
            .With("Up", "<Gamepad>/dpad/up")
            .With("Down", "<Gamepad>/dpad/down")
            .With("Left", "<Gamepad>/dpad/left")
            .With("Right", "<Gamepad>/dpad/right");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Submit = A / Enter / Space
        submitAction = map.AddAction("Submit", InputActionType.Button, "<Gamepad>/buttonSouth");
        submitAction.AddBinding("<Keyboard>/enter");
        submitAction.AddBinding("<Keyboard>/space");
        submitAction.performed += _ => OnSubmit();

        // Cancel = B / Esc
        cancelAction = map.AddAction("Cancel", InputActionType.Button, "<Gamepad>/buttonEast");
        cancelAction.AddBinding("<Keyboard>/escape");
        cancelAction.performed += _ => OnCancel();

        // Rotate = X / R
        rotateAction = map.AddAction("Rotate", InputActionType.Button, "<Gamepad>/buttonWest");
        rotateAction.AddBinding("<Keyboard>/r");
        rotateAction.performed += _ => OnRotate();
    }

    private void MoveCursor(int dx, int dy)
    {
        cx += dx;
        cy += dy;
        ClampCursor();
        UpdateCursorVisual();
        gridView.OnCellHover(gridView.cellUIs[cx, cx]);
    }

    private void ClampCursor()
    {
        if (gridView == null) return;
        cx = Mathf.Clamp(cx, 0, gridView.inventoryGrid.Width - 1);
        cy = Mathf.Clamp(cy, 0, gridView.inventoryGrid.Height - 1);
    }

    private void UpdateCursorVisual()
    {
        if (highlight!=null)
        {
            Destroy(highlight);
            highlight = null;
        }

        Debug.Log("高亮格子");
        if (gridView == null || cursorHighlight == null) return;

        RectTransform cell = gridView.cellUIs[cx,cy].GetComponent<RectTransform>();
        if (cell == null) return;

        // 让高亮框对齐到该格子
        highlight = Instantiate(cursorHighlight, cell);
        highlight.transform.localPosition = Vector3.zero;

        Vector2 size = gridView.Layout.cellSize;
        highlight.GetComponent<RectTransform>().sizeDelta = size;
    }

    private void OnSubmit()
    {
        if (gridView == null) return;

        gridView.OnCellClicked(gridView.cellUIs[cx, cy]);
        picked = gridView.DraggingItem;
    }

    private void OnCancel()
    {
        if (gridView == null) return;

        if (picked != null)
        {
            gridView.inventoryGrid.PlaceNewItemWithNoPosition(picked.item);
            picked = null;
            rotated = false;
            DestroyGhost();
            gridView.ClearPreview();
        }
        else
        {
            // 没拿东西时的Cancel：你可以在这里关闭背包UI
        }
    }

    private void OnRotate()
    {
        if (picked == null) return;
        rotated = !rotated;

        // ghost 旋转 / 预览刷新
        UpdateGhostIcon();
    }

    // ---------------- ghost icon ----------------

    private void CreateGhostIcon()
    {
        if (ghostRoot == null || itemIconPrefab == null || picked == null) return;

        DestroyGhost();

        ghostGO = Instantiate(itemIconPrefab, ghostRoot);
        ghostRT = ghostGO.GetComponent<RectTransform>();

        // 半透明
        var img = ghostGO.GetComponent<Image>();
        if (img != null)
        {
            var c = img.color; c.a = 0.6f;
            img.color = c;
            img.raycastTarget = false;
        }

        // 数量文字可隐藏
        var tmp = ghostGO.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmp != null) tmp.text = "";

        UpdateGhostIcon();
    }

    private void UpdateGhostIcon()
    {
        if (ghostRT == null || gridView == null || picked == null) return;

        // 位置：跟随当前格子中心
        //RectTransform cell = gridView.GetCellRect(cx, cy);
        /*if (cell != null)
        {
            Vector3 worldCenter = cell.TransformPoint(cell.rect.center);
            Vector3 local = ghostRoot.InverseTransformPoint(worldCenter);
            ghostRT.anchoredPosition3D = local;
        }*/

        // 大小：按占格 w×h
        //Vector2 size = gridView.GetItemVisualSize(picked, rotated);
        //ghostRT.sizeDelta = size;

        // 旋转：90°
        ghostRT.localRotation = rotated ? Quaternion.Euler(0, 0, 90f) : Quaternion.identity;
    }

    private void DestroyGhost()
    {
        if (ghostGO != null) Destroy(ghostGO);
        ghostGO = null;
        ghostRT = null;
    }
}
