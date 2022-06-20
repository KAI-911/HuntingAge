using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerStateBase
{
    float speed;
    RunOnce _runOnce;
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        speed = owner.MoveDirection.magnitude;
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Fall);
        owner.Animator.SetTrigger("Change");
        _runOnce = new RunOnce();

    }
    public override void OnExit(Player owner, PlayerStateBase nextState)
    {

    }

    public override void OnUpdate(Player owner)
    {
        if (!owner.GroundChecker.IsGround())
        {
            owner.MoveDirection = Vector3.zero;
            owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().x * owner.GetCameraRight(owner.PlayerCamera);
            owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().y * owner.GetCameraForward(owner.PlayerCamera);
            owner.MoveDirection = owner.MoveDirection.normalized * speed;
        }
    }


    public override void OnFixedUpdate(Player owner)
    {
        if (!owner.GroundChecker.IsGround())
        {
            owner.Rigidbody.AddForce(owner.MoveDirection, ForceMode.Impulse);
        }


        owner.LookAt(360);
    }

    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "End")
        {
            owner.ChangeState<LocomotionState>();
        }
    }
    public override void OnCollisionStay(Player owner, Collision collision)
    {


    }
}
