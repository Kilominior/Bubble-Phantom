using QFramework;
using UnityEngine;

public class PlayerPosHint: MonoBehaviour
{
    private Transform targetTrans;
    [SerializeField]
    private float floatBias;
// public******************************************************************************

// private******************************************************************************

    private void Update()
    {
        if(targetTrans == null) return;
        var targetPos = targetTrans.position + floatBias * Vector3.up;
        if (SceneEdge.Instance.IsPositionLegal(targetTrans.position))
        {
            transform.position = targetPos;
            return;
        }
        transform.position = SceneEdge.Instance.ClampPositionIntoLegal(targetTrans.position);
        transform.LookAtDirection(targetPos - transform.position);
    }

    private void Awake()
    {
        TypeEventSystem.Global.Register<GameStartEvent>(OnGameStart).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameWinEvent>(OnGameWin).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<GameLoseEvent>(OnGameLose).UnRegisterWhenGameObjectDestroyed(this);
        TypeEventSystem.Global.Register<BubbleUpdateEvent>(OnBubbleUpdate).UnRegisterWhenGameObjectDestroyed(this);
    }

    private void OnGameStart(GameStartEvent @event)
    {
    }

    private void OnGameWin(GameWinEvent @event)
    {
    }

    private void OnGameLose(GameLoseEvent @event)
    {
    }

    private void OnBubbleUpdate(BubbleUpdateEvent @event)
    {
        targetTrans = @event.bubble.transform;
    }
}
