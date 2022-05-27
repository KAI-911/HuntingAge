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
        //別の場所へ移動
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
        //攻撃に移行
        if(owner.BitingAttackArea.TriggerHit) owner.ChangeState<TyrannosaurusBitingAttackState>();
        if(owner.TailAttackArea.TriggerHit) owner.ChangeState<TyrannosaurusTailAttackState>();

        if (owner.WarningFlg)
        {
                //プレイヤーを見つけると威嚇する
            if (owner.Search())
            {
                owner.ChangeState<TyrannosaurusRoarState>();
                owner.WarningFlg = false;
            }

            //徘徊中に目的の場所に到達すると停止
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
