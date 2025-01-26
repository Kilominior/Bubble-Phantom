using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodPanel : MonoBehaviour
{
    // 上层在此值以上不显示
    private static readonly float upAppearVal = 0.95f;
    private static readonly float downAppearVal = 0.5f;
    private static readonly float downBonus = 0.2f;

    [SerializeField]
    private GameObject BloodPanelDown;
    [SerializeField]
    private GameObject BloodPanelUp;

    private Material BloodDownMat;
    private Material BloodUpMat;

    private float bloodMatVal;

    private void Start()
    {
        BloodPanelDown.SetActive(true);
        BloodPanelUp.SetActive(true);

        BloodDownMat = BloodPanelDown.GetComponent<Image>().material;
        BloodUpMat = BloodPanelUp.GetComponent<Image>().material;

        TypeEventSystem.Global.Register<PlayerHealthUpdateEvent>(OnHealthUpdate).UnRegisterWhenGameObjectDestroyed(this);

        Initialize();
    }

    private void Initialize()
    {
        SetBlood(1.0f);
    }

    private void OnHealthUpdate(PlayerHealthUpdateEvent @event)
    {
        SetBlood(@event.healthRemainPercentage);
    }

    private void SetBlood(float bloodVal)
    {
        bloodMatVal = bloodVal;
        SetBloodMat();
    }

    private void SetBloodMat()
    {
        if(bloodMatVal > upAppearVal)
        {
            BloodPanelUp.SetActive(false);
        }
        else
        {
            BloodPanelUp.SetActive(true);
        }
        BloodUpMat.SetFloat("_Cutoff", bloodMatVal);


        if (bloodMatVal > downAppearVal)
        {
            BloodPanelDown.SetActive(false);
        }
        else
        {
            BloodPanelDown.SetActive(true);
        }
        BloodDownMat.SetFloat("_Cutoff", bloodMatVal + downBonus);

    }
}
