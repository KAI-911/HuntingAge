using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : PlayerStateBase
{
    bool _lockFlg;
    float _fallStartPosY;
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Fall);
        owner.Animator.SetTrigger("Change");
        _lockFlg = false;
        _fallStartPosY = owner.transform.position.y;
    }

    public override void OnFixedUpdate(Player owner)
    {
        if (!_lockFlg)
        {
            owner.MoveDirection = Vector3.zero;
            owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().x * owner.GetCameraRight(owner.PlayerCamera);
            owner.MoveDirection += owner.InputMoveAction.ReadValue<Vector2>().y * owner.GetCameraForward(owner.PlayerCamera);
            owner.MoveDirection = owner.MoveDirection.normalized * owner.MaxSpeed;
            owner.Rigidbody.AddForce(owner.MoveDirection, ForceMode.Impulse);

            if (owner.GroundChecker.IsGround())
            {
                owner.ChangeState<LocomotionState>();
                return;
                //_lockFlg = true;
                //float _fallLength = _fallStartPosY - owner.transform.position.y;
                //Debug.Log(_fallLength);
                //if (_fallLength > owner.FallLength)
                //{
                //    Debug.Log("着地");
                //    owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Landing);
                //}
                //else
                //{
                //    Debug.Log("すぐに移動できる");
                //    owner.ChangeState<LocomotionState>();
                //}
            }
            owner.LookAt();
        }
    }

    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {
        //if (animationEvent.stringParameter == "End")
        //{
        //    Debug.Log("ディレイが終わってlocomotionにいどう");
        //    owner.ChangeState<LocomotionState>();
        //}
    }

}
