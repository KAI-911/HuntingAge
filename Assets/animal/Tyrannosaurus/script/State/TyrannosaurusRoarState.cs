using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusRoarState : TyrannosaurusState
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {
        owner.Animator.SetInteger("AniState", (int)TyrannosaurusAnimationState.Roar);
        
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
        //�U���͈͂Ƀ^�[�Q�b�g������΍U����ԂɈڍs

        //�����łȂ���΋߂Â�
        owner.ChangeState<TyrannosaurusMoveState>();
    }
    public override void OnCollisionStay(Tyrannosaurus owner, Collision collision)
    {

    }
}
