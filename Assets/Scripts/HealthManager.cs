using System;
using System.Collections;
using QFramework;
using UnityEngine;

public enum HealthUpdateMode
{
    move,           // 移动
    dodge,          // 普通闪避
    perfectDodge,   // 极限闪避
    collide         // 撞击Boss
}

public enum HealthState
{
    spawn,
    safe,
    dangerous,
    dead
}

public enum InvincibleType
{
    normalDodge,
    perfectDodge
}

public class HealthManager: SingletonMono<HealthManager>
{
    public float maxHealth;
    public float dangerHealth;
    public float moveDeltaHealth;
    public float dodgeDeltaHealth;
    public float perfectDodgeDeltaHealth;
    [Tooltip("子泡泡初始生命值基于母泡泡的百分比")]
    public float childBubbleHealthPercentage;
    [Tooltip("主控泡泡受到治愈时基于被吸收泡泡的百分比")]
    public float healHealthPercentage;

    private bool m_isInvincible;
    public bool isInvincible// todo: 无敌不是全局性的
    {
        get { return m_isInvincible; }
        private set { m_isInvincible = value; }
    }
    private WaitForSeconds waitForEndOfNormalInvincible;
    private WaitForSeconds waitForEndOfPerfectInvincible;
    [SerializeField]
    private float normalInvincibleTime;
    [SerializeField]
    private float perfectInvincibleTime;
    // public******************************************************************************
    public float InitHealthByOther(float motherHealth)
    {
        return motherHealth * childBubbleHealthPercentage;
    }

    public void UpdateHealth(ref float health, HealthUpdateMode updateMode)
    {
        if (isInvincible) return;
        switch (updateMode)
        {
            case HealthUpdateMode.move:
                health -= moveDeltaHealth * Time.deltaTime;
                break;
            case HealthUpdateMode.dodge:
                health -= dodgeDeltaHealth;
                break;
            case HealthUpdateMode.perfectDodge:
                health -= perfectDodgeDeltaHealth;
                break;
            case HealthUpdateMode.collide:
                health = 0;
                break;
            default:
                health = 0;
                break;
        }
    }

    public void UpdateHealth(ref float health, float healthSource)
    {
        float deltaHealth = healthSource * healHealthPercentage;
        health += deltaHealth;
        health = Math.Clamp(health, 0, maxHealth);
    }

    public HealthState GetHealthState(float health)
    {
        if (health <= 0)
        {
            return HealthState.dead;
        }

        if (health <= dangerHealth)
        {
            return HealthState.dangerous;
        }
        return HealthState.safe;
    }

    public float GetScalePercentage(float health)
    {
        return (health / maxHealth + 1.0f) * 0.5f;
    }

    public void StartInvincible(InvincibleType type)
    {
        Debug.Log("invincible start");
        isInvincible = true;
        StopCoroutine(nameof(InvincibleING));
        StartCoroutine(nameof(InvincibleING), type);
    }
    // private******************************************************************************
    private void Awake() {
        waitForEndOfNormalInvincible = new WaitForSeconds(normalInvincibleTime);
        waitForEndOfPerfectInvincible = new WaitForSeconds(perfectInvincibleTime);

        TypeEventSystem.Global.Register<GameWinEvent>(OnGameWin).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameLoseEvent>(OnGameLose).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameStartEvent>(OnGameStart).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnGameWin(GameWinEvent @event)
    {
        isInvincible = true;
    }

    private void OnGameLose(GameLoseEvent @event)
    {
        isInvincible = true;
    }

    private void OnGameStart(GameStartEvent @event)
    {
        isInvincible = false;
    }

    private IEnumerator InvincibleING(InvincibleType type)
    {
        if(type == InvincibleType.normalDodge)
        {
            yield return waitForEndOfNormalInvincible;
        }
        else
        {
            yield return waitForEndOfPerfectInvincible;
        }
        isInvincible = false;
        Debug.Log("invincible end");
    }
}
