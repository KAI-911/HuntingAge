using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deer : Enemy
{
    [SerializeField] private StateBase_Deer _currentState;
    public StateBase_Deer CurrentState { get => _currentState; }

    void Start()
    {
        _currentState = new Idle_Deer();
        _currentState.OnEnter(this, null);
    }
    void Update()
    {
        Animator.SetInteger("HP", Status.HP);
        _currentState.OnUpdate(this);

        if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Deer))
        {
            ChangeState<Death_Deer>();
        }
    }
    public override bool ReceivedAttack()
    {
        ChangeState<Move_Deer>();
        return true;
    }
    void FixedUpdate()
    {
        _currentState.OnFixedUpdate(this);
    }
    void OnAnimationEvent(AnimationEvent animationEvent)
    {
        _currentState.OnAnimationEvent(this, animationEvent);

    }
    public void ChangeState<T>() where T : StateBase_Deer, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }

}
