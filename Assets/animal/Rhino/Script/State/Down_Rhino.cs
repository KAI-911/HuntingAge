using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Down_Rhino : StateBase_Rhino
{
    float downTime;

    public override void OnEnter(Rhino owner, StateBase_Rhino prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Down);
        owner.Animator.SetTrigger("Down");
        downTime = owner.DownTime;


    }
    public override void OnExit(Rhino owner, StateBase_Rhino nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Rhino owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        downTime -= Time.deltaTime;
        if (downTime <= 0)
        {
            owner.ChangeState<Move_Rhino>();
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
