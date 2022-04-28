using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Player
{
    public class PlayerWalkState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetInteger("AniState", (int)AniState.Walk);
        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("WalkState");
            owner.PlayerMove(owner.walkSpeed, owner.AniWalkSpeed);

            //�ړ���������Ă��Ȃ�������ҋ@��Ԃ�
            if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
            {
                owner.ChangeState(playerIdleState);
                return;
            }

            //�_�b�V���{�^����������Ă����瑖���Ԃ�
            if (Input.GetButton("Dash"))
            {
                owner.ChangeState(playerDushState);
                return;
            }

            //�W�����v�{�^���������ꂽ��
            if (Input.GetButtonDown("Jump") && owner.jumpInterval < owner.jumpIntervalCount)
            {
                owner.ChangeState(playerJumpState);
                return;
            }

            //���Ⴊ�݃{�^���������ꂽ������Ԃ�
            if (Input.GetButtonDown("Dodge"))
            {
                owner.ChangeState(playerDodgeState);
                return;
            }

        }
    }
}