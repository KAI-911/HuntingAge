using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    public class PlayerDodgeState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetInteger("AniState", (int)AniState.Dodge);

            //移動方向をカメラ基準に直す
            owner.nowMaxSpeed = owner.dodgeSpeed;
            owner.moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            owner.moveDirection = Quaternion.Euler(0, owner.playerCamera.transform.rotation.eulerAngles.y, 0) * owner.moveDirection;
        }

        public override void OnUpdate(Player owner)
        {
            Debug.Log("回避");
            //進行方向に向く
            Vector3 rotateTarget = new Vector3(owner.moveDirection.x, 0, owner.moveDirection.z);
            if (rotateTarget.magnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
                owner.transform.rotation = Quaternion.Lerp(lookRotation, owner.transform.rotation, 0.1f);
            }
            owner.controller.Move(owner.moveDirection * Time.deltaTime);
        }
        public override void OnExit(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetTrigger("Change");
        }

        public override void OnAnimetionEnd(Player owner, AnimationEvent _animationEvent)
        {
            owner.ChangeState<PlayerLocomotionState>();
            return;
        }
    }
}
