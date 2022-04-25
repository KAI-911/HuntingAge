using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    public class PlayerDodgeState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetInteger("AniState", (int)AniState.Dodge);

            //�ړ��������J������ɒ���
            owner.moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); ;
            owner.moveDirection = Quaternion.Euler(0, owner.playerCamera.transform.rotation.eulerAngles.y, 0) * owner.moveDirection;
            owner.moveDirection *= owner.dodgeSpeed;
            //�d�͂�������
            owner.moveDirection.y -= owner.gravity * Time.deltaTime;

        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("DodgeState");

            //�i�s�����Ɍ���
            Vector3 rotateTarget = new Vector3(owner.moveDirection.x, 0, owner.moveDirection.z);
            if (rotateTarget.magnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
                owner.avater.transform.rotation = Quaternion.Lerp(lookRotation, owner.avater.transform.rotation, 0.1f);
            }
            owner.controller.Move(owner.moveDirection * Time.deltaTime);
        }

        public override void OnAnimetionEnd(Player owner, int _num)
        {
            //WASD��������Ă����������Ԃ�
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                //�_�b�V���{�^����������Ă����瑖���Ԃ�
                if (Input.GetButton("Dash"))
                {
                    owner.ChangeState(playerDushState);
                    return;
                }
                owner.ChangeState(playerWalkState);
                return;
            }
            //�ړ���������Ă��Ȃ�������ҋ@��Ԃ�
            owner.ChangeState(playerIdleState);
            return;
        }

        public override void OnAnimetionFunction(Player owner, int _num)
        {
            //�W�����v���Ď�������猸��
            owner.moveDirection.x *= 0.7f;
            owner.moveDirection.z *= 0.7f;
        }
    }
}
