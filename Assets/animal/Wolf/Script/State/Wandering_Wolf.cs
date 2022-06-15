using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering_Wolf : SatetBase_Wolf
{
    private Vector3 target;
    private RunOnce once = new RunOnce();
    int waitTime;
    public override void OnEnter(Wolf owner, SatetBase_Wolf prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);

        owner.NavMeshAgent.speed = 2.5f;

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
    public override void OnExit(Wolf owner, SatetBase_Wolf nextState)
    {

    }
    public override void OnUpdate(Wolf owner)
    {
        Debug.Log("wandering");
        owner.NavMeshAgent.destination = target;

        if (owner.Search())
        {
            owner.ChangeState<Move_Wolf>();
            return;
        }
        var vec = target - owner.transform.position;
        vec.y = 0;
        if (vec.magnitude < owner.NavMeshAgent.stoppingDistance * 1.2f)
        {
            //�ڕW�n�_�ɗ�����A��~����B
            //waitTime�b��A�������Ă��Ȃ������������x�p�j����B
            once.Run(() =>
            {
                Debug.Log("onceRun");
                owner.Animator.SetInteger("AniState", (int)State.Idle);
                _ = owner.WaitForAsync(waitTime, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Wolf>(); });
            });
        }
    }
    public override void OnFixedUpdate(Wolf owner)
    {

    }
    public override void OnAnimationEvent(Wolf owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Wolf owner, Collision collision)
    {

    }
}
