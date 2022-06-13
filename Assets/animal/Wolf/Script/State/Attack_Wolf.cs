using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Wolf : StateBase
{
    public override void OnEnter(Enemy owner, StateBase prevState)
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
    public override void OnExit(Enemy owner, StateBase nextState)
    {
        owner.HitReceiver.AttackFlgReset();
    }
    public override void OnUpdate(Enemy owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));

    }
    public override void OnFixedUpdate(Enemy owner)
    {

    }
    public override void OnAnimationEvent(Enemy owner, AnimationEvent animationEvent)
    {
        Debug.Log("attack");
        if (animationEvent.stringParameter == "Change")
        {
            //�����蔻��𔽓]
            owner.HitReceiver.ChangeAttackFlg((PartType)animationEvent.intParameter);
        }
        if (animationEvent.stringParameter == "End")
        {
            owner.ChangeState<Move_Wolf>();
        }
    }
    public override void OnCollisionStay(Enemy owner, Collision collision)
    {

    }
}
