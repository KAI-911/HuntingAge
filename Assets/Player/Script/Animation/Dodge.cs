using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodge : StateMachineBehaviour
{
    Player player;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.GetComponent<Player>();
        player.moveDirection = player.moveDirection.normalized;
        player.moveDirection *= player.nowMaxSpeed;
        

    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {            
        if (stateInfo.normalizedTime > 0.5f)
        {
            player.moveDirection *= 0.9f;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

}
