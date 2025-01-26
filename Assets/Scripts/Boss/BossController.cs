using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public enum BossState
{
    Idle,
    Flying,
    Rushing,
    Leaving
}

public class BossController: MonoBehaviour
{
    private int health;
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private BossHealthPanel healthPanel;
    [SerializeField]
    private float gameOverDelayTime;

    [SerializeField]
    private Collider2D preAttackFly;
    [SerializeField]
    private Collider2D preAttackRush;
    [SerializeField]
    private GameObject friesPrefab;
    private Bubble currentBubble;
    [SerializeField]
    private Transform friesRoot;

    // 状态
    private bool m_isWithFries;
    private bool isWithFries
    {
        get { return m_isWithFries; }
        set
        {
            m_isWithFries = value;
            animator.SetBool("isWithFries", value);
        }
    }
    private bool m_hasLeaved;
    private bool hasLeaved
    {
        get { return m_hasLeaved;}
        set
        {
            if(value && !m_hasLeaved)
            {
                animator.SetTrigger("Leaved");
            }
            m_hasLeaved = value;
        }
    }
    private BossState state;

    // 行动
    [SerializeField]
    private float flySpeed;
    [SerializeField]
    private float rushSpeed;
    [SerializeField]
    private float FriesSpeed;
    private Vector3 curDest;
    [Tooltip("判定到达的偏移范围")]
    [SerializeField]
    private float reachDestBias;
    [SerializeField]
    private PositionTracer tracer;
    [Tooltip("每次决策的时间间隔")]
    [SerializeField]
    private float decideTime;

    private Action attackAction;

    private Rigidbody2D rb;
    private Animator animator;
    [SerializeField]
    private Animator damagedAnimator;
    [SerializeField]
    private Animator attackHintAnimator;
    // public******************************************************************************
    public void FlyAttack()
    {
        attackHintAnimator.SetTrigger("Hint");
        // hint播完才能进攻
        attackAction = () =>{
            Debug.Log("Fly Attack");
            var dest = PickPlayerPos();
            FlyTo(dest + (dest - transform.position));
        };
    }

    public void RushAttack()
    {
        attackHintAnimator.SetTrigger("Hint");
        attackAction = () => {
            Debug.Log("Rush Attack");
            var dest = PickPlayerPos();
            RushTo(dest + (dest - transform.position));
        };
    }

    public void FriesAttack()
    {
        if(!isWithFries) return;
        attackHintAnimator.SetTrigger("Hint");
        animator.SetTrigger("FriesAttack");
        attackAction = () =>
        {
            Debug.Log("Fries Attack");
            Vector3 dest = PickPlayerPos();
            ThrowFriesTo(dest);
        };
    }

    // 由StateMachineBehaviour调用，在播放完攻击提示后，执行并清理当前的攻击
    public void ExecuteAttack()
    {
        attackAction?.Invoke();
        attackAction = null;
    }

    public void FlyTo(Vector3 dest)
    {
        UpdateCurrentDestination(dest);
        if(state != BossState.Leaving || hasLeaved)
        {
            hasLeaved = false;
            UpdateBossState(BossState.Flying);
        }
        animator.SetTrigger("Fly");
    }

    public void RushTo(Vector3 dest)
    {
        UpdateCurrentDestination(dest);
        UpdateBossState(BossState.Rushing);
        animator.SetTrigger("Rush");

    }

    public void HideOutside()
    {
        UpdateBossState(BossState.Leaving);
        Vector3 dest = tracer.PickNearestPos(PositionChoiceType.Outside);
        FlyTo(dest);
    }

    public void ThrowFriesTo(Vector3 dest)
    {
        var Fries = Instantiate(friesPrefab, friesRoot.position, Quaternion.identity);
        var dir = (dest - friesRoot.position).normalized;
        Fries.transform.LookAtDirection(dir);
        Fries.GetComponent<Rigidbody2D>().velocity = dir * FriesSpeed;
        isWithFries = false;
    }

    public void OnDamaged()
    {
        if (HealthManager.Instance.isInvincible) return;
        health--;
        Debug.Log("Boss Health decrease to " + health);
        healthPanel.HealthDecrease();
        if (health == 0)
        {
            // 死亡动画
            StopCoroutine(nameof(DecideCoolDown));
            animator.SetTrigger("AttackedToDeath");
        }
        else
        {
            // 受击动画
            damagedAnimator.SetTrigger("Attacked");
        }
    }

