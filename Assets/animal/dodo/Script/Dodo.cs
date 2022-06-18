using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodo : Enemy
{
    [SerializeField] private StateBase_Dodo _currentState;
    public StateBase_Dodo CurrentState { get => _currentState; }

    void Start()
    {
        _currentState = new Idle_Dodo();
        _currentState.OnEnter(this, null);
    }
    void Update()
    {
        Animator.SetInteger("HP", Status.HP);
        _currentState.OnUpdate(this);

        //if (Status.DownFlg && CurrentState.GetType() != typeof(Down_Trex))
        //{
        //    ChangeState<Down_Trex>();
        //}
        //if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Trex))
        //{
        //    ChangeState<Death_Trex>();
        //}
    }
    void FixedUpdate()
    {
        _currentState.OnFixedUpdate(this);
    }
    void OnAnimationEvent(AnimationEvent animationEvent)
    {
        _currentState.OnAnimationEvent(this, animationEvent);

    }
    public void ChangeState<T>() where T : StateBase_Dodo, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }

}
