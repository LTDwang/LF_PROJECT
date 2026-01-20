// QuickWheelController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickWheelController : MonoBehaviour
{
    [Header("Refs")]
    public InventoryGrid inventoryGrid;
    public InventoryGridView inventoryGridView;
    public DropSystem dropSystem;
    public QuickWheelView view;

    [Header("Config")]
    [Range(4, 12)] public int slotCount = 8;
    public float keepTime;

    [Header("Input (hold to open)")]
    public Key openKey = Key.Tab;     // keyboard hold
    public bool allowGamepad = true;  // gamepad LB hold
    public bool releaseToUse = true;  // release -> use
    public bool useMouseFirst = true; // prefer mouse direction over stick

    public QuickWheelModel _model;

    public int _selectedIndex;

    public MyPlayerInput input;

    public float pressedTime;

    public PlayerController playerController;

    public bool pressed;

    public bool bagOpening = false;

    void Awake()
    {
        _model = new QuickWheelModel(slotCount);
        keepTime = input.inventoryKeyKeepTime;
        pressedTime = 0;
        if (view != null)
        {
            view.slotCount = slotCount;
            view.Build();
            view.SetOpen(false);
        }
    }

    void Update()
    {
        if (view == null) return;

        bool held = IsOpenHeld();

        if (held)
        {
            pressedTime += Time.deltaTime;
            pressed = true;
        }

        if (held && !view.IsOpen && pressedTime > keepTime && pressed)
        {
            view.SetOpen(true);
            Render();
        }

        if (!held && view.IsOpen && pressedTime > keepTime && pressed)
        {
            if (releaseToUse && !IsDraggingToBind())
                TryUseSelected();

            view.SetOpen(false);
            pressedTime = 0;
            pressed = false;
            return;
        }

        if (!held && pressed)
        {
            pressed = false;
            if (!bagOpening)
            {
                bagOpening = true;
                playerController.OnOpenInventory();
            }
            else
            {
                bagOpening = false;
                playerController.OnCloseInventory();
            }
        }
        if (!view.IsOpen) return;

        // update selection by direction
        Vector2 dir = GetSelectDirection();
        if (dir.sqrMagnitude > 0.15f * 0.15f)
            _selectedIndex = DirectionToIndex(dir, slotCount);

        Render();
    }

    private void Render()
    {
        view.Render(
            getItem: i => _model.GetItem(i),
            getCount: item => inventoryGrid != null ? inventoryGrid.GetTotalCount(item) : 0,
            selectedIndex: _selectedIndex
        );
    }



    public bool IsDraggingToBind()
        => inventoryGridView != null && inventoryGridView.IsDraggingItem;

    private void TryUseSelected()
    {
        var item = _model.GetItem(_selectedIndex);
        if (item == null) return;
        if (inventoryGrid == null) return;


        if (!inventoryGrid.TryConsumeOne(item)) return;


        if (dropSystem != null)
            dropSystem.Drop(item);

        if (inventoryGridView != null)
            inventoryGridView.RefreshAllItems();
    }

    private bool IsOpenHeld()
    {
        bool kb = Keyboard.current != null && Keyboard.current[openKey].isPressed;

        bool gp = false;
        if (allowGamepad && Gamepad.current != null)
            gp = Gamepad.current.leftShoulder.isPressed;

        return kb || gp;
    }

    private Vector2 GetSelectDirection()
    {
        // mouse direction
        if (useMouseFirst && Mouse.current != null && view != null && view.wheelCenter != null)
        {
            Vector2 center = RectTransformUtility.WorldToScreenPoint(null, view.wheelCenter.position);
            Vector2 mouse = Mouse.current.position.ReadValue();
            Vector2 d = mouse - center;
            if (d.sqrMagnitude > 1e-3f) return d.normalized;
        }

        // gamepad right stick
        if (allowGamepad && Gamepad.current != null)
        {
            Vector2 stick = Gamepad.current.rightStick.ReadValue();
            if (stick.sqrMagnitude > 1e-3f) return stick.normalized;
        }

        return Vector2.zero;
    }

    private static int DirectionToIndex(Vector2 dir, int count)
    {
        float angle = Mathf.Atan2(dir.y, dir.x);          // -pi..pi
        float t = angle / (Mathf.PI * 2f);   // 0..1
        int index = Mathf.RoundToInt(t * count) % count;
        if (index<0)
        {
            index += 8;
        }
        return index;
    }
}
