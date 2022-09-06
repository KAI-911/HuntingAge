using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Rhino : StateBase_Rhino
{
    public override void OnEnter(Rhino owner, StateBase_Rhino prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Attack);

        TargetCheckerType type = TargetCheckerType.Non;
        foreach (var item in owner.TargetChecker())
        {
            if (item == TargetCheckerType.Search) continue;
            owner.Animator.SetInteger("AttackType", (int)item);
            type = item;
            break;
        }
        switch (type)
        {
            case TargetCheckerType.Gore:
                owner.HitReceiver.HitReaction = HitReaction.middleReaction;
                break;
            default:
                owner.HitReceiver.HitReaction = HitReaction.nonReaction;
                break;
        }


    }
    public override void OnExit(Rhino owner, StateBase_Rhino nextState)
    {
        owner.HitReceiver.AttackFlgReset();
    }
    public override void OnUpdate(Rhino owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));

    }
    public override void OnFixedUpdate(Rhino owner)
    {

    }
    public override void OnAnimationEvent(Rhino owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Change")
        {
            //“–‚½‚è”»’è
            owner.HitReceiver.ChangeAttackFlg((PartType)animationEvent.intParameter);
        }
        if (animationEvent.stringParameter == "End")
        {
            owner.ChangeState<Move_Rhino>();
        }
    }
    public override void OnCollisionStay(Rhino owner, Collision collision)
    {

    }
}
