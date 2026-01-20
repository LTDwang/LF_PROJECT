using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FuctionChange : MonoBehaviour
{
    public KeyBinding inventoryFuctionAdd = new KeyBinding { keyboardBinding = "<Keyboard>/e" };
    public KeyBinding inventoryFuctionDec = new KeyBinding { keyboardBinding = "<Keyboard>/q" };

    private InputActionMap inventoryMap;
    private InputAction functionAdd;
    private InputAction functionDec;

    public List<GameObject> fuctionPads = new List<GameObject>();
    public int nowIndex = 0;

    private void Awake()
    {
        inventoryMap = new InputActionMap("inventory");

        functionAdd = inventoryMap.AddAction("Add", InputActionType.Button);
        functionDec = inventoryMap.AddAction("Dec", InputActionType.Button);

        functionAdd.AddBinding(inventoryFuctionAdd.keyboardBinding);
        functionDec.AddBinding(inventoryFuctionDec.keyboardBinding);

        functionAdd.started += FunctionAdd;
        functionDec.started += FunctionDec;
    }

    private void OnEnable()
    {
        inventoryMap.Enable();
    }

    private void OnDisable()
    {
        inventoryMap.Disable();
    }

    private void Start()
    {
        if (fuctionPads == null || fuctionPads.Count == 0) return;

        for (int i = 0; i < fuctionPads.Count; i++)
            if (fuctionPads[i] != null) fuctionPads[i].SetActive(false);

        nowIndex = Mathf.Clamp(nowIndex, 0, fuctionPads.Count - 1);
        if (fuctionPads[nowIndex] != null) fuctionPads[nowIndex].SetActive(true);
    }

    private void FunctionAdd(InputAction.CallbackContext ctx)
    {
        if (fuctionPads == null || fuctionPads.Count == 0) return;

        if (fuctionPads[nowIndex] != null) fuctionPads[nowIndex].SetActive(false);

        nowIndex++;
        if (nowIndex >= fuctionPads.Count) nowIndex = 0;

        if (fuctionPads[nowIndex] != null) fuctionPads[nowIndex].SetActive(true);
    }

    private void FunctionDec(InputAction.CallbackContext ctx)
    {
        if (fuctionPads == null || fuctionPads.Count == 0) return;

        if (fuctionPads[nowIndex] != null) fuctionPads[nowIndex].SetActive(false);

        nowIndex--;
        if (nowIndex < 0) nowIndex = fuctionPads.Count - 1;

        if (fuctionPads[nowIndex] != null) fuctionPads[nowIndex].SetActive(true);
    }
}
