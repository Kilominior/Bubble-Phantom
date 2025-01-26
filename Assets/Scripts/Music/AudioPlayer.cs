using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    protected AudioSource audioSource;

    [SerializeField]
    protected AudioClip[] audioClips;

    private WaitForFixedUpdate WaitForEndOfCurrentClip;

    protected void AudioPlay(int id, bool isLoop = false)
    {
        StopCoroutine(nameof(WaitForEndOfFirstClip));
        audioSource.Pause();
        audioSource.clip = audioClips[id];
        audioSource.Play();
        audioSource.loop = isLoop;
    }

    protected void AudioPlayRandom(int startId, int endId, bool isLoop = false)
    {
        StopCoroutine(nameof(WaitForEndOfFirstClip));
        audioSource.Pause();
        audioSource.clip = audioClips[Random.Range(startId, endId + 1)];
        audioSource.Play();
        audioSource.loop = isLoop;
    }

    protected void AudioPlayAndThenLoopPlay(int firstId, int loopId)
    {
        AudioPlay(firstId);
        WaitForEndOfCurrentClip = new WaitForFixedUpdate();
        StopCoroutine(nameof(WaitForEndOfFirstClip));
        StartCoroutine(nameof(WaitForEndOfFirstClip), loopId);
    }


    private IEnumerator WaitForEndOfFirstClip(int loopId)
    {
        while (true)
        {
            yield return WaitForEndOfCurrentClip;
            if (!audioSource.isPlaying)
            {
                audioSource.clip = audioClips[loopId];
                audioSource.loop = true;
                audioSource.Play();
                yield break;
            }
        }
    }
}
