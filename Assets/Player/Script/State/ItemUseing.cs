using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseing : PlayerStateBase
{
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.ItemUseing);
        owner.Animator.SetTrigger("Change");
    }
    public override void OnExit(Player owner, PlayerStateBase nextState)
    {

    }

    public override void OnDodge(Player owner)
    {
        if (!owner.GroundChecker.IsGround()) return;
        owner.ChangeState<DodgeState>();
    }
    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Use")
        {
            GameManager.Instance.UIItemView.ItemUseEnd();
        }
        else if (animationEvent.stringParameter == "End")//アニメーション終了
        {

            owner.ChangeState<LocomotionState>();
        }
    }
}
