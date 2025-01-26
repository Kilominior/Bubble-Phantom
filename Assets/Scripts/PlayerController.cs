using System.Collections;
using Cinemachine;
using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    private bool _isMoving;
    private bool isMoving
    {
        get { return _isMoving; }
        set
        {
            _isMoving = value;
            if(currentBubble != null)
                currentBubble.SetMoveStatus(value);
        }
    }
    private bool isDodging;

    // 泡泡
    private Bubble currentBubble;
    [SerializeField]
    private float rebirthTime;
    private WaitForSeconds WaitForRebirth;
    [SerializeField]
    private float loseDelayTime;
    private WaitForSeconds WaitForLose;

    // 行动
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float maxMoveVelocity;
    [SerializeField]
    private float dodgeSpeed;
    [SerializeField]
    private float dodgeTime;

    // 输入
    private PlayerInput input;
    private InputActionMap iam;
    private Vector3 moveDir;

    // 其他
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    // public******************************************************************************

    // private******************************************************************************

    private void Start()
    {
        input = GetComponent<PlayerInput>();
        iam = input.actions.actionMaps[0];

        TypeEventSystem.Global.Register<PlayerDeathEvent>(OnPlayerDeath).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<PlayerRebirthEvent>(OnPlayerRebirth).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameWinEvent>(OnGameWin).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameStartEvent>(OnGameStart).UnRegisterWhenGameObjectDestroyed(this);

        Initialize();
    }

    private void Initialize()
    {
        moveDir = Vector2.down;
        isMoving = false;
        isDodging = false;
        WaitForRebirth = new WaitForSeconds(rebirthTime);
        WaitForLose = new WaitForSeconds(loseDelayTime);
        iam["Exit"].performed += OnExit;
    }

    private void OnDestroy()
    {
        iam["Exit"].performed -= OnExit;
        IAUnbinding();
    }

    private void Update()
    {
        Move();
    }

    private void OnPlayerDeath(PlayerDeathEvent @event)
    {
        IAUnbinding();
        TryRebirth();
    }

    private void OnPlayerRebirth(PlayerRebirthEvent @event)
    {
        IABinding();
    }

    private void OnGameWin(GameWinEvent @event)
    {
    }

    private void OnGameStart(GameStartEvent @event)
    {
        isMoving = false;
        isDodging = false;
        TryRebirth();
    }

    private void OnExit(InputAction.CallbackContext context)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // 更新当前主控泡泡，条件允许复活时调用
    private void UpdateBubble(Bubble bubble)
    {
        Debug.Log("update current bubble");
        currentBubble = bubble;
        TypeEventSystem.Global.Send(new BubbleUpdateEvent(currentBubble));
        currentBubble.GetControlled(maxMoveVelocity, dodgeTime, onDodgeEnd);
        virtualCamera.Follow = currentBubble.transform;

        StartCoroutine(nameof(RebirthDelay));
    }

    private void TryRebirth()
    {
        Bubble rebirthBubble = BubbleManager.Instance.QueryAvailableBubble();
        if (rebirthBubble == null)
        {
            StartCoroutine(nameof(LoseDelay));
            return;
        }
        UpdateBubble(rebirthBubble);
    }

    private IEnumerator RebirthDelay()
    {
        yield return WaitForRebirth;
        TypeEventSystem.Global.Send<PlayerRebirthEvent>();
    }

    private IEnumerator LoseDelay()
    {
        yield return WaitForLose;
        TypeEventSystem.Global.Send<GameLoseEvent>();
    }

    #region Control
    private void Move()
    {
        if (currentBubble == null) return;
        if (isDodging) return;
        if (isMoving)
        {
            currentBubble.Move(moveSpeed * Time.deltaTime * moveDir);
        }
    }

    private void Dodge()
    {
        if (isDodging) return;
        isDodging = true;
        currentBubble.Dodge(dodgeSpeed * moveDir);
    }

    private void onDodgeEnd()
    {
        isDodging = false;
    }

    #endregion


    #region Input
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>().normalized;
        isMoving = true;
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
        moveDir = Vector2.down;
    }

    private void OnDodgePerformed(InputAction.CallbackContext context)
    {
        Dodge();
    }

    private void IABinding()
    {
        iam["Move"].performed += OnMovePerformed;
        iam["Move"].canceled += OnMoveCanceled;
        iam["Dodge"].performed += OnDodgePerformed;
    }

    private void IAUnbinding()
    {
        isMoving = false;
        isDodging = false;
        moveDir = Vector2.down;
        iam["Move"].performed -= OnMovePerformed;
        iam["Move"].canceled -= OnMoveCanceled;
        iam["Dodge"].performed -= OnDodgePerformed;
    }

    #endregion

}
