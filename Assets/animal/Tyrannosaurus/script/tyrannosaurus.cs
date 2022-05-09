using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public partial class Tyrannosaurus : MonoBehaviour, IAttackDamage
{
    public GameObject avater = null;        //アバター
    public Animator Animator;               //アニメーションの制御用
    public int angularSpeed = 160;          //回転の最高速度（単位： 度／秒）
    public float walkSpeed = 5.0F;          //移動速度
    public float aniWalkSpeed;              //アニメーションで進む早さ
    public GameObject target;               //追跡対象
    public NavMeshAgent agent;              //ナビメッシュ
    public TyrannosaurusStateBase currentState;
    public GameObject rayStartPos;          //レイ発射位置
    public float searchRange;               //索敵範囲
    public float searchAngle;               //視野角
    //public hit bitingHit;

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
        currentState = new TyrannosaurusIdleState();
        currentState.OnEnter(this, null);
        agent = GetComponent<NavMeshAgent>();
        target = GameObject.FindWithTag("Player");
        agent.speed = walkSpeed;
        agent.acceleration = walkSpeed;
        agent.angularSpeed = angularSpeed;
        //bitingHit = GetComponent<hit>();
        //bitingHit.info.damage = 10;
        //bitingHit.info.name = "ティラノサウルス";
        //bitingHit.SetActive(false);
    }

    private void Update()
    {
        currentState.OnUpdate(this);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("enemy  "+hit.gameObject.tag);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("enemy  " + collision.gameObject.tag);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enemy  " + other.gameObject.tag);
    }

    public void OnDamaged(AttackInfo _attackInfo)
    {
        // Debug.Log("get damage");
    }

    public void ChangeState<T>()where T:TyrannosaurusStateBase,new()
    {
        var nextState = new T();
        currentState.OnExit(this, nextState);
        nextState.OnEnter(this, currentState);
        currentState = nextState;
    }

    //アニメーションイベント
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
