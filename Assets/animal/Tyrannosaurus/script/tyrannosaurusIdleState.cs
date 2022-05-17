using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusIdleState : TyrannosaurusStateBase
{
    Hit seatchHit = null;
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)Tyrannosaurus.AniState.Idle);
        foreach (var element in owner.receiver.hits)
        {
            if (element.GetPart() != HitReceiver.Part.searchTrigger) continue;
            seatchHit = element;
        }

    }

    public override void OnUpdate(Tyrannosaurus owner)
    {
        if (seatchHit == null || seatchHit.GetTrigger() == false) return;
        // ��ԏ�̃x�N�g��
        var from = owner.gameObject.transform.forward;
        var to = owner.target.transform.position - owner.rayStartPos.transform.position;
        // ���ʂ̖@���x�N�g���i������x�N�g���Ƃ���j
        var planeNormal = Vector3.up;
        // ���ʂɓ��e���ꂽ�x�N�g�������߂�
        var planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
        var planeTo = Vector3.ProjectOnPlane(to, planeNormal);
        // ���ʂɓ��e���ꂽ�x�N�g�����m�̕����t���p�x
        // ���v���Ő��A�����v���ŕ�
        var signedAngle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);
        if (owner.searchAngle / 2.0f < Mathf.Abs(signedAngle)) return;

        //�Ԃɏ�Q�������邩�m�F
        Vector3 dir = to.normalized;
        RaycastHit[] raycastHits = new RaycastHit[10];
        Ray ray = new Ray
        {
            origin = owner.rayStartPos.transform.position,
            direction = dir
        };
        int hitCount = Physics.RaycastNonAlloc(ray, raycastHits, to.magnitude, 0);
        //tag��Untagged�̂��̂ƃ��C���������
        if (hitCount > 0) return;

        owner.ChangeState<TyrannosaurusMoveState>();
    }
}
