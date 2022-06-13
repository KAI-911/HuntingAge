using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;

public class Wolf : Enemy
{
    [SerializeField] private SatetBase_Wolf _currentState;
    public SatetBase_Wolf CurrentState { get => _currentState; }

    void Start()
    {
        _currentState = new Idle_Wolf();

    }
    void Update()
    {
        Animator.SetInteger("HP", Status.HP);
        _currentState.OnUpdate(this);

        if (Status.DownFlg && CurrentState.GetType() != typeof(Down_Wolf) && CurrentState.GetType() != typeof(Death_Wolf))
        {
            ChangeState<Down_Wolf>();
        }
        if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Wolf))
        {
            ChangeState<Death_Wolf>();
        }
    }
    void FixedUpdate()
    {
        _currentState.OnFixedUpdate(this);
    }
    void OnAnimationEvent(AnimationEvent animationEvent)
    {
        _currentState.OnAnimationEvent(this, animationEvent);

    }
    public void ChangeState<T>() where T : SatetBase_Wolf, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }

}
