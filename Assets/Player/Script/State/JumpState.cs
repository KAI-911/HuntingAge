using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : PlayerStateBase
{
    float speed;
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        owner.Rigidbody.AddForce(new Vector3(0, owner.JumpPowor, 0), ForceMode.Impulse);
        speed = owner.MaxSpeed;
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Jump);
        owner.Animator.SetTrigger("Change");
    }

    public override void OnUpdate(Player owner)
    {
        if (owner.Rigidbody.velocity.y < 0)
        {
            owner.ChangeState<FallState>();
        }

        owner.MoveDirection = Vector3.zero;
        owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().x * owner.GetCameraRight(owner.PlayerCamera);
        owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().y * owner.GetCameraForward(owner.PlayerCamera);
        owner.MoveDirection = owner.MoveDirection.normalized * speed;

        owner.LookAt();
    }
    public override void OnFixedUpdate(Player owner)
    {
        owner.Rigidbody.AddForce(owner.MoveDirection, ForceMode.Impulse);

    }


}
