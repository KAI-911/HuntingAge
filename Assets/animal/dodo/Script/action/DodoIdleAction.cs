using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodoIdleAction : StateMachineBehaviour
{
    private RunOnce runOnce;
    [SerializeField] int GroomingPercent;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        runOnce = new RunOnce();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float time = stateInfo.normalizedTime % 1;
        if (time > 0.9)
        {
            runOnce.Run(() =>
            {
                var tmp = Random.Range(0, 100);
                if (tmp > GroomingPercent) animator.SetFloat("IdleBlend", (int)Idle.Grooming);
                else animator.SetFloat("IdleBlend", (int)Idle.Overlooking);
            });
        }
        else
        {
            runOnce.Flg = false;
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    enum Idle
    {
        Grooming,
        Overlooking
    }


}
