using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Wolf : StateBase
{
    public override void OnEnter(Enemy owner, StateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Idle);
        _ = owner.WaitForAsync(5, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Wolf>(); });
    }
    public override void OnExit(Enemy owner, StateBase nextState)
    {

    }
    public override void OnUpdate(Enemy owner)
    {
        Debug.Log("idle");
        owner.NavMeshAgent.destination = owner.transform.position;

        if (owner.Search())
        {
            owner.ChangeState<Move_Wolf>();
            return;
        }

    }
    public override void OnFixedUpdate(Enemy owner)
    {

    }
    public override void OnAnimationEvent(Enemy owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Enemy owner, Collision collision)
    {

    }
}
