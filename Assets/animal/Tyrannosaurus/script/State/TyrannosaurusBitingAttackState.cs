using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusBitingAttackState : TyrannosaurusState
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {
        owner.Animator.SetInteger("AniState", (int)TyrannosaurusAnimationState.BitingAttack);
        owner.HitReceiver.HitReaction = HitReaction.middleReaction;
    }
    public override void OnExit(Tyrannosaurus owner, TyrannosaurusState nextState)
    {

    }
    public override void OnUpdate(Tyrannosaurus owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
    }
    public override void OnFixedUpdate(Tyrannosaurus owner)
    {

    }
    public override void OnAnimationEvent(Tyrannosaurus owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter== "Change")
        {
            //“–‚½‚è”»’è‚ð”½“]
            owner.HitReceiver.ChangeAttackFlg(PartType.head);
        }
        else if (animationEvent.stringParameter=="End")
        {
            owner.HitReceiver.AttackFlgReset();
            owner.ChangeState<TyrannosaurusMoveState>();

        }
    }
    public override void OnCollisionStay(Tyrannosaurus owner, Collision collision)
    {

    }
}
