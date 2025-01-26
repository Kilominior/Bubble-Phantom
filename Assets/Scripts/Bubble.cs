using System;
using System.Collections;
using QFramework;
using UnityEngine;

public enum BubbleState
{
    Spawning,       // 生成中
    Idle,           // 在场景中静置
    Controlled,     // 被玩家控制
    PerfectChance,  // 可触发极限闪避
    Exploding       // 正在消亡
}

public class Bubble: MonoBehaviour
{
    private BubbleState state;
    private bool isControlled;
    private WaitForSeconds waitForDodgeEnd;
    private WaitForSeconds waitForSpawnEnd;
    private Rigidbody2D rb;
    private Collider2D collision;
    private float maxMoveVelocity;
    private Action onDodgeEnd;

    public float health;

    private Animator animator;
    [SerializeField]
    private Animator weakAnimator;
    [SerializeField]
    private Animator perfectAnimator;
    // public******************************************************************************
    public void Spawn(float spawnTime, float motherHealth, bool isInitial = false)
    {
        if(isInitial)
        {
            health = HealthManager.Instance.maxHealth;
        }
        else
        {
            health = HealthManager.Instance.InitHealthByOther(motherHealth);
        }
        UpdateHealthState(HealthManager.Instance.GetHealthState(health));

        isControlled = false;
        state = BubbleState.Spawning;
        waitForSpawnEnd = new WaitForSeconds(spawnTime);
        StartCoroutine(nameof(Spawning));
    }

    public void GetControlled(float maxMoveVelocity, float dodgeTime, Action onDodgeEnd)
    {
        this.maxMoveVelocity = maxMoveVelocity;
        waitForDodgeEnd = new WaitForSeconds(dodgeTime);
        this.onDodgeEnd = onDodgeEnd;
        UpdateHealthState(HealthManager.Instance.GetHealthState(health));

        isControlled = true;
        state = BubbleState.Controlled;
    }

    public void Move(Vector3 deltaPos)
    {
        Fade(HealthUpdateMode.move);
        if (state == BubbleState.Exploding) return;

        transform.Translate(deltaPos);
    }

    public void SetMoveStatus(bool isMoving)
    {
        // animator.SetBool("isMoving", isMoving);
    }

    public void Dodge(Vector2 deltaPos)
    {
        Fade(HealthUpdateMode.dodge);
        if (state == BubbleState.Exploding) return;

        if (state == BubbleState.PerfectChance)
        {
            // 极限闪避，生成子泡泡
            animator.SetTrigger("PerfectDodge");
            perfectAnimator.SetTrigger("Perfect");
            HealthManager.Instance.StartInvincible(InvincibleType.perfectDodge);
            BubbleManager.Instance.SpawnBubble(health, transform.position);
        }
        else
        {
            animator.SetTrigger("Dodge");
            HealthManager.Instance.StartInvincible(InvincibleType.normalDodge);
        }
        rb.AddForce(deltaPos);
        StartCoroutine(nameof(Dodging));
    }
// private******************************************************************************
    private void EndDodge()
    {
        rb.velocity = Vector2.zero;
        onDodgeEnd();
    }

    private IEnumerator Dodging()
    {
        yield return waitForDodgeEnd;
        EndDodge();
    }

    private void EndSpawn()
    {
        collision.enabled = true;
        if (state == BubbleState.Spawning)
        state = BubbleState.Idle;
    }

    private IEnumerator Spawning()
    {
        yield return waitForSpawnEnd;
        EndSpawn();
    }

    private void Heal(float healthSource)
    {
        HealthManager.Instance.UpdateHealth(ref health, healthSource);
        UpdateHealthState(HealthManager.Instance.GetHealthState(health));
    }

    private void Fade(HealthUpdateMode updateMode)
    {
        HealthManager.Instance.UpdateHealth(ref health, updateMode);
        UpdateHealthState(HealthManager.Instance.GetHealthState(health));

    }

    private void UpdateHealthState(HealthState healthState)
    {
        switch (healthState)
        {
            case HealthState.spawn:
                // Debug.Log("health: spawn");
                break;
            case HealthState.safe:
                // Debug.Log("health: safe");
                animator.SetBool("isWeak", false);
                weakAnimator.SetBool("isWeak", false);
                break;
            case HealthState.dangerous:
                // Debug.Log("health: dangerous");
                animator.SetBool("isWeak", true);
                weakAnimator.SetBool("isWeak", true);
                break;
            case HealthState.dead:
                Debug.Log("health: dead: " + health);
                Explode();
                return;
            default:
                break;
        }
        float scalePercentage = HealthManager.Instance.GetScalePercentage(health);
        Vector3 scale = new Vector3(scalePercentage, scalePercentage, scalePercentage);
        transform.localScale = scale;
    }

    private void Explode()
    {
        animator.SetTrigger("Dead");
        state = BubbleState.Exploding;
        BubbleManager.Instance.RemoveBubble(this);
        collision.enabled = false;

        // 主控bubble破裂，尝试复活
        if(isControlled)
        {
            TypeEventSystem.Global.Send<PlayerDeathEvent>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(state == BubbleState.Spawning || state == BubbleState.Exploding) return;
        if(other.gameObject.layer == BossCollisionLayer.BossBody)
        {
            Fade(HealthUpdateMode.collide);
        }

        if (other.gameObject.layer == BubbleCollisionLayer.BubbleNormal)
        {
            if (state == BubbleState.Controlled || state == BubbleState.PerfectChance)
            {
                Heal(other.GetComponent<Bubble>().health);
            }
            else
            {
                Explode();
            }
        }

        // 进入极限闪避状态
        if (!isControlled || state != BubbleState.Controlled) return;
        if (other.gameObject.layer == BossCollisionLayer.BossPreAttack)
        {

            Debug.Log("state to invincible");
            state = BubbleState.PerfectChance;
            // gameObject.layer = BubbleCollisionLayer.BubbleInvincible;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (state == BubbleState.Spawning || state == BubbleState.Exploding) return;
        // 离开极限闪避状态
        if (!isControlled || state != BubbleState.PerfectChance) return;
        if (other.gameObject.layer == BossCollisionLayer.BossPreAttack)
        {
            Debug.Log("state to controlled");
            state = BubbleState.Controlled;
            // gameObject.layer = BubbleCollisionLayer.BubbleNormal;
        }
    }

    private void OnEnable() {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        collision = GetComponent<Collider2D>();
        collision.enabled = false;
    }
}
