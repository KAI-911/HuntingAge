using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeState : PlayerStateBase
{

    [SerializeField] float _invincibleTime = 0.1f;//�b
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Dodge);
        owner.Animator.SetTrigger("Change");


        //��莞�Ԍo�߂����疳�G�t���O�𗎂Ƃ�
        owner.Status.InvincibleFlg = true;
        _ = owner.WaitForAsync(_invincibleTime, () => owner.Status.InvincibleFlg = false);
    }
    public override void OnExit(Player owner, PlayerStateBase nextState)
    {

    }
    public override void OnUpdate(Player owner)
    {

        //���n���Ă��Ȃ������痎����ԂɕΈ�
        if (!owner.GroundChecker.IsGround())
        {
            owner.ChangeState<FallState>();
        }

        owner.LookAt(360);

    }
    public override void OnFixedUpdate(Player owner)
    {
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
