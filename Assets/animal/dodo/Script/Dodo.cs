using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dodo : Enemy
{
    [SerializeField] private StateBase_Dodo _currentState;
    public StateBase_Dodo CurrentState { get => _currentState; }
    /// <summary>
    /// 逃げる場所
    /// </summary>
    [SerializeField] private List<Vector3> _escapePos;
    public List<Vector3> EscapePos { get => _escapePos; set => _escapePos = value; }


    [SerializeField] private AnimationCurve _ditherCurve;
    public AnimationCurve DitherCurve { get => _ditherCurve; }

    void Start()
    {
        _currentState = new Idle_Dodo();
        _currentState.OnEnter(this, null);
        foreach (var pos in GameObject.FindGameObjectsWithTag("EscapePosition"))
        {
            _escapePos.Add(pos.transform.position);
        }
    }
    void Update()
    {
        Animator.SetInteger("HP", Status.HP);


        if (ReceivedAttackCheck()) return;
        _currentState.OnUpdate(this);


        if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Dodo))
        {
            ChangeState<Death_Dodo>();
        }
    }
    public override bool ReceivedAttack()
    {
        if (CurrentState.GetType() != typeof(Escape_Dodo))
        {
            ChangeState<Escape_Dodo>();

            //周りにいるドードーも一緒に逃げる
            //ドードーのリスト作成
            List<Dodo> dodoList = new List<Dodo>();
            foreach (var enemy in GameManager.Instance.Quest.EnemyList)
            {
                if (enemy.EnemyID == EnemyID) dodoList.Add(enemy.GetComponent<Dodo>());
            }
            //どのくらいの距離までのドードーも一緒に逃げるか
            float dis = 0;
            foreach (var item in AreaChecker)
            {
                if (TargetCheckerType.Search != item.TargetCheckerType) continue;
                dis = item.transform.localScale.x;
                break;
            }
            Debug.Log(dis);
            //magnitudeはルートの計算をするので軽いsqrMagnitudeを使う
            //sqrMagnitudeは２乗の形になるのでdisもそれに合わせる
            dis = dis * dis;
            foreach (var dodo in dodoList)
            {
                if (!dodo.gameObject.activeSelf) continue;
                if (dodo.CurrentState.GetType() == typeof(Escape_Dodo)) continue;
                if (dodo.CurrentState.GetType() == typeof(Death_Dodo)) continue;
                var sqrMag = (transform.position - dodo.transform.position).sqrMagnitude;
                if (dis < sqrMag) continue;
                dodo.ChangeState<Escape_Dodo>();
                //_ = dodo.WaitForAsync((dis / sqrMag) * 10, () => dodo.ChangeState<Escape_Dodo>());
            }
            return true;
        }
        return false;
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
