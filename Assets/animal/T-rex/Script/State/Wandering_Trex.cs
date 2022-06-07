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

        while (true)//�ʂ̏ꏊ�ֈړ�
        {
            target = owner.WaningPos[Random.Range(0, owner.WaningPos.Length)];
            var vec = target.transform.position - owner.transform.position;
            vec.y = 0;
            //�ړ��悪������x����Ă���ꍇ���[�v�𔲂���
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
            //�ڕW�n�_�ɗ�����A��~����B
            //waitTime�b��A�������Ă��Ȃ������������x�p�j����B
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
