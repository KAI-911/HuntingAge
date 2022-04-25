using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Player
{
    public class PlayerIdleState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetInteger("AniState", (int)AniState.Idle);
        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("IdleState");

            //���Ⴊ�݃{�^���������ꂽ�炵�Ⴊ�ݏ�Ԃ�
            if (Input.GetButtonDown("Dodge"))
            {
                owner.ChangeState(playerSneakState);
                return;
            }

            //�W�����v�{�^���������ꂽ��
            if(Input.GetButtonDown("Jump"))
            {
                owner.ChangeState(playerJumpState);
                return;
            }

            //WASD�������ꂽ�������Ԃ�
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                owner.ChangeState(playerWalkState);
                return;
            }
        }
    }
}