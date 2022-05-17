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
            owner.Animator.SetInteger("AniState", (int)AniState.Knockback);
            //Damage(vector3)のほうで移動方向の設定やダメージ計算をしている
        }

        public override void OnUpdate(Player owner)
        {
            //移動実行
            owner.controller.Move(owner.moveDirection * Time.deltaTime);
        }
        public override void OnExit(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetTrigger("Change");
        }
        public override void OnAnimetionEnd(Player owner, AnimationEvent _animationEvent)
        {
            owner.ChangeState<PlayerLocomotionState>();
        }
        public override void OnAnimetionFunction(Player owner, AnimationEvent _animationEvent)
        {
            
        }
    }
}