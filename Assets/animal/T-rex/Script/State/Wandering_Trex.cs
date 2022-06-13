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
        while (true)//�ʂ̏ꏊ�ֈړ�
        {
            int num = Random.Range(0, owner.WaningPos.Length - 1);
            target = owner.WaningPos[num].transform.position;
            var vec = target - owner.transform.position;
            vec.y = 0;
            //�ړ��悪������x����Ă���ꍇ���[�v�𔲂���
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
        Debug.Log("����" + vec.magnitude + "�@�@ ��~����" + owner.NavMeshAgent.stoppingDistance * 1.2f);
        if (vec.magnitude < owner.NavMeshAgent.stoppingDistance * 1.2f)
        {
            Debug.Log("�߂��܂ŗ���");
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
