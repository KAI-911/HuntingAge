using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Down_Trex : StateBase_Trex
{
    float downTime;

    public override void OnEnter(Trex owner, StateBase_Trex prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Down);
        owner.Animator.SetTrigger("Down");
        downTime = owner.DownTime;


    }
    public override void OnExit(Trex owner, StateBase_Trex nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Trex owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        downTime -= Time.deltaTime;
        if (downTime <= 0)
        {
            owner.ChangeState<Move_Trex>();
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
