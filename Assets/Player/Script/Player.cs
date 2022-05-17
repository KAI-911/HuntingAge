using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
public partial class Player : MonoBehaviour, IAttackDamage
{

    void Start()
    {
        playerCamera = Camera.main;
        currentState = new PlayerLocomotionState();
        currentState.OnEnter(this, null);
        nowMaxSpeed = walkMaxSpeed;
        HPslider.maxValue = hitPoint;
        HPslider.minValue = 0;
    }

    void Update()
    {
        HPslider.value = hitPoint;
        Animator.SetInteger("HP", hitPoint);
        Animator.SetFloat("move", nowSpeed, 0.1f, Time.deltaTime);
        Animator.SetBool("Sneak", sneakFlg);
        var speed = moveDirection;
        speed.y = 0;
        nowSpeed = speed.magnitude;
        currentState.OnUpdate(this);

        if (jumpIntervalCount < jumpInterval)
        {
            jumpIntervalCount += Time.deltaTime;
        }
    }

    private void AnimetionEvent(AnimationEvent _num)
    {
        currentState.OnAnimetionFunction(this, _num);
    }
    private void AnimetionEnd(AnimationEvent _num)
    {
        currentState.OnAnimetionEnd(this, _num);
    }
    private void AnimetionStart(AnimationEvent _num)
    {
        currentState.OnAnimetionStart(this, _num);
    }


    public void OnDamaged(AttackInfo _attackInfo)
    {
        if (currentState.GetType() == typeof(PlayerKnockBackState)) return;
        ChangeState<PlayerKnockBackState>();
        Debug.Log(_attackInfo.name + "����" + _attackInfo.damage + "�̃_���[�W���󂯂�   " + gameObject.tag + "���");
        hitPoint -= _attackInfo.damage;
        if (hitPoint < 0)
        {
            hitPoint = 0;
        }
        moveDirection = transform.position - _attackInfo.transform.position;
        moveDirection = moveDirection.normalized * 50.0f;

    }


    /// <summary>
    /// �X�e�[�g�̕ύX
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangeState<T>() where T : PlayerStateBase, new()
    {
        var nextState = new T();
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
    private void PlayerMove()
    {            
        ////�A�j���[�V�����̈ړ����x�̒���
        //float aniSpeed;
        //float moveSpeed = nowSpeed;
        //if (moveSpeed > AniWalkSpeed)
        //{
        //    //�_�b�V���ƕ����̃u�����h��
        //    float walkRatio = (moveSpeed - AniWalkSpeed) / (AniWalkSpeed - AniDushSpeed);
        //    aniSpeed = (AniWalkSpeed * walkRatio) + (AniDushSpeed * (1 - walkRatio));
        //}
        //else
        //{
        //    //�����ƕ����̃u�����h��
        //    float walkRatio = (AniWalkSpeed - moveSpeed) / AniWalkSpeed;
        //    aniSpeed = AniWalkSpeed * walkRatio;
        //}

        Vector3 inputVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (controller.isGrounded)
        {
            //�ړ��������J������ɒ���
            moveDirection = inputVec;
            Vector3 camRot = playerCamera.transform.rotation.eulerAngles;
            moveDirection = Quaternion.Euler(0, camRot.y, 0) * moveDirection;
            //�e��ړ��X�s�[�h�̕ύX
            if(moveDirection.magnitude>1)
            {
                moveDirection = moveDirection.normalized;
            }
            moveDirection *= nowMaxSpeed;
        }

        //�i�s�����Ɍ���
        Vector3 rotateTarget = new Vector3(moveDirection.x, 0, moveDirection.z);
        if (rotateTarget.magnitude > 0.1f)Look(rotateTarget, turnSmoothing);
        //�d��
        moveDirection.y = gravity.y;

        //�ړ����s
        controller.Move(moveDirection * Time.deltaTime);
    }

    public void Look(Vector3 _direction,float _turnSmooth)
    {
        if (_direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Lerp(lookRotation, transform.rotation, _turnSmooth);
        }

    }

    //private async Task Damage(AttackInfo _attackInfo)
    //{
    //    ChangeState<PlayerKnockBackState>();
    //    Debug.Log(_attackInfo.name + "����" + _attackInfo.damage + "�̃_���[�W���󂯂�   " + gameObject.tag + "���");
    //    hitPoint -= _attackInfo.damage;
    //    if (hitPoint < 0)
    //    {
    //        hitPoint = 0;
    //    }
    //    moveDirection = transform.position - _attackInfo.transform.position;
    //    moveDirection = moveDirection.normalized * 10f;
    //    await Task.Delay(TimeSpan.FromSeconds(0.5f));
    //    moveDirection = Vector3.zero;
    //}
}
