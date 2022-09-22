using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Trex : StateBase_Trex
{
    public override void OnEnter(Trex owner, StateBase_Trex prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Idle);
        _ = owner.WaitForAsync(5, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Trex>(); });
    }
    public override void OnExit(Trex owner, StateBase_Trex nextState)
    {

    }
    public override void OnUpdate(Trex owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        if (owner.ReceivedAttackCheck()) return;

        if (owner.Search())
        {
            owner.ChangeState<Roar_Trex>();
            return;
        }

    }
    public override void OnFixedUpdate(Trex owner)
    {

    }
    public override void OnAnimationEvent(Trex owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Trex owner, Collision collision)
    {

    }
}
