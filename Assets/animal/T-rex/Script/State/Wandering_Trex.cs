using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering_Trex : StateBase
{
    private GameObject target;
    private RunOnce once = new RunOnce();
    float waitTime;
    public override void OnEnter(Enemy owner, StateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);

        while (true)//別の場所へ移動
        {
            target = owner.WaningPos[Random.Range(0, owner.WaningPos.Length)];
            var vec = target.transform.position - owner.transform.position;
            vec.y = 0;
            //移動先がある程度離れている場合ループを抜ける
            if (vec.sqrMagnitude > owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance)
            {
                break;
            }
        }

        waitTime = Random.Range(5f, 10f);
    }
    public override void OnExit(Enemy owner, StateBase nextState)
    {

    }
    public override void OnUpdate(Enemy owner)
    {
        Debug.Log("wandering");
        owner.NavMeshAgent.destination = target.transform.position;

        if (owner.Search())
        {
            owner.ChangeState<Move_Trex>();
            return;
        }
        var vec = target.transform.position - owner.transform.position;
        vec.y = 0;
        if (vec.magnitude < owner.NavMeshAgent.stoppingDistance * 1.2f)
        {
            //目標地点に来たら、停止する。
            //waitTime秒後、発見していなかったらもう一度徘徊する。
            once.Run(() =>
            {
                Debug.Log("onceRun");
                owner.Animator.SetInteger("AniState", (int)State.Idle);
                _ = owner.WaitForAsync(waitTime, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Trex>(); });
            });
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
