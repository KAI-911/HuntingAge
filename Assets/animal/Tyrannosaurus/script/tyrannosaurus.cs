using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public partial class tyrannosaurus : MonoBehaviour
{
    public GameObject avater = null;        //�A�o�^�[
    public Animator Animator;               //�A�j���[�V�����̐���p
    public int angularSpeed = 160;          //��]�̍ō����x�i�P�ʁF �x�^�b�j
    public float walkSpeed = 5.0F;          //�ړ����x
    public float aniWalkSpeed;              //�A�j���[�V�����Ői�ޑ���
    public GameObject target;               //�ǐՑΏ�
    public NavMeshAgent agent;              //�i�r���b�V��
    public tyrannosaurusStateBase currentState;
    public GameObject rayStartPos;          //���C���ˈʒu
    public float searchRange;               //���G�͈�
    public float searchAngle;               //����p
    public GameObject bitingColl;
    public enum AniState
    {
        Idle,
        Move,
        BitingAttack
    }

    [System.Obsolete]
    private void Start()
    {
        //������Ԃ̐ݒ�
        currentState = new tyrannosaurusIdleState();
        currentState.OnEnter(this, null);
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player");
        agent.speed = walkSpeed;
        agent.acceleration = walkSpeed;
        agent.angularSpeed = angularSpeed;

        bitingColl.SetActive(false);
    }

    private void Update()
    {
        currentState.OnUpdate(this);
    }

    public void ChangeState<T>()where T:tyrannosaurusStateBase,new()
    {
        var nextState = new T();
        currentState.OnExit(this, nextState);
        nextState.OnEnter(this, currentState);
        currentState = nextState;
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

}
