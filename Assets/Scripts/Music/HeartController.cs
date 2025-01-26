using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartController : AudioPlayer
{
    private static readonly float lowHealth = 0.4f;

    private void Start()
    {
        TypeEventSystem.Global.Register<PlayerHealthUpdateEvent>(OnHealthUpdate).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<PlayerDeathEvent>(OnPlayerDead).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnPlayerDead(PlayerDeathEvent @event)
    {
        audioSource.Stop();
    }

    private void OnHealthUpdate(PlayerHealthUpdateEvent @event)
    {
        if (@event.healthRemainPercentage <= lowHealth)
        {
            AudioPlay(0, true);
        }
        else
        {
            audioSource.Stop();
        }
    }
}
