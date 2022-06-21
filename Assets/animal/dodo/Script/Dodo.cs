using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodo : Enemy
{
    [SerializeField] private StateBase_Dodo _currentState;
    public StateBase_Dodo CurrentState { get => _currentState; }
    /// <summary>
    /// ì¶Ç∞ÇÈèÍèä
    /// </summary>
    [SerializeField] private List<Vector3> _escapePos;
    public List<Vector3> EscapePos { get => _escapePos; set => _escapePos = value; }

    /// <summary>
    /// çUåÇÇéÛÇØÇΩÇ©ämîFÇ∑ÇÈÇΩÇﬂ
    /// </summary>
    private int _hitPointSave;
    public int HitPointSave { get => _hitPointSave; set => _hitPointSave = value; }



    void Start()
    {
        _currentState = new Idle_Dodo();
        _currentState.OnEnter(this, null);
        _hitPointSave = Status.HP;

    }
    void Update()
    {
        Animator.SetInteger("HP", Status.HP);
        
        if (_hitPointSave != Status.HP && CurrentState.GetType() != typeof(Escape_Dodo))
        {
            ChangeState<Escape_Dodo>();
            _hitPointSave = Status.HP;
        }


        _currentState.OnUpdate(this);

        //if (Status.DownFlg && CurrentState.GetType() != typeof(Down_Trex))
        //{
        //    ChangeState<Down_Trex>();
        //}
        if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Dodo))
        {
            ChangeState<Death_Dodo>();
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
    public void ChangeState<T>() where T : StateBase_Dodo, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }

}
