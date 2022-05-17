using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public partial class Tyrannosaurus : MonoBehaviour, IAttackDamage
{

    private void Awake()
    {
    }
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

    }


    private void Update()
    {
        currentState.OnUpdate(this);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("enemy  " + collision.gameObject.tag);
    //    foreach (var element in colliders)
    //    {
    //    }
    //    //プレイヤーと当たってる
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        AttackInfo tmp = new AttackInfo();
    //        tmp.damage = 10;
    //        tmp.name = "ティラノサウルス";
    //        tmp.transform = this.transform;
    //        ExecuteEvents.Execute<IAttackDamage>(
    //        target: collision.gameObject,
    //        eventData: null,
    //        functor: (reciever, eventData) => reciever.OnDamaged(tmp));
    //    }
    //}


    public void OnDamaged(AttackInfo _attackInfo)
    {
        
        // Debug.Log("get damage");
    }

    public void ChangeState<T>() where T : TyrannosaurusStateBase, new()
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
