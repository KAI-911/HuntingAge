using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering_Rhino : StateBase_Rhino
{
    private Vector3 target;
    private RunOnce once = new RunOnce();
    int waitTime;
    public override void OnEnter(Rhino owner, StateBase_Rhino prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Wandering);
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
    public override void OnExit(Rhino owner, StateBase_Rhino nextState)
    {

    }
    public override void OnUpdate(Rhino owner)
    {
        owner.NavMeshAgent.destination = target;
        if (owner.ReceivedAttackCheck()) return;
        if (owner.Search())
        {
            owner.ChangeState<Move_Rhino>();
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
                owner.Animator.SetInteger("AniState", (int)State.Idle);
                _ = owner.WaitForAsync(waitTime, () => { if (!owner.DiscoverFlg) owner.ChangeState<Wandering_Rhino>(); });
            });
        }
    }
    public override void OnFixedUpdate(Rhino owner)
    {

    }
    public override void OnAnimationEvent(Rhino owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Rhino owner, Collision collision)
    {

    }
}
