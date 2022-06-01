using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusDownState : TyrannosaurusState
{
    float downTime; 
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {
        owner.Animator.SetInteger("AniState", (int)TyrannosaurusAnimationState.Down);
        owner.Animator.SetTrigger("Down");
        downTime = owner.DownTime;
    }
    public override void OnExit(Tyrannosaurus owner, TyrannosaurusState nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Tyrannosaurus owner)
    {
        downTime -= Time.deltaTime;
        if(downTime<0)
        {
            owner.ChangeState<TyrannosaurusMoveState>();
        }
    }
    public override void OnFixedUpdate(Tyrannosaurus owner)
    {

    }
    public override void OnAnimationEvent(Tyrannosaurus owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Tyrannosaurus owner, Collision collision)
    {

    }
}
