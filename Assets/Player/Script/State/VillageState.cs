using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillageState : PlayerStateBase
{
    float nowSpeed;
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        nowSpeed = owner.MaxSpeed;
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Locomotion);
        owner.Animator.SetTrigger("Change");
    }
    public override void OnExit(Player owner, PlayerStateBase nextState)
    {

    }
    public override void OnUpdate(Player owner)
    {
        owner.MoveDirection = Vector3.zero;
        if (owner.IsAction)
        {
            owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().x * owner.GetCameraRight(owner.PlayerCamera);
            owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().y * owner.GetCameraForward(owner.PlayerCamera);
            owner.MoveDirection = owner.MoveDirection * nowSpeed;
            //デッドゾーンを作る
            if (owner.MoveDirection.sqrMagnitude < (0.5f * 0.5f)) owner.MoveDirection = Vector3.zero;
        }
        owner.Animator.SetFloat("speed", owner.MoveDirection.magnitude, 0.1f, Time.deltaTime);
    }
    public override void OnFixedUpdate(Player owner)
    {
        owner.Rigidbody.AddForce(owner.MoveDirection, ForceMode.Impulse);
        owner.LookAt();
    }


    public override void OnDodge(Player owner)
    {

    }
    public override void OnJump(Player owner)
    {

    }
    public override void OnStrongAttack(Player owner)
    {

    }
    public override void OnWeakAttack(Player owner)
    {

    }
}
