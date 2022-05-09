using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System;

public partial class Player
{
    public class PlayerKnockBackState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetTrigger("Change");
            owner.Animator.SetInteger("AniState", (int)AniState.knockback);
        }

        public override void OnUpdate(Player owner)
        {
            if (owner.moveDirection == Vector3.zero) return;
            //Damage(vector3)�̂ق��ňړ��������Ȃǂ̐ݒ�����Ă���
            //�G�̕���
            Vector3 rotateTarget = new Vector3(-owner.moveDirection.x, 0, -owner.moveDirection.z);
            Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
            owner.avater.transform.rotation = Quaternion.Lerp(lookRotation, owner.avater.transform.rotation, owner.turnSmoothing);
            //�d�͂�������
            owner.moveDirection.y -= owner.gravity * Time.deltaTime;
            //�ړ����s
            owner.controller.Move(owner.moveDirection * Time.deltaTime);
            //���X�ɃX�s�[�h�𗎂Ƃ�
            owner.moveDirection *= 0.99f;
        }

        public override void OnAnimetionEnd(Player owner, int _num)
        {
            owner.ChangeState(playerIdleState);
        }
    }
}