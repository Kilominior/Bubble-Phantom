using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    private void Start()
    {
        StartCoroutine(nameof(WaitForDestory));
    }

    private IEnumerator WaitForDestory()
    {
        yield return new WaitForSeconds(10.0f);
        Destroy(gameObject);
    }
}
