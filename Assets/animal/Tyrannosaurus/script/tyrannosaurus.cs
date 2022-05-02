using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public partial class tyrannosaurus : MonoBehaviour
{
    public GameObject avater = null;        //アバター
    public Animator Animator;               //アニメーションの制御用
    public int angularSpeed = 160;          //回転の最高速度（単位： 度／秒）
    public float walkSpeed = 5.0F;          //移動速度
    public float aniWalkSpeed;              //アニメーションで進む早さ
    public GameObject target;               //追跡対象
    public NavMeshAgent agent;              //ナビメッシュ
    public tyrannosaurusStateBase currentState;
    public GameObject rayStartPos;          //レイ発射位置
    public float searchRange;               //索敵範囲
    public float searchAngle;               //視野角
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
        //初期状態の設定
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
