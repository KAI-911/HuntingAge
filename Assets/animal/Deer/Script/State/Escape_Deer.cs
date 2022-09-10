using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_Deer : StateBase_Deer
{
    private Vector3 _escapePos;
    private RunOnce _runOnce;
    float _time;
    float speed = 4.5f;
    float defaultSpeed;

    public override void OnEnter(Deer owner, StateBase_Deer prevState)
    {
        defaultSpeed = owner.NavMeshAgent.speed;
        //逃げる速度に変更
        owner.NavMeshAgent.speed = speed;

        owner.Animator.SetInteger("AniState", (int)State.Escape);
        owner.Animator.Play("escape", 0, Random.Range(0f, 1f));
        float farDis =0;
        //一番遠い逃げる場所を目指す
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
        //到達したら待機
        if ((owner.NavMeshAgent.destination - owner.transform.position).sqrMagnitude < (owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance))
        {
            owner.NavMeshAgent.speed = defaultSpeed;
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
