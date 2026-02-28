using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FastUseActionSO:ScriptableObject
{
    public abstract void Excute(GameObject player, ItemSO item);
}
