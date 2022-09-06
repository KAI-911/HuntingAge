using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;

public class Enemy : MonoBehaviour
{
    //ナビメッシュエージェント---------------------------------------------------------------------------
    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; set => _navMeshAgent = value; }

    //アニメーター---------------------------------------------------------------------------------------
    private Animator _animator;
    public Animator Animator { get => _animator; set => _animator = value; }

    //基礎ステータス-------------------------------------------------------------------------------------
    private Status _status;
    public Status Status { get => _status; set => _status = value; }

    //攻撃判定-------------------------------------------------------------------------------------------
    private HitReceiver _hitReceiver;
    public HitReceiver HitReceiver { get => _hitReceiver; set => _hitReceiver = value; }

    //警戒状態で見回るポイント----------------------------------------------------------------------------
    [SerializeField] private List<Vector3> _warningPos;
    public List<Vector3> WaningPos { get => _warningPos; set => _warningPos = value; }

    //障害物が無いか判断するレイ判定の発射位置------------------------------------------------------------
    [SerializeField] private GameObject _rayStartPos;

    //視野角範囲------------------------------------------------------------------------------------------
    [SerializeField] private float _searchAngle;

    //回転角度/秒
    [SerializeField] private float _rotationAngle;
    public float RotationAngle { get => _rotationAngle; }


    //範囲内に居るかの判定----------------------------------------------------------------------------------------
    [SerializeField] private TargetChecker[] _areaChecker;
    public TargetChecker[] AreaChecker { get => _areaChecker; }


    //転倒している時間------------------------------------------------------------------------------------
    [SerializeField] private float _downTime;
    public float DownTime { get => _downTime; }

    //本体のメッシュレンダラー----------------------------------------------------------------------------
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    public SkinnedMeshRenderer SkinnedMeshRenderer { get => _skinnedMeshRenderer; set => _skinnedMeshRenderer = value; }


    //マテリアルを消すまでの時間--------------------------------------------------------------------------
    [SerializeField] private float _dissoveTime;
    public float DissoveTime { get => _dissoveTime; }

    //マテリアルを消すタイミング--------------------------------------------------------------------------
    [SerializeField] private AnimationCurve _dissoveCurve;
    public AnimationCurve DissoveCurve { get => _dissoveCurve; }

    //ID
    [SerializeField] private string _enemyID;
    public string EnemyID { get => _enemyID; }

    private CollectionScript _collectionScript;
    public CollectionScript CollectionScript { get => _collectionScript;}
    //攻撃対象--------------------------------------------------------------------------------------------
    private GameObject _target;
    public GameObject Target { get => _target; }


    //発見状態のフラグ-----------------------------------------------------------------------------------
    private bool _discoverFlg;
    public bool DiscoverFlg { get => _discoverFlg; set => _discoverFlg = value; }

    private RunOnce Run_death = new RunOnce();

    private int _keepHP;


    [SerializeField] List<GameObject> DebugSetPosObj;
    [ContextMenu("DebugSetPosObjSet")]
    public void DebugSetPosObjSet()
    {
        _warningPos.Clear();
        foreach (var item in DebugSetPosObj)
        {
            _warningPos.Add(item.transform.position);
        }
        var data = GameManager.Instance.EnemyDataList;
        int i = data.Keys.IndexOf(_enemyID);
        var pos =data.Values[i].EnemyPos.Find(n => n.scene == GameManager.Instance.NowScene);
        pos.pos = _warningPos;
        data.DesrializeDictionary();
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();
        _hitReceiver = GetComponent<HitReceiver>();
        _status = GetComponent<Status>();
        _collectionScript = GetComponentInChildren<CollectionScript>();
        _target = GameObject.FindWithTag("Player");
        _discoverFlg = false;
        _keepHP = _status.HP;
        GameManager.Instance.Quest.AddEnemy(this);
        _collectionScript.gameObject.SetActive(false);
    }

    /// <summary>
    /// 攻撃を受けていたら（記録したHPと今のHPを比較）ReceivedAttackを呼び出す
    /// 戻り値はReceivedAttackと同じ
    /// </summary>
    /// <returns></returns>
    public bool ReceivedAttackCheck()
    {
        if (_keepHP != _status.HP)
        {
            _keepHP = _status.HP;
            ReceivedAttack();
            _discoverFlg = true;
            return true;
        }
        return false;
    }
    public virtual bool ReceivedAttack()
    {
        return false;
    }
    public void Death()
    {
        Run_death.Run(() => GameManager.Instance.Quest.KillEnemyCount.Add(EnemyID));
    }


    public bool Search()
    {
        //索敵範囲内かつ視野角範囲内かつ間に障害物が無ければtrueを返す
        //つまり見つけて居たらtrue

        //一定範囲にいるかの判定
        if (!TargetChecker(TargetCheckerType.Search))
        {
            _discoverFlg = false;
            return false;
        }
        //視界に入っているかの判定
        var from = gameObject.transform.forward;
        var to = _target.transform.position - _rayStartPos.transform.position;
        var planeNormal = Vector3.up;
        var planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
        var planeTo = Vector3.ProjectOnPlane(to, planeNormal);
        // 平面に投影されたベクトル同士の符号付き角度  時計回りで正、反時計回りで負
        var signedAngle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);
        if (_searchAngle / 2.0f < Mathf.Abs(signedAngle))
        {
            _discoverFlg = false;
            return false;
        }


        //間に障害物があるかの判定
        Vector3 dir = to.normalized;
        RaycastHit[] raycastHits = new RaycastHit[10];
        Ray ray = new Ray
        {
            origin = _rayStartPos.transform.position,
            direction = dir
        };
        int hitCount = Physics.RaycastNonAlloc(ray, raycastHits, to.magnitude, 0);
        if (hitCount > 0)
        {
            _discoverFlg = false;
            return false;
        }

        _discoverFlg = true;
        return true;
    }

    public void LookToTarget(int _turningAngle)
    {
        //_targetの方を向く。一回で回る角度は_turningAngleまで。
        _turningAngle = Mathf.Abs(_turningAngle);

        var vec = _target.transform.position - transform.position;
        vec.y = 0;
        vec = vec.normalized;


        var nowVec = transform.forward;
        nowVec.y = 0;
        nowVec = nowVec.normalized;

        var signedAngle = Vector3.SignedAngle(nowVec, vec, Vector3.up);

        //回転する角度以上なら_turningAngleで制限する
        if (_turningAngle < Mathf.Abs(signedAngle))
        {
            if (signedAngle < 0) _turningAngle = -_turningAngle;
            vec = Quaternion.Euler(0, 0, _turningAngle) * nowVec;
        }

        transform.LookAt(transform.position + vec);

    }

    public bool TargetChecker(TargetCheckerType type)
    {
        //typeの範囲に対象が居ればtrueを返す
        foreach (var item in _areaChecker)
        {
            if (type != item.TargetCheckerType) continue;
            if (item.TriggerHit) return true;
            break;
        }
        return false;
    }
    public List<TargetCheckerType> TargetChecker()
    {
        //対象が居る範囲のTargetCheckerTypeを全て返す
        List<TargetCheckerType> list = new List<TargetCheckerType>();
        foreach (var item in _areaChecker)
        {
            if (item.TriggerHit)
            {
                list.Add(item.TargetCheckerType);
            }
        }
        return list;
    }
    public async Task WaitForAsync(float seconds, Action action)
    {
        //seconds秒待ってからactionを実行
        //呼び出すときは _ = WaitForAsync(1,()=>Delete()); のような形で
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        action();
    }

    public void Delete()
    {
        //自分を削除
        gameObject.SetActive(false);
    }

}
public enum State
{
    Idle,       //待機
    Move,       //移動
    Wandering,  //徘徊
    Attack,     //攻撃
    Roar,       //威嚇
    Down,       //転倒
    Death,      //死亡
    Escape      //逃走
}
public enum TargetCheckerType
{
    Non,    //攻撃しない
    Search, //索敵
    Biting, //噛みつき
    Tail,   //尻尾
    Stomp,  //踏みつけ
    tackle, //タックル
    Gore    //角で突く

}
