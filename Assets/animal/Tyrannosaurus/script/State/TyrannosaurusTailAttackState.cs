using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusTailAttackState : TyrannosaurusState
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {
        owner.Animator.SetInteger("AniState", (int)TyrannosaurusAnimationState.TailAttack);

    }
    public override void OnExit(Tyrannosaurus owner, TyrannosaurusState nextState)
    {

    }
    public override void OnUpdate(Tyrannosaurus owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget();
    }
    public override void OnFixedUpdate(Tyrannosaurus owner)
    {

    }
    public override void OnAnimationEvent(Tyrannosaurus owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Change")
        {
            //“–‚½‚è”»’è‚ð”½“]
            owner.HitReceiver.ChangeAttackFlg(PartType.tail);
        }
        if (animationEvent.stringParameter == "End")
        {
            owner.HitReceiver.AttackFlgReset();
            owner.ChangeState<TyrannosaurusMoveState>();
        }

    }
    public override void OnCollisionStay(Tyrannosaurus owner, Collision collision)
    {

    }
}
