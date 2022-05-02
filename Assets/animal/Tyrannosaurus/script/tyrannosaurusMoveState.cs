using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tyrannosaurusMoveState : tyrannosaurusStateBase
{
    public override void OnEnter(tyrannosaurus owner, tyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)tyrannosaurus.AniState.Move);
    }

    public override void OnUpdate(tyrannosaurus owner)
    {
        //�����肵�Ȃ��悤�Ɉړ��A�j���[�V�����̍Đ����x�̒���
        owner.Animator.SetFloat("AniSpeed", owner.agent.speed / owner.aniWalkSpeed);
        owner.agent.destination = owner.target.transform.position;
        if ((owner.target.transform.position - owner.gameObject.transform.position).magnitude < owner.agent.stoppingDistance)
        {
            Vector3 diff = owner.target.transform.position - owner.rayStartPos.transform.position;
            // ��ԏ�̃x�N�g��
            var from = owner.gameObject.transform.forward;
            var to = diff;
            // ���ʂ̖@���x�N�g���i������x�N�g���Ƃ���j
            var planeNormal = Vector3.up;
            // ���ʂɓ��e���ꂽ�x�N�g�������߂�
            var planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
            var planeTo = Vector3.ProjectOnPlane(to, planeNormal);
            // ���ʂɓ��e���ꂽ�x�N�g�����m�̕����t���p�x
            // ���v���Ő��A�����v���ŕ�
            var signedAngle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);

            owner.ChangeState<tyrannosaurusBitingAttackState>();
        }
    }
}
