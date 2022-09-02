using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Trex : StateBase_Trex
{
    public override void OnEnter(Trex owner, StateBase_Trex prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Attack);

        TargetCheckerType type= TargetCheckerType.Non;
        foreach (var item in owner.TargetChecker())
        {
            if (item == TargetCheckerType.Search) continue;
            owner.Animator.SetInteger("AttackType", (int)item);
            type = item;
            break;
        }
        switch (type)
        {
            case TargetCheckerType.Biting:
                owner.HitReceiver.HitReaction = HitReaction.middleReaction;
                break;
            case TargetCheckerType.Stomp:
                owner.HitReceiver.HitReaction = HitReaction.middleReaction;
                break;
            case TargetCheckerType.Tail:
                owner.HitReceiver.HitReaction = HitReaction.lowReaction;
                break;
            default:
                owner.HitReceiver.HitReaction = HitReaction.nonReaction;
                break;
        }


    }
    public override void OnExit(Trex owner, StateBase_Trex nextState)
    {
        owner.HitReceiver.AttackFlgReset();
    }
    public override void OnUpdate(Trex owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));

    }
    public override void OnFixedUpdate(Trex owner)
    {

    }
    public override void OnAnimationEvent(Trex owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Change")
        {
            //“–‚½‚è”»’è
            owner.HitReceiver.ChangeAttackFlg((PartType)animationEvent.intParameter);
        }
        if (animationEvent.stringParameter == "End")
        {
            owner.ChangeState<Move_Trex>();
        }
    }
    public override void OnCollisionStay(Trex owner, Collision collision)
    {

    }
}
