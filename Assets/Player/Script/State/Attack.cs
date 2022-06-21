using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : PlayerStateBase
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

    public override void OnFixedUpdate(Player owner)
    {
        if (owner.Animator.GetBool("InputReception"))
        {
            owner.LookAt(100);
        }
    }
    public override void OnAnimationEvent(Player owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "Change")//�U���̓����蔻��̐؂�ւ�
        {
            owner.HitReceiver.ChangeAttackFlg((PartType)animationEvent.intParameter);
        }
        else if (animationEvent.stringParameter == "InputReceptionStart")//�A���U������ꍇ�̓��͎�t���J�n
        {
            owner.Animator.SetBool("InputReception", true);
        }
        else if (animationEvent.stringParameter == "InputReceptionEnd")//�A���U������ꍇ�̓��͎�t���I��
        {
            owner.Animator.SetBool("InputReception", false);
        }
        else if (animationEvent.stringParameter == "End")//�A�j���[�V�����I��
        {
            owner.HitReceiver.AttackFlgReset();
            owner.ChangeState<LocomotionState>();
        }
    }

    public override void OnDodge(Player owner)
    {
        if (!owner.GroundChecker.IsGround()) return;
        if (owner.InputMoveAction.ReadValue<Vector2>().sqrMagnitude <= 0.1f) return;
        if (!owner.Animator.GetBool("InputReception")) return;
        owner.ChangeState<DodgeState>();

    }
    public override void OnStrongAttack(Player owner)
    {
        //�A���U���̓��͎�t
        if (owner.Animator.GetBool("InputReception"))
        {
            owner.Animator.SetTrigger("AttackChain");
            owner.Animator.SetBool("InputReception", false);
            owner.Animator.SetInteger("AttackType", (int)AttackType.StrongAttack);
        }
    }
    public override void OnWeakAttack(Player owner)
    {
        //�A���U���̓��͎�t
        if (owner.Animator.GetBool("InputReception"))
        {
            owner.Animator.SetTrigger("AttackChain");
            owner.Animator.SetBool("InputReception", false);
            owner.Animator.SetInteger("AttackType", (int)AttackType.WeakAttack);

        }
    }
}
