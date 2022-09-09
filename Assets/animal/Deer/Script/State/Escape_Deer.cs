using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_Deer : StateBase_Deer
{
    private Vector3 _escapePos;
    private RunOnce _runOnce;
    float _time;

    public override void OnEnter(Deer owner, StateBase_Deer prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Escape);
        owner.Animator.Play("escape", 0, Random.Range(0f, 1f));
        float farDis =0;
        //ˆê”Ô‰“‚¢“¦‚°‚éêŠ‚ğ–Úw‚·
        foreach (var pos in owner.WaningPos)
        {
            var dis = (owner.transform.position - pos).magnitude;
            if (farDis < dis)
            {
                _escapePos = pos;
                farDis = dis;
            }
        }
        owner.NavMeshAgent.destination = _escapePos;

        _runOnce = new RunOnce();
    }
    public override void OnExit(Deer owner, StateBase_Deer nextState)
    {

    }
    public override void OnUpdate(Deer owner)
    {
        //“’B‚µ‚½‚ç‘Ò‹@
        if ((owner.NavMeshAgent.destination - owner.transform.position).sqrMagnitude < (owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance))
        {
            owner.ChangeState<Idle_Deer>();
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
