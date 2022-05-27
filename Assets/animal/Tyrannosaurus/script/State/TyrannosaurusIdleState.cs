using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusIdleState : TyrannosaurusState
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {
        owner.Animator.SetInteger("AniState", (int)TyrannosaurusAnimationState.Idle);
        _ = owner.WaitForAsync(5, () => owner.WarningFlg = true);
    }
    public override void OnExit(Tyrannosaurus owner, TyrannosaurusState nextState)
    {

    }
    public override void OnUpdate(Tyrannosaurus owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget();
        if (owner.Search() || owner.WarningFlg)
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
