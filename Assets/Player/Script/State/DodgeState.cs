using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : PlayerStateBase
{
    float power;
    float resistancePower = 0.15f;
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        power = owner.DodgePower;
        owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().x * owner.GetCameraRight(owner.PlayerCamera);
        owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().y * owner.GetCameraForward(owner.PlayerCamera);
        owner.MoveDirection = owner.MoveDirection.normalized * power;
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Dodge);
        owner.Animator.SetTrigger("Change");
    }
    public override void OnExit(Player owner, PlayerStateBase nextState)
    {

    }
    public override void OnUpdate(Player owner)
    {
        //if (owner.MoveDirection.sqrMagnitude < 0.1f)
        //{
        //    //í èÌèÛë‘Ç…ïŒà⁄
        //    owner.ChangeState<LocomotionState>();
        //}

        //íÖínÇµÇƒÇ¢Ç»Ç©Ç¡ÇΩÇÁóéâ∫èÛë‘Ç…ïŒà⁄
        if (!owner.GroundChecker.IsGround())
        {
            owner.ChangeState<FallState>();
        }


    }
    public override void OnFixedUpdate(Player owner)
    {
        owner.LookAt();
        power -= resistancePower;
        if (power < 0) return;
        owner.MoveDirection = owner.MoveDirection.normalized * power;
        owner.Rigidbody.AddForce(owner.MoveDirection, ForceMode.Impulse);
    }
    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {
        owner.ChangeState<LocomotionState>();
    }
    public override void OnCollisionStay(Player owner, Collision collision)
    {

    }
}
