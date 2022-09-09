using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Deer : StateBase_Deer
{
    public override void OnEnter(Deer owner, StateBase_Deer prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Idle);
        _ = owner.WaitForAsync(5, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Deer>(); });
    }
    public override void OnExit(Deer owner, StateBase_Deer nextState)
    {

    }
    public override void OnUpdate(Deer owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        if (owner.ReceivedAttackCheck()) return;

        if (owner.Search())
        {
            owner.ChangeState<Move_Deer>();
            return;
        }

    }
    public override void OnFixedUpdate(Deer owner)
    {

    }
    public override void OnAnimationEvent(Deer owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Deer owner, Collision collision)
    {

    }
}
