using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : StateMachineBehaviour
{
    Player player;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<Player>();

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //”w’†‚©‚ç’n–Ê‚É‚Â‚­‚½‚ßˆÚ“®•ûŒü‚Ì”½‘Î‚ÉŒü‚­
        var lookDir = -player.moveDirection;
        lookDir.y = 0;
        player.Look(lookDir, 0.1f);

        float par = 1 - (stateInfo.normalizedTime + 0.1f);
        if (par < 0) par = 0;
        player.moveDirection *= par;

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //‹N‚«ã‚ª‚èanimation‚ÉˆÚs
        //state‚Íˆø‚«‘±‚«PlayerKnockBackState
    }

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
}
