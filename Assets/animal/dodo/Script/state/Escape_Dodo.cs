using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_Dodo : StateBase_Dodo
{
    private Vector3 escapePos;
    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Escape);
        float nearDis = float.MaxValue;
        //一番近い逃げる場所を目指す
        foreach (var pos in owner.EscapePos)
        {
            var dis = (owner.transform.position - pos).magnitude;
            if (nearDis > dis) escapePos = pos;
        }
        owner.NavMeshAgent.destination = escapePos;

    }
    public override void OnExit(Dodo owner, StateBase_Dodo nextState)
    {

    }
    public override void OnUpdate(Dodo owner)
    {
        Debug.Log("Escape");

    }
    public override void OnFixedUpdate(Dodo owner)
    {

    }
    public override void OnAnimationEvent(Dodo owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Dodo owner, Collision collision)
    {

    }
}
