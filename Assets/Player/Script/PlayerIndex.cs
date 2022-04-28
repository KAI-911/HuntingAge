using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private Camera playerCamera = null;         //�v���C���[������J����
    public GameObject avater = null;            //�A�o�^�[
    public Transform AimPos;                    //�����_
    public Animator Animator;                   //�A�j���[�V�����̐���p
    public CharacterController controller;      //�v���C���[�̐���
    public float jumpSpeed = 8.0F;              //�W�����v��
    public float dodgeSpeed = 8.0F;             //�������Ƃ��̈ړ����x
    public float gravity = 20.0F;               //�d�͂̑傫��
    public float turnSmoothing = 0.98F;         //�U������̑���
    public float jumpInterval = 1.0f;           //�A���ŃW�����v�ł���Ԋu
    public float sneakSpeed = 1.0F;             //���Ⴊ�ݕ������x
    public float walkSpeed = 3.0F;              //�������x
    public float dushSpeed = 6.0F;              //���葬�x
    //�A�j���[�V�����Ői�ޑ���
    public float AniSneakSpeed;
    public float AniWalkSpeed;
    public float AniDushSpeed;

    private float jumpIntervalCount = 0.0f;
    private Vector3 moveDirection = Vector3.zero;//�ړ������̕ۑ�
    private bool sneakFlg = false;//���Ⴊ��ł���==1�A����ȊO==0
    public enum AniState
    {
        Idle,
        Sneak,
        SneakingWalk,
        Walk,
        Dush,
        Dodge,
        Jump
    };
    private static readonly PlayerIdleState playerIdleState = new PlayerIdleState();//�ҋ@
    private static readonly PlayerSneakState playerSneakState = new PlayerSneakState();//���Ⴊ�ݑҋ@
    private static readonly PlayerSneakingWalkState playerSneakingWalkState = new PlayerSneakingWalkState();//���Ⴊ�݈ړ�
    private static readonly PlayerWalkState playerWalkState = new PlayerWalkState();//����
    private static readonly PlayerDushState playerDushState = new PlayerDushState();//����
    private static readonly PlayerDodgeState playerDodgeState = new PlayerDodgeState();//���
    private static readonly PlayerJumpState playerJumpState = new PlayerJumpState();//�W�����v

    private PlayerStateBase currentState = playerIdleState;


}
