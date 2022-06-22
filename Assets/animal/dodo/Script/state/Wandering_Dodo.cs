using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering_Dodo : StateBase_Dodo
{
    private Vector3 target;
    private RunOnce once = new RunOnce();
    int waitTime;

    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);
        while (true)//�ʂ̏ꏊ�ֈړ�
        {
            int num = Random.Range(0, owner.WaningPos.Count);
            target = owner.WaningPos[num];
            var vec = target - owner.transform.position;
            vec.y = 0;
            //�ړ��悪������x����Ă���ꍇ���[�v�𔲂���
            if (vec.sqrMagnitude > owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance)
            {
                break;
            }
        }
        waitTime = Random.Range(5, 10);

    }
    public override void OnExit(Dodo owner, StateBase_Dodo nextState)
    {

    }
    public override void OnUpdate(Dodo owner)
    {
        owner.NavMeshAgent.destination = target;

        var vec = target - owner.transform.position;
        vec.y = 0;
        if (vec.magnitude < owner.NavMeshAgent.stoppingDistance * 1.2f)
        {
            //�ڕW�n�_�ɗ�����A��~����B
            //waitTime�b��A�������Ă��Ȃ������������x�p�j����B
            once.Run(() =>
            {
                owner.Animator.SetInteger("AniState", (int)State.Idle);
                _ = owner.WaitForAsync(waitTime, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Dodo>(); });
            });
        }


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
