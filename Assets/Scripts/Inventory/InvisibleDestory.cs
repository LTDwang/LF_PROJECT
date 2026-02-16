using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InvisibleDestory : MonoBehaviour
{
    [SerializeField] private float delayTime = 10f;
    Coroutine coroutine;
    private void OnBecameInvisible()
    {
        if (coroutine==null)
        {
            coroutine = StartCoroutine(DestroyTimer());
        }
    }
    private void OnBecameVisible()
    {
        if (coroutine!=null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(delayTime);
        Destroy(gameObject);
    }
}
