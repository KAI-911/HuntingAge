using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitReactionState : PlayerStateBase
{
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.HitReaction);
        owner.Animator.SetInteger("HitReactionType", (int)owner.Status.HitReaction);
        owner.Animator.SetTrigger("Change");
    }

    public override void OnExit(Player owner, PlayerStateBase nextState)
    {
        owner.Animator.SetInteger("HitReactionType", (int)HitReaction.nonReaction);

    }

    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "End")
        {
            owner.Status.HitReactionReset();
            owner.ChangeState<LocomotionState>();
        }

    }


}
