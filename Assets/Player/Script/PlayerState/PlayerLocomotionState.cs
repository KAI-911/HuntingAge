using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Player
{
    public class PlayerLocomotionState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetInteger("AniState", (int)AniState.Locomotion);
        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("移動");
            //アニメーションの移動速度の調整
            float aniSpeed;
            float moveSpeed = owner.nowSpeed;
            if (moveSpeed > owner.AniWalkSpeed)
            {
                //ダッシュと歩きのブレンド中
                float walkRatio = (moveSpeed - owner.AniWalkSpeed) / (owner.AniWalkSpeed - owner.AniDushSpeed);
                aniSpeed = (owner.AniWalkSpeed * walkRatio) + (owner.AniDushSpeed * (1 - walkRatio));
            }
            else
            {
                //立ちと歩きのブレンド中
                float walkRatio = (owner.AniWalkSpeed - moveSpeed) / owner.AniWalkSpeed;
                aniSpeed = owner.AniWalkSpeed * walkRatio;
            }
            owner.Animator.SetFloat("AniSpeed", aniSpeed, 0.1f, Time.deltaTime);

            owner.PlayerMove();

            //移動速度の設定　スニーク中にダッシュボタンを押したらスニーク解除して走る
            owner.nowMaxSpeed = owner.walkMaxSpeed;
            if (owner.sneakFlg) owner.nowMaxSpeed = owner.sneakMaxSpeed;
            if (Input.GetButton("Dash")) owner.nowMaxSpeed = owner.dushMaxSpeed;

            //ジャンプ
            if(!owner.sneakFlg&&Input.GetButtonDown("Jump"))
            {
                owner.ChangeState<PlayerJumpState>();
            }


            //ダッシュするとしゃがみ状態の解除
            if (owner.nowMaxSpeed > owner.sneakMaxSpeed)
            {
                owner.sneakFlg = false;
            }
            if (Input.GetButtonDown("Dodge"))
            {
                if (owner.nowSpeed < 0.1f)
                {
                    if (owner.nowSpeed < owner.walkMaxSpeed / 2)//移動速度が遅ければしゃがむ
                    {
                        owner.sneakFlg = !owner.sneakFlg;
                    }
                }
                else
                {
                    //回避
                    owner.sneakFlg = false;
                    owner.ChangeState<PlayerDodgeState>();
                    return;
                }
            }
        }

        public override void OnExit(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetTrigger("Change");

        }
    }
}