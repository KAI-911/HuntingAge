using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Rhino : StateBase_Rhino
{
    TargetCheckerType type = TargetCheckerType.Non;

    float speed = 10.0f;
    float defaultSpeed;

    public override void OnEnter(Rhino owner, StateBase_Rhino prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Attack);

        defaultSpeed = owner.NavMeshAgent.speed;

        Debug.Log("çUåÇîªíË");
        foreach (var item in owner.TargetChecker())
        {
            Debug.Log(item);
        }
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
            case TargetCheckerType.Rush:
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
        if (type == TargetCheckerType.Rush)
        {
            if ((owner.NavMeshAgent.destination - owner.transform.position).sqrMagnitude <= (owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance))
            {
                owner.HitReceiver.ChangeAttackFlg(PartType.head);
                owner.Animator.SetInteger("AttackType", (int)TargetCheckerType.Gore);
                type = TargetCheckerType.Gore;
                owner.NavMeshAgent.speed = defaultSpeed;
            }
        }
        else
        {
            Debug.Log("âøílè„Ç∞");
            owner.NavMeshAgent.destination = owner.transform.position;
        }
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));

    }
    public override void OnFixedUpdate(Rhino owner)
    {

    }
    public override void OnAnimationEvent(Rhino owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Change")
        {
            //ìñÇΩÇËîªíË
            owner.HitReceiver.ChangeAttackFlg((PartType)animationEvent.intParameter);
        }
        if (animationEvent.stringParameter == "End")
        {
            owner.ChangeState<Move_Rhino>();
        }

        if (animationEvent.stringParameter == "Ready")
        {
            var pos = UISoundManager.Instance._player.transform.position;
            pos += (pos - owner.transform.position).normalized * owner.NavMeshAgent.stoppingDistance * 3.0f;
            owner.NavMeshAgent.destination = pos;

            //ìÀêiÇÃë¨ìxÇ…Ç∑ÇÈ
            owner.NavMeshAgent.speed = speed;
        }
    }
    public override void OnCollisionStay(Rhino owner, Collision collision)
    {

    }
}
