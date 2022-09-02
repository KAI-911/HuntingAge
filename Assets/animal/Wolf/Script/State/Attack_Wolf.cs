using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Wolf : SatetBase_Wolf
{
    public override void OnEnter(Wolf owner, SatetBase_Wolf prevState)
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
            case TargetCheckerType.Biting:
                owner.HitReceiver.HitReaction = HitReaction.lowReaction;
                break;
            default:
                owner.HitReceiver.HitReaction = HitReaction.nonReaction;
                break;
        }


    }
    public override void OnExit(Wolf owner, SatetBase_Wolf nextState)
    {
        owner.HitReceiver.AttackFlgReset();
    }
    public override void OnUpdate(Wolf owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));

    }
    public override void OnFixedUpdate(Wolf owner)
    {

    }
    public override void OnAnimationEvent(Wolf owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Change")
        {
            //“–‚½‚è”»’è
            owner.HitReceiver.ChangeAttackFlg((PartType)animationEvent.intParameter);
        }
        if (animationEvent.stringParameter == "End")
        {
            owner.ChangeState<Move_Wolf>();
        }
    }
    public override void OnCollisionStay(Wolf owner, Collision collision)
    {

    }
}
