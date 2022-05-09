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
            owner.Animator.SetTrigger("Change");
            owner.Animator.SetInteger("AniState", (int)AniState.knockback);
        }

        public override void OnUpdate(Player owner)
        {
            if (owner.moveDirection == Vector3.zero) return;
            //Damage(vector3)のほうで移動っ方向などの設定をしている
            //敵の方向
            Vector3 rotateTarget = new Vector3(-owner.moveDirection.x, 0, -owner.moveDirection.z);
            Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
            owner.avater.transform.rotation = Quaternion.Lerp(lookRotation, owner.avater.transform.rotation, owner.turnSmoothing);
            //重力を加える
            owner.moveDirection.y -= owner.gravity * Time.deltaTime;
            //移動実行
            owner.controller.Move(owner.moveDirection * Time.deltaTime);
            //徐々にスピードを落とす
            owner.moveDirection *= 0.99f;
        }

        public override void OnAnimetionEnd(Player owner, int _num)
        {
            owner.ChangeState(playerIdleState);
        }
    }
}