using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHintCompleteBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.GetComponentInParent<BossController>().ExecuteAttack();
    }
}
