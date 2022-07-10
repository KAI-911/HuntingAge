using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocomotionState : PlayerStateBase
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

        //着地していなかったら落下状態に偏移
        if (!owner.GroundChecker.IsGround())
        {
            owner.ChangeState<FallState>();
        }

    }
    public override void OnFixedUpdate(Player owner)
    {
        owner.Rigidbody.AddForce(owner.MoveDirection, ForceMode.Impulse);
        owner.LookAt();
    }


    public override void OnDodge(Player owner)
    {
        if (!owner.GroundChecker.IsGround()) return;
        if (owner.InputMoveAction.ReadValue<Vector2>().sqrMagnitude <= 0.1f)
        {
            //方向入力がなければ採取
            if (owner.CollectionFlg)
            {
                owner.ChangeState<Collection>();
            }
            return;
        }
        owner.ChangeState<DodgeState>();

    }
    public override void OnJump(Player owner)
    {
        if (!owner.GroundChecker.IsGround()) return;
        owner.ChangeState<JumpState>();

    }
    public override void OnStrongAttack(Player owner)
    {
        owner.Animator.SetInteger("AttackType", (int)AttackType.StrongAttack);
        owner.ChangeState<Attack>();
    }
    public override void OnWeakAttack(Player owner)
    {
        owner.Animator.SetInteger("AttackType", (int)AttackType.WeakAttack);
        owner.ChangeState<Attack>();
    }
}
