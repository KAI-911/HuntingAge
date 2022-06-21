using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrongAttack : PlayerStateBase
{
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.StrongAttack);
        owner.Animator.SetTrigger("Change");
        owner.Animator.SetBool("AttackChain", false);
        owner.Animator.SetBool("InputReception", false);
    }
    public override void OnExit(Player owner, PlayerStateBase nextState)
    {
        owner.HitReceiver.AttackFlgReset();
    }
    public override void OnUpdate(Player owner)
    {

    }
    public override void OnFixedUpdate(Player owner)
    {
        if(owner.Animator.GetBool("InputReception"))
        {
            owner.LookAt(100);
        }
    }
    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Change")//攻撃の当たり判定の切り替え
        {
            owner.HitReceiver.ChangeAttackFlg(owner.WeponChange.GetPartType());
        }
        else if(animationEvent.stringParameter == "InputReceptionStart")//連続攻撃する場合の入力受付を開始
        {
            owner.Animator.SetBool("InputReception", true);
        }
        else if (animationEvent.stringParameter == "InputReceptionEnd")//連続攻撃する場合の入力受付を終了
        {
            owner.Animator.SetBool("InputReception", false);
        }
        else if (animationEvent.stringParameter == "End")//アニメーション終了
        {
            Debug.Log("攻撃終了");
            owner.HitReceiver.AttackFlgReset(); 
            owner.ChangeState<LocomotionState>();
        }
    }
    public override void OnCollisionStay(Player owner, Collision collision)
    {

    }

}
