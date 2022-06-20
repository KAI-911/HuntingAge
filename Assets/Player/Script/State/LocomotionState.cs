using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : PlayerStateBase
{
    float nowSpeed;
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        nowSpeed = owner.DashSpeed;
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Locomotion);
        owner.Animator.SetTrigger("Change");

    }
    public override void OnExit(Player owner, PlayerStateBase nextState)
    {

    }
    public override void OnUpdate(Player owner)
    {

        owner.MoveDirection = Vector3.zero;
        owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().x * owner.GetCameraRight(owner.PlayerCamera);
        owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().y * owner.GetCameraForward(owner.PlayerCamera);
        owner.MoveDirection = owner.MoveDirection * nowSpeed;
        //�f�b�h�]�[�������
        if (owner.MoveDirection.sqrMagnitude < (0.5f * 0.5f)) owner.MoveDirection = Vector3.zero;
        owner.Animator.SetFloat("speed", owner.MoveDirection.magnitude, 0.1f, Time.deltaTime);

        //���n���Ă��Ȃ������痎����ԂɕΈ�
        if (!owner.GroundChecker.IsGround())
        {
            owner.ChangeState<FallState>();
        }

    }
    public override void OnFixedUpdate(Player owner)
    {
        owner.Rigidbody.AddForce(owner.MoveDirection, ForceMode.Impulse);
        owner.LookAt(360);
    }
    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Player owner, Collision collision)
    {

    }
}
