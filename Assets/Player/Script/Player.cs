using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private static readonly PlayerIdleState playerIdleState = new PlayerIdleState();//�ҋ@
    private static readonly PlayerSneakState playerSneakState = new PlayerSneakState();//���Ⴊ�ݑҋ@
    private static readonly PlayerSneakingWalkState playerSneakingWalkState = new PlayerSneakingWalkState();//���Ⴊ�݈ړ�
    private static readonly PlayerWalkState playerWalkState = new PlayerWalkState();//����
    private static readonly PlayerDushState playerDushState = new PlayerDushState();//����
    private static readonly PlayerDodgeState playerDodgeState = new PlayerDodgeState();//���
    private static readonly PlayerJumpState playerJumpState = new PlayerJumpState();//�W�����v

    private  PlayerStateBase currentState = playerIdleState;

    void Start()
    {
        playerCamera = Camera.main;
        currentState.OnEnter(this, null);
    }

    void Update()
    {
        currentState.OnUpdate(this);
        jumpIntervalCount += Time.deltaTime;
    }

    private void AnimetionEvent(AnimationEvent _num)
    {
        int i = _num.intParameter;
        currentState.OnAnimetionFunction(this, i);
    }
    private void AnimetionEnd(AnimationEvent _num)
    {
        int i = _num.intParameter;
        currentState.OnAnimetionEnd(this, i);
    }
    private void AnimetionStart(AnimationEvent _num)
    {
        int i = _num.intParameter;
        currentState.OnAnimetionStart(this, i);
    }


    /// <summary>
    /// �X�e�[�g�̕ύX
    /// </summary>
    /// <param name="nextState"></param>
    private void ChangeState(PlayerStateBase nextState)
    {
        currentState.OnExit(this, nextState);
        nextState.OnEnter(this, currentState);
        currentState = nextState;
    }
    /// <summary>
    /// �ړ����x�ƃA�j���[�V�����ړ����x��n����
    /// WASD�ňړ����Ă����
    /// </summary>
    /// <param name="_moveSpeed"></param>
    /// <param name="_aniMoveSpeed"></param>
    private void PlayerMove(float _moveSpeed,float _aniMoveSpeed)
    {
        Vector3 inputVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (controller.isGrounded)
        {
            //�ړ��������J������ɒ���
            moveDirection = inputVec;
            Vector3 camRot = playerCamera.transform.rotation.eulerAngles;
            moveDirection = Quaternion.Euler(0, camRot.y, 0) * moveDirection;
            //�e��ړ��X�s�[�h�̕ύX
            moveDirection *= _moveSpeed;
            //�����肵�Ȃ��悤�Ɉړ��A�j���[�V�����̍Đ����x�̒���
            float aniSpeed = moveDirection.magnitude / _aniMoveSpeed;
            Animator.SetFloat("AniSpeed", aniSpeed);
        }
        //�i�s�����Ɍ���
        Vector3 rotateTarget = new Vector3(moveDirection.x, 0, moveDirection.z);
        if (rotateTarget.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
            avater.transform.rotation = Quaternion.Lerp(lookRotation, avater.transform.rotation, turnSmoothing);
        }
        //�d�͂������Ĉړ����s
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

}
