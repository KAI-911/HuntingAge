using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_Wolf : SatetBase_Wolf
{
    public override void OnEnter(Wolf owner, SatetBase_Wolf prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Idle);
        _ = owner.WaitForAsync(5, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Wolf>(); });
    }
    public override void OnExit(Wolf owner, SatetBase_Wolf nextState)
    {

    }
    public override void OnUpdate(Wolf owner)
    {
        Debug.Log("idle");
        owner.NavMeshAgent.destination = owner.transform.position;

        if (owner.Search())
        {
            owner.ChangeState<Move_Wolf>();
            return;
        }

    }
    public override void OnFixedUpdate(Wolf owner)
    {

    }
    public override void OnAnimationEvent(Wolf owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Wolf owner, Collision collision)
    {

    }
}
