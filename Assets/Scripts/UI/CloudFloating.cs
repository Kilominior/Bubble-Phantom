using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CloudFloating: MonoBehaviour
{
// public******************************************************************************
// private******************************************************************************
    private void Awake() {
        StartCoroutine(nameof(ShakeCoolDown));
    }
    private IEnumerator ShakeCoolDown()
    {
        var coolDownTime = Random.Range(2.0f, 8.0f);
        yield return new WaitForSeconds(coolDownTime);
        var shakeTime = Random.Range(2.0f, 8.0f);
        transform.DOShakePosition(shakeTime, new Vector3(0.2f, 0.2f, 0), 1, 10).OnComplete(() =>
        {
            StartCoroutine(nameof(ShakeCoolDown));
        });
    }
}
