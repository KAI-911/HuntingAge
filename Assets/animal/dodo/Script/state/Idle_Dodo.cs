using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Dodo : StateBase_Dodo
{
    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Idle);
        _ = owner.WaitForAsync(5, () => { Debug.Log("�p�j"); owner.ChangeState<Wandering_Dodo>(); });
    }
    public override void OnExit(Dodo owner, StateBase_Dodo nextState)
    {

    }
    public override void OnUpdate(Dodo owner)
    {
        Debug.Log("idle");
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
        if (animationEvent.stringParameter == "End")
        {
            var tmp = Random.Range(0, 100);
            //if (tmp > 50) owner.Animator.SetFloat("IdleBlend", (int)Idle.Grooming);
            //else owner.Animator.SetFloat("IdleBlend", (int)Idle.Overlooking);
        }
    }
    public override void OnCollisionStay(Dodo owner, Collision collision)
    {

    }
}
