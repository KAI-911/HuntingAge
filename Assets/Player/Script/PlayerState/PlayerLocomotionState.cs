using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Player
{
    public class PlayerLocomotionState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetInteger("AniState", (int)AniState.Locomotion);
        }
        public override void OnUpdate(Player owner)
        {
            Debug.Log("�ړ�");
            //�A�j���[�V�����̈ړ����x�̒���
            float aniSpeed;
            float moveSpeed = owner.nowSpeed;
            if (moveSpeed > owner.AniWalkSpeed)
            {
                //�_�b�V���ƕ����̃u�����h��
                float walkRatio = (moveSpeed - owner.AniWalkSpeed) / (owner.AniWalkSpeed - owner.AniDushSpeed);
                aniSpeed = (owner.AniWalkSpeed * walkRatio) + (owner.AniDushSpeed * (1 - walkRatio));
            }
            else
            {
                //�����ƕ����̃u�����h��
                float walkRatio = (owner.AniWalkSpeed - moveSpeed) / owner.AniWalkSpeed;
                aniSpeed = owner.AniWalkSpeed * walkRatio;
            }
            owner.Animator.SetFloat("AniSpeed", aniSpeed, 0.1f, Time.deltaTime);

            owner.PlayerMove();

            //�ړ����x�̐ݒ�@�X�j�[�N���Ƀ_�b�V���{�^������������X�j�[�N�������đ���
            owner.nowMaxSpeed = owner.walkMaxSpeed;
            if (owner.sneakFlg) owner.nowMaxSpeed = owner.sneakMaxSpeed;
            if (Input.GetButton("Dash")) owner.nowMaxSpeed = owner.dushMaxSpeed;

            //�W�����v
            if(!owner.sneakFlg&&Input.GetButtonDown("Jump"))
            {
                owner.ChangeState<PlayerJumpState>();
            }


            //�_�b�V������Ƃ��Ⴊ�ݏ�Ԃ̉���
            if (owner.nowMaxSpeed > owner.sneakMaxSpeed)
            {
                owner.sneakFlg = false;
            }
            if (Input.GetButtonDown("Dodge"))
            {
                if (owner.nowSpeed < 0.1f)
                {
                    if (owner.nowSpeed < owner.walkMaxSpeed / 2)//�ړ����x���x����΂��Ⴊ��
                    {
                        owner.sneakFlg = !owner.sneakFlg;
                    }
                }
                else
                {
                    //���
                    owner.sneakFlg = false;
                    owner.ChangeState<PlayerDodgeState>();
                    return;
                }
            }
        }

        public override void OnExit(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetTrigger("Change");

        }
    }
}