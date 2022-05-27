using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusMoveState : TyrannosaurusState
{
    private GameObject target;
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {
        owner.Animator.SetInteger("AniState", (int)TyrannosaurusAnimationState.Move);
        target = owner.Target;
        //•Ê‚ÌêŠ‚ÖˆÚ“®
        if (owner.WarningFlg)
        {
            while (true)
            {
                target = owner.WaningPos[Random.Range(0, owner.WaningPos.Length)];
                var vec = target.transform.position - owner.transform.position;
                vec.y = 0;
                if (vec.sqrMagnitude > owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance)
                {
                    break;
                }
            }
        }
    }
    public override void OnExit(Tyrannosaurus owner, TyrannosaurusState nextState)
    {

    }
    public override void OnUpdate(Tyrannosaurus owner)
    {
        owner.NavMeshAgent.destination = target.transform.position;
        //UŒ‚‚ÉˆÚs
        if(owner.BitingAttackArea.TriggerHit) owner.ChangeState<TyrannosaurusBitingAttackState>();
        if(owner.TailAttackArea.TriggerHit) owner.ChangeState<TyrannosaurusTailAttackState>();

        if (owner.WarningFlg)
        {
                //ƒvƒŒƒCƒ„[‚ğŒ©‚Â‚¯‚é‚ÆˆĞŠd‚·‚é
            if (owner.Search())
            {
                owner.ChangeState<TyrannosaurusRoarState>();
                owner.WarningFlg = false;
            }

            //œpœj’†‚É–Ú“I‚ÌêŠ‚É“’B‚·‚é‚Æ’â~
            var vec = target.transform.position - owner.transform.position;
            vec.y = 0;
            if (vec.sqrMagnitude < owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance)
            {
                owner.WarningFlg = false;
                owner.ChangeState<TyrannosaurusIdleState>();
            }
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
