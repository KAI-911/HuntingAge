using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhino : Enemy
{
    [SerializeField] private StateBase_Rhino _currentState;
    public StateBase_Rhino CurrentState { get => _currentState; }

    void Start()
    {
        _currentState = new Idle_Rhino();
        _currentState.OnEnter(this, null);
    }
    void Update()
    {
        Animator.SetInteger("HP", Status.HP);
        _currentState.OnUpdate(this);

        if (Status.DownFlg && CurrentState.GetType() != typeof(Down_Rhino))
        {
            ChangeState<Down_Rhino>();
        }
        if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Rhino))
        {
            ChangeState<Death_Rhino>();
        }
    }
    public override bool ReceivedAttack()
    {
        ChangeState<StateBase_Rhino>();
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
    public void ChangeState<T>() where T : StateBase_Rhino, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }

}
