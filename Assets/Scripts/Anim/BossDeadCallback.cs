using QFramework;
using UnityEngine;

public class BossDeadCallback : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TypeEventSystem.Global.Send<GameWinEvent>();
    }
}