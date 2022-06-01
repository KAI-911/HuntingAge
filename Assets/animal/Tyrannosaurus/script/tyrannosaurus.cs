using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;

public class Tyrannosaurus : MonoBehaviour
{
    //ナビメッシュエージェントーーーーーーーーーーーーーーーーーーーーーーーー
    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; set => _navMeshAgent = value; }

    //アニメーターーーーーーーーーーーーーーーーーーーーーーーーー
    private Animator _animator;
    public Animator Animator { get => _animator; set => _animator = value; }

    //現在の状態ーーーーーーーーーーーーーーーーーーーーーーーー
    private TyrannosaurusState _currentState;

    //基礎ステータス
    private Status _status;
    public Status Status { get => _status; set => _status = value; }

    //攻撃判定ーーーーーーーーーーーーーーーーーーーーーーーー
    private HitReceiver _hitReceiver;
    public HitReceiver HitReceiver { get => _hitReceiver; set => _hitReceiver = value; }


    //警戒状態のフラグーーーーーーーーーーーーーーーーーーーーーーーー
    private bool _warningFlg;
    public bool WarningFlg { get => _warningFlg; set => _warningFlg = value; }

    //発見状態のフラグーーーーーーーーーーーーーーーーーーーーーーーー
    private bool _discoverFlg;
    public bool DiscoverFlg { get => _discoverFlg; set => _discoverFlg = value; }

    //警戒状態で見回るポイントーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private GameObject[] _warningPos;
    public GameObject[] WaningPos { get => _warningPos; }

    //攻撃対象ーーーーーーーーーーーーーーーーーーーーーーーー
    private GameObject _target;
    public GameObject Target { get => _target; }

    //障害物が無いか判断するレイ判定の発射位置ーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private GameObject _rayStartPos;

    //視野角範囲ーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private float _searchAngle;

    //索敵範囲ーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private TargetChecker _searchArea;
    public TargetChecker SearchArea { get => _searchArea; }


    //噛みつき攻撃範囲ーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private TargetChecker _bitingAttackArea;
    public TargetChecker BitingAttackArea { get => _bitingAttackArea; }

    //尻尾攻撃範囲ーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private TargetChecker _tailAttackArea;
    public TargetChecker TailAttackArea { get => _tailAttackArea; }
   
    //尻尾攻撃範囲ーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private TargetChecker _stompAttackArea;
    public TargetChecker StompAttackArea { get => _stompAttackArea; }

    //転倒している時間ーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private float _downTime;
    public float DownTime { get => _downTime; }



    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        _currentState = new TyrannosaurusIdleState();
        _hitReceiver = GetComponent<HitReceiver>();
        _currentState.OnEnter(this, null);
    }

    void Start()
    {
        _status = GetComponent<Status>();
        _target = GameObject.FindWithTag("Player");
        _discoverFlg = false;
        _warningFlg = false;
    }

    void Update()
    {
        if (_status.DownFlg && _currentState.GetType() != typeof(TyrannosaurusDownState))
        {
            ChangeState<TyrannosaurusDownState>();
        }

        if (_status.HP<=0 && _currentState.GetType() != typeof(TyrannosaurusDeathState))
        {
            Debug.Log("death");
            ChangeState<TyrannosaurusDeathState>();
        }
        _animator.SetInteger("HP", Status.HP);
        _currentState.OnUpdate(this);
    }
    private void FixedUpdate()
    {
        _currentState.OnFixedUpdate(this);
    }
    private void OnAnimationEvent(AnimationEvent animationEvent)
    {
        _currentState.OnAnimationEvent(this, animationEvent);
    }
    public void ChangeState<T>() where T : TyrannosaurusState, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }
    public bool Search()
    {
        //一定範囲にいるかの判定
        if (!_searchArea.TriggerHit) return false;

        //視界に入っているかの判定
        var from = gameObject.transform.forward;
        var to = _target.transform.position - _rayStartPos.transform.position;
        // 平面の法線ベクトル（上向きベクトルとする）
        var planeNormal = Vector3.up;
        // 平面に投影されたベクトルを求める
        var planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
        var planeTo = Vector3.ProjectOnPlane(to, planeNormal);
        // 平面に投影されたベクトル同士の符号付き角度  時計回りで正、反時計回りで負
        var signedAngle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);
        if (_searchAngle / 2.0f < Mathf.Abs(signedAngle)) return false;

        //間に障害物があるかの判定
        Vector3 dir = to.normalized;
        RaycastHit[] raycastHits = new RaycastHit[10];
        Ray ray = new Ray
        {
            origin = _rayStartPos.transform.position,
            direction = dir
        };
        int hitCount = Physics.RaycastNonAlloc(ray, raycastHits, to.magnitude, 0);
        if (hitCount > 0) return false;

        return true;
    }

    public void LookToTarget()
    {
        var vec = this._target.transform.position - this.transform.position;
        vec.y = 0;
        vec = vec.normalized;
        this.transform.LookAt(this.transform.position + vec);

    }
    public async Task WaitForAsync(float seconds, Action action)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        action();
    }

}
public enum TyrannosaurusAnimationState
{
    Idle,
    Move,
    Attack,
    Roar,
    Wandering,
    Down,
    Death
}
public enum TyrannosaurusAttack
{
    Non,
    Biting,
    Tail,
    Stomp
}

