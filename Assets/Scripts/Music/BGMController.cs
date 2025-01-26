using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : AudioPlayer
{
    [SerializeField]
    private AudioSource FXSource;

    [SerializeField]
    private AudioClip[] FXClips;

    private WaitForSeconds waitForFXMusicPlayDelay;

    private void Start()
    {
        waitForFXMusicPlayDelay = new WaitForSeconds(1.0f);
        TypeEventSystem.Global.Register<PlayerDeathEvent>(OnPlayerDead).UnRegisterWhenGameObjectDestroyed(this);

        Initialize();
    }

    private void OnPlayerDead(PlayerDeathEvent @event)
    {
        audioSource.Stop();
        FXSource.Stop();
    }

    private void Initialize()
    {
        StartCoroutine(nameof(WaitForFXMusicPlay));
    }

    private IEnumerator WaitForFXMusicPlay()
    {
        while (true)
        {
            yield return waitForFXMusicPlayDelay;
            // 有概率播放环境音
            if (!FXSource.isPlaying && Random.Range(0, 1.0f) >= 0.8f)
            {
                FXPlay();
            }
        }
    }


    protected void FXPlay()
    {
        FXSource.Pause();
        FXSource.clip = FXClips[Random.Range(0, FXClips.Length)];
        FXSource.Play();
        FXSource.loop = false;
    }
}
