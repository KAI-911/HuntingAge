using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering_Deer : StateBase_Deer
{
    private Vector3 target;
    private RunOnce once = new RunOnce();
    int waitTime;
    float speed = 1.5f;
    float defaultSpeed;

    public override void OnEnter(Deer owner, StateBase_Deer prevState)
    { 
        defaultSpeed = owner.NavMeshAgent.speed;
        //歩く速度に変える
        owner.NavMeshAgent.speed = speed;
        owner.Animator.SetInteger("AniState", (int)State.Wandering);
        while (true)//別の場所へ移動
        {
            int num = Random.Range(0, owner.WaningPos.Count);
            target = owner.WaningPos[num];
            var vec = target - owner.transform.position;
            vec.y = 0;
            //移動先がある程度離れている場合ループを抜ける
            if (vec.sqrMagnitude > owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance)
            {
                break;
            }
        }
        waitTime = Random.Range(5, 10);
    }
    public override void OnExit(Deer owner, StateBase_Deer nextState)
    {

    }
    public override void OnUpdate(Deer owner)
    {
        owner.NavMeshAgent.destination = target;
        if (owner.ReceivedAttackCheck()) return;
        if (owner.Search())
        {
            owner.NavMeshAgent.speed = defaultSpeed;
            owner.ChangeState<Escape_Deer>();
            return;
        }
        var vec = target - owner.transform.position;
        vec.y = 0;
        if (vec.magnitude < owner.NavMeshAgent.stoppingDistance * 1.2f)
        {
            //目標地点に来たら、停止する。
            //waitTime秒後、発見していなかったらもう一度徘徊する。
            once.Run(() =>
            {
                owner.Animator.SetInteger("AniState", (int)State.Idle);
                _ = owner.WaitForAsync(waitTime, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Deer>(); });
            });
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
