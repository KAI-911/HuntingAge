using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    public class PlayerDushState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetInteger("AniState", (int)AniState.Dush);
        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("DushState");
            owner.PlayerMove(owner.dushSpeed, owner.AniDushSpeed);

            //�_�b�V���{�^����������Ă��Ȃ������������Ԃ�
            if (!Input.GetButton("Dash"))
            {
                owner.ChangeState(playerWalkState);
                return;
            }

            //�ړ���������Ă��Ȃ�������ҋ@��Ԃ�
            if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
            {
                owner.ChangeState(playerIdleState);
                return;
            }

            //�W�����v�{�^���������ꂽ��
            if (Input.GetButtonDown("Jump"))
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