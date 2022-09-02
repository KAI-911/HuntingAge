using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Dodo : StateBase_Dodo
{
    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Idle);
        _ = owner.WaitForAsync(5, () => { if (owner.CurrentState.GetType() == typeof(Idle_Dodo)) owner.ChangeState<Wandering_Dodo>(); });
    }
    public override void OnExit(Dodo owner, StateBase_Dodo nextState)
    {

    }
    public override void OnUpdate(Dodo owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        
        //if (owner.Search())
        //{
        //    owner.ChangeState<Move_Trex>();
        //    return;
        //}

    }
    public override void OnFixedUpdate(Dodo owner)
    {

    }
    public override void OnAnimationEvent(Dodo owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Dodo owner, Collision collision)
    {

    }
}