    // private******************************************************************************
    private void MakeDecision()
    {
        if(state == BossState.Rushing) return;
        Debug.Log("Making Decision");
        List<float> decides = new List<float>();
        for(int i = 0; i < 3; i++)
        {
            decides.Add(UnityEngine.Random.Range(0, 1.0f));
        }
        Vector3 dest;
        switch (state)
        {
            case BossState.Idle:
                if (decides[0] < 0.5f)
                {
                    if(decides[1] < 0.5f)
                    {
                        FlyAttack();
                    }
                    else
                    {
                        if(isWithFries)
                        {
                            FriesAttack();
                        }
                        else
                        {
                            RushAttack();
                        }
                    }
                }
                else if(decides[0] < 0.8f)
                {
                    if (decides[1] < 0.6f)
                    {
                        dest = tracer.PickNearestPos(PositionChoiceType.MidDistance);
                    }
                    else
                    {
                        dest = tracer.PickRandomPos(PositionChoiceType.MidDistance);
                    }
                    FlyTo(dest);
                }
                else
                {
                    // 耗尽薯条后才重新去取
                    if(!isWithFries)
                    {
                        HideOutside();
                    }
                    else
                    {
                        dest = tracer.PickFurtherestPos(PositionChoiceType.MidDistance);
                        FlyTo(dest);
                    }
                }
                break;
            case BossState.Flying:
                if(!isWithFries) break;
                if(decides[0] < 0.3f)
                {
                    // 在当前正在飞向玩家时才发射薯条
                    if(curDest - transform.position == PickPlayerPos() - transform.position)
                    {
                        FriesAttack();
                    }
                }
                break;
            case BossState.Leaving:
                if(!hasLeaved) break;
                Debug.Log("Pick Fries");
                isWithFries = true;
                if (decides[1] < 0.6f)
                {
                    dest = tracer.PickNearestPos(PositionChoiceType.MidDistance);
                }
                else
                {
                    dest = tracer.PickRandomPos(PositionChoiceType.MidDistance);
                }
                FlyTo(dest);
                break;
            default:
                return;
        }
    }

    private void Update()
    {
        if(!healthPanel.gameObject.activeSelf) return;
        float curDistance = Vector3.Distance(transform.position, curDest);
        bool reachDest = curDistance <= reachDestBias;
        Vector3 direction;
        if(reachDest)
        {
            animator.SetTrigger("Stop");
            direction = (PickPlayerPos() - transform.position).normalized;
        }
        else
        {
            direction = (curDest - transform.position).normalized;
        }
        transform.LookAtDirection(direction);

        switch (state)
        {
            case BossState.Idle:
                return;
            case BossState.Flying:
                if(reachDest)
                {
                    UpdateBossState(BossState.Idle);
                }
                else
                {
                    transform.position += flySpeed * Time.deltaTime * direction;
                }
                break;
            case BossState.Rushing:
                if (reachDest)
                {
                    UpdateBossState(BossState.Idle);
                }
                else
                {
                    transform.position += rushSpeed * Time.deltaTime * direction;
                }
                break;
            case BossState.Leaving:
                if (reachDest)
                {
                    Debug.Log("Has Leaved");
                    hasLeaved = true;
                }
                else
                {
                    transform.position += flySpeed * Time.deltaTime * direction;
                }
                break;
            default:
                return;
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        tracer.Init(transform);
        TypeEventSystem.Global.Register<GameStartEvent>(OnGameStart).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameWinEvent>(OnGameWin).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameLoseEvent>(OnGameLose).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<BubbleUpdateEvent>(OnBubbleUpdate).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnGameStart(GameStartEvent @event)
    {
        // 初始化生命值
        health = maxHealth;
        healthPanel.gameObject.SetActive(true);
        healthPanel.Init(maxHealth);

        // 初始化状态
        isWithFries = false;
        hasLeaved = false;
        UpdateBossState(BossState.Idle);
        curDest = PickPlayerPos();
        StartCoroutine(nameof(DecideCoolDown));
        attackAction = null;
    }

    private void OnGameWin(GameWinEvent @event)
    {
    }

    private void OnGameLose(GameLoseEvent @event)
    {
    }

    private void OnBubbleUpdate(BubbleUpdateEvent @event)
    {
        currentBubble = @event.bubble;
    }

    private void UpdateBossState(BossState state)
    {
        this.state = state;
        switch (state)
        {
            case BossState.Idle:
                preAttackFly.enabled = false;
                preAttackRush.enabled = false;
                break;
            case BossState.Flying:
                preAttackFly.enabled = true;
                preAttackRush.enabled = false;
                break;
            case BossState.Rushing:
                preAttackFly.enabled = false;
                preAttackRush.enabled = true;
                break;
            case BossState.Leaving:
                break;
            default:
                break;
        }
    }

    private Vector3 PickPlayerPos()
    {
        if(currentBubble == null) return Vector3.zero;
        return currentBubble.transform.position;
    }

    private void UpdateCurrentDestination(Vector3 dest)
    {
        dest = new Vector3(dest.x, dest.y, 0);
        curDest = dest;
    }

    private IEnumerator DecideCoolDown()
    {
        var randomTime = UnityEngine.Random.Range(0, decideTime);
        yield return new WaitForSeconds(randomTime);
        MakeDecision();
        StartCoroutine(nameof(DecideCoolDown));
    }
}
