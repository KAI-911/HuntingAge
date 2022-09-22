using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trex : Enemy
{
    [SerializeField] private StateBase_Trex _currentState;
    public StateBase_Trex CurrentState { get => _currentState; }

    void Start()
    {
        _currentState = new Idle_Trex();
        _currentState.OnEnter(this, null);
    }
    void Update()
    {
        Animator.SetInteger("HP", Status.HP);
        _currentState.OnUpdate(this);

        if (Status.DownFlg && CurrentState.GetType() != typeof(Down_Trex))
        {
            ChangeState<Down_Trex>();
        }
        if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Trex))
        {
            ChangeState<Death_Trex>();
        }
    }
    public override bool ReceivedAttack()
    {
        ChangeState<Move_Trex>();
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
    public void ChangeState<T>() where T : StateBase_Trex, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }

    void CreateSound(AnimationEvent Sound)
    {   
        var obj = Instantiate(Sound.objectReferenceParameter)as GameObject;
        obj.transform.position = transform.position;
        obj.GetComponent<AudioSource>().Play();
    }

}
