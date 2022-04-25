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

            //しゃがみボタンが押されたらしゃがみ状態へ
            if (Input.GetButtonDown("Dodge"))
            {
                owner.ChangeState(playerSneakState);
                return;
            }

            //ジャンプボタンが押されたら
            if(Input.GetButtonDown("Jump"))
            {
                owner.ChangeState(playerJumpState);
                return;
            }

            //WASDが押されたら歩き状態へ
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                owner.ChangeState(playerWalkState);
                return;
            }
        }
    }
}