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

    /// <summary>
    /// 攻撃を受けたか確認するため
    /// </summary>
    private int _hitPointSave;
    public int HitPointSave { get => _hitPointSave; set => _hitPointSave = value; }

    [SerializeField] private AnimationCurve _ditherCurve;
    public AnimationCurve DitherCurve { get => _ditherCurve; }

    void Start()
    {
        _currentState = new Idle_Dodo();
        _currentState.OnEnter(this, null);
        _hitPointSave = Status.HP;
        foreach (var pos in GameObject.FindGameObjectsWithTag("EscapePosition"))
        {
            _escapePos.Add(pos.transform.position);
        }
    }
    void Update()
    {
        Animator.SetInteger("HP", Status.HP);


        if (_hitPointSave != Status.HP && CurrentState.GetType() != typeof(Escape_Dodo))
        {
            ChangeState<Escape_Dodo>();
            _hitPointSave = Status.HP;

            //周りにいるドードーも一緒に逃げる
            if (QuestManager != null)
            {
                //ドードーのリスト作成
                List<Dodo> dodoList = new List<Dodo>();
                foreach (var enemy in QuestManager.EnemyList)
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
            }

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
