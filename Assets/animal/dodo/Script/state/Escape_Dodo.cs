using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_Dodo : StateBase_Dodo
{
    private Vector3 escapePos;
    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Escape);
        owner.Animator.Play("escape", 0, Random.Range(0f, 1f));
        float nearDis = float.MaxValue;
        //ˆê”Ô‹ß‚¢“¦‚°‚éêŠ‚ð–ÚŽw‚·
        foreach (var pos in owner.EscapePos)
        {
            var dis = (owner.transform.position - pos).magnitude;
            if (nearDis > dis)
            {
                escapePos = pos;
                nearDis = dis;
            }
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
