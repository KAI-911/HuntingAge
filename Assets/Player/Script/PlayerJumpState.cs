using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    public class PlayerJumpState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.lockAxisCamera.y_islocked = true;
            owner.lockAxisCamera.lockPosition.y = owner.playerCamera.transform.position.y;
            float moveSpeed = owner.moveDirection.magnitude;
            owner.Animator.SetInteger("AniState", (int)AniState.Jump);
            //�ړ��������J������ɒ���
            owner.moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //�A�j���[�V�����̒���
            owner.Animator.SetFloat("AniSpeed", Mathf.Clamp01(owner.moveDirection.magnitude));
            owner.moveDirection = Quaternion.Euler(0, owner.playerCamera.transform.rotation.eulerAngles.y, 0) * owner.moveDirection;
            owner.moveDirection *= moveSpeed;
            owner.moveDirection.y = owner.jumpSpeed;
        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("JumpState");
            //�d�͂�������
            owner.moveDirection.y -= owner.gravity * Time.deltaTime;
            owner.controller.Move(owner.moveDirection * Time.deltaTime);
        }
        public override void OnExit(Player owner, PlayerStateBase prevState)
        {
            owner.lockAxisCamera.y_islocked = false;
            owner.jumpIntervalCount = 0;
        }
        public override void OnAnimetionEnd(Player owner, int _num)
        {
            //WASD�������ꂽ�������Ԃ�
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
            else
            {
                //�ړ���������Ă��Ȃ�������ҋ@��Ԃ�
                owner.ChangeState(playerIdleState);
                return;
            }
        }

        public override void OnAnimetionFunction(Player owner, int _num)
        {
            //�W�����v���Ď�������猸��
            owner.moveDirection.x *= 0.8f;
            owner.moveDirection.z *= 0.8f;
        }
    }
}
