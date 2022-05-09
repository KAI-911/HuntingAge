using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusIdleState : TyrannosaurusStateBase
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)Tyrannosaurus.AniState.Idle);
    }

    public override void OnUpdate(Tyrannosaurus owner)
    {
        Vector3 diff = owner.target.transform.position - owner.rayStartPos.transform.position;
        float dis = diff.magnitude;
        if (owner.searchRange > dis)
        {
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
            if (owner.searchAngle / 2.0f > Mathf.Abs(signedAngle))
            {
                //�Ԃɏ�Q�������邩�m�F
                Vector3 dir = diff.normalized;
                RaycastHit[] raycastHits = new RaycastHit[50];
                Ray ray = new Ray
                {
                    origin = owner.rayStartPos.transform.position,
                    direction = dir
                };
                int hitCount = Physics.RaycastNonAlloc(ray, raycastHits, dis, 3);
                //�v���C���[�̓L�����N�^�[�R���g���[���[���g�p�����Ă��邽�߁A
                //RaycastNonAlloc�̓R���W������������̂������肷��̂Ńv���C���[�͔��肳��Ȃ�
                if (hitCount == 0)
                {
                    owner.ChangeState<TyrannosaurusMoveState>();
                }
            }
        }
    }
}
