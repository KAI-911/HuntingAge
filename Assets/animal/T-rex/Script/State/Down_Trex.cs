using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Down_Trex : StateBase
{
    float downTime;

    public override void OnEnter(Enemy owner, StateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Down);
        owner.Animator.SetTrigger("Down");
        downTime = owner.DownTime;


    }
    public override void OnExit(Enemy owner, StateBase nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Enemy owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        downTime -= Time.deltaTime;
        if (downTime <= 0)
        {
            owner.ChangeState<Move_Trex>();
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
