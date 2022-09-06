using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Rhino : StateBase_Rhino
{
    public override void OnEnter(Rhino owner, StateBase_Rhino prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Idle);
        _ = owner.WaitForAsync(5, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Rhino>(); });
    }
    public override void OnExit(Rhino owner, StateBase_Rhino nextState)
    {

    }
    public override void OnUpdate(Rhino owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        if (owner.ReceivedAttackCheck()) return;

        if (owner.Search())
        {
            owner.ChangeState<Move_Rhino>();
            return;
        }

    }
    public override void OnFixedUpdate(Rhino owner)
    {

    }
    public override void OnAnimationEvent(Rhino owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Rhino owner, Collision collision)
    {

    }
}
