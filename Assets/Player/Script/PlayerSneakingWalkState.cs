using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Player
{
    public class PlayerSneakingWalkState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.sneakFlg = true;
            owner.Animator.SetInteger("AniState", (int)AniState.SneakingWalk);
        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("SneakingWalkState");
            owner.PlayerMove(owner.sneakSpeed, owner.AniSneakSpeed);

            //���Ⴊ�݃{�^���������ꂽ������Ԃ�
            if (Input.GetButtonDown("Dodge"))
            {
                owner.ChangeState(playerDodgeState);
                return;
            }
            //���Ⴊ�ݒ��ɃW�����v�{�^���������ꂽ�������Ԃ�
            if (Input.GetButtonDown("Jump"))
            {
                owner.ChangeState(playerWalkState);
                return;
            }
            //�ړ���������Ă��Ȃ�������ҋ@��Ԃ�
            if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
            {
                owner.ChangeState(playerSneakState);
                return;
            }
        }
        public override void OnExit(Player owner, PlayerStateBase prevState)
        {
            owner.sneakFlg = false;
        }
    }
}