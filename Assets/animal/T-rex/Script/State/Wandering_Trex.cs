using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering_Trex : StateBase_Trex
{
    private Vector3 target;
    private RunOnce once = new RunOnce();
    int waitTime;
    public override void OnEnter(Trex owner, StateBase_Trex prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);
        while (true)//別の場所へ移動
        {
            int num = Random.Range(0, owner.WaningPos.Length - 1);
            target = owner.WaningPos[num].transform.position;
            var vec = target - owner.transform.position;
            vec.y = 0;
            //移動先がある程度離れている場合ループを抜ける
            if (vec.sqrMagnitude > owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance)
            {
                break;
            }
        }
        Debug.Log(target);
        waitTime = Random.Range(5, 10);
    }
    public override void OnExit(Trex owner, StateBase_Trex nextState)
    {

    }
    public override void OnUpdate(Trex owner)
    {
        Debug.Log("wandering");
        owner.NavMeshAgent.destination = target;

        if (owner.Search())
        {
            owner.ChangeState<Move_Trex>();
            return;
        }
        var vec = target - owner.transform.position;
        vec.y = 0;
        Debug.Log("距離" + vec.magnitude + "　　 停止距離" + owner.NavMeshAgent.stoppingDistance * 1.2f);
        if (vec.magnitude < owner.NavMeshAgent.stoppingDistance * 1.2f)
        {
            Debug.Log("近くまで来た");
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
