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

            //移動が押されていなかったら待機状態へ
            if (!Input.GetButton("Horizontal") && !Input.GetButton("Vertical"))
            {
                owner.ChangeState(playerIdleState);
                return;
            }

            //ダッシュボタンが押されていたら走り状態へ
            if (Input.GetButton("Dash"))
            {
                owner.ChangeState(playerDushState);
                return;
            }

            //ジャンプボタンが押されたら
            if (Input.GetButtonDown("Jump") && owner.jumpInterval < owner.jumpIntervalCount)
            {
                owner.ChangeState(playerJumpState);
                return;
            }

            //しゃがみボタンが押されたら回避状態へ
            if (Input.GetButtonDown("Dodge"))
            {
                owner.ChangeState(playerDodgeState);
                return;
            }

        }
    }
}