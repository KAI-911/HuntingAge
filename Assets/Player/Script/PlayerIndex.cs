using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public partial class Player
{
    [SerializeField] private Camera playerCamera = null;            //�v���C���[������J����
    [SerializeField] private CharacterController controller;        //�v���C���[�̐���
    [SerializeField] private Transform AimPos;                      //�����_
    [SerializeField] private float turnSmoothing = 0.98F;           //�U������̑���
    public Vector3 gravity = new Vector3(0, -20.0f, 0);                 //�d�͂̑傫��
    //�W�����v�֌W�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private float jumpSpeed = 8.0F;                //�W�����v��
    [SerializeField] private float jumpInterval = 1.0f;             //�A���ŃW�����v�ł���Ԋu
    [SerializeField] private float jumpIntervalCount = 0.0f;        //�W�����v�Ԋu�̃J�E���g

    //�ړ����x�֌W�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private float sneakMaxSpeed = 1.0F;            //���Ⴊ�ݕ������x
    [SerializeField] private float walkMaxSpeed = 3.0F;             //�������x
    [SerializeField] private float dushMaxSpeed = 6.0F;             //���葬�x
    public float nowMaxSpeed, nowSpeed;
    [SerializeField] private float dodgeSpeed = 8.0F;               //�������Ƃ��̈ړ����x
    [SerializeField] private bool sneakFlg = false;                 //���Ⴊ��ł���==1�A����ȊO==0
    public Vector3 moveDirection = Vector3.zero;                    //�ړ������̕ۑ�

    //animation�֌W�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    public Animator Animator;                      //�A�j���[�V�����̐���p
    //�A�j���[�V�����Ői�ޑ���
    [SerializeField] private float AniSneakSpeed, AniWalkSpeed, AniDushSpeed;


    //�̗́[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private int hitPoint = 100;
    [SerializeField] private Slider HPslider;

    public enum AniState
    {
        Locomotion,
        Dodge,
        Jump,
        Knockback
    };

    private PlayerStateBase currentState;






}
