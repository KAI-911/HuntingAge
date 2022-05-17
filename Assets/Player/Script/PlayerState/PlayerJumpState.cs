using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    public class PlayerJumpState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            var vec = owner.moveDirection;
            vec.y = 0;
            float moveSpeed = vec.magnitude;
            owner.Animator.SetInteger("AniState", (int)AniState.Jump);
            //�ړ��������J������ɒ���
            owner.moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            //�A�j���[�V�����̒���
            owner.Animator.SetFloat("AniSpeed", moveSpeed);
            owner.moveDirection = Quaternion.Euler(0, owner.playerCamera.transform.rotation.eulerAngles.y, 0) * owner.moveDirection;
            owner.moveDirection *= moveSpeed;
        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("�W�����v");
            owner.moveDirection += owner.gravity * Time.deltaTime;
            owner.controller.Move(owner.moveDirection * Time.deltaTime);
        }
        public override void OnExit(Player owner, PlayerStateBase prevState)
        {
            owner.jumpIntervalCount = 0;
            owner.Animator.SetTrigger("Change");
        }
        public override void OnAnimetionEnd(Player owner, AnimationEvent _animationEvent)
        {
            owner.ChangeState<PlayerLocomotionState>();
        }

        public override void OnAnimetionFunction(Player owner, AnimationEvent _animationEvent)
        {
            if (_animationEvent.intParameter == 1)
            {
                //��яオ��Ƃ��ɏ�����ɗ͂�������
                owner.moveDirection.y = owner.jumpSpeed;
            }
        }
    }
}
