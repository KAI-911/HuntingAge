using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Down_Wolf : SatetBase_Wolf
{
    float downTime;

    public override void OnEnter(Wolf owner, SatetBase_Wolf prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Down);
        owner.Animator.SetTrigger("Down");
        downTime = owner.DownTime;


    }
    public override void OnExit(Wolf owner, SatetBase_Wolf nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Wolf owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;
        downTime -= Time.deltaTime;
        if (downTime <= 0)
        {
            owner.ChangeState<Move_Wolf>();
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
