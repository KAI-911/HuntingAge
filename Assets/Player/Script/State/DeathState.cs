using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : PlayerStateBase
{
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Death);
        owner.Animator.SetTrigger("Change");
    }

    public override void OnExit(Player owner, PlayerStateBase nextState)
    {
    }

    public override void OnUpdate(Player owner)
    {

    }

    public override void OnFixedUpdate(Player owner)
    {

    }

    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {

    }

    public override void OnCollisionStay(Player owner, Collision collision)
    {

    }
}
