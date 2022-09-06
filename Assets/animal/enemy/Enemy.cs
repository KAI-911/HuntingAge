using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;

public class Enemy : MonoBehaviour
{
    //�i�r���b�V���G�[�W�F���g---------------------------------------------------------------------------
    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; set => _navMeshAgent = value; }

    //�A�j���[�^�[---------------------------------------------------------------------------------------
    private Animator _animator;
    public Animator Animator { get => _animator; set => _animator = value; }

    //��b�X�e�[�^�X-------------------------------------------------------------------------------------
    private Status _status;
    public Status Status { get => _status; set => _status = value; }

    //�U������-------------------------------------------------------------------------------------------
    private HitReceiver _hitReceiver;
    public HitReceiver HitReceiver { get => _hitReceiver; set => _hitReceiver = value; }

    //�x����ԂŌ����|�C���g----------------------------------------------------------------------------
    [SerializeField] private List<Vector3> _warningPos;
    public List<Vector3> WaningPos { get => _warningPos; set => _warningPos = value; }

    //��Q�������������f���郌�C����̔��ˈʒu------------------------------------------------------------
    [SerializeField] private GameObject _rayStartPos;

    //����p�͈�------------------------------------------------------------------------------------------
    [SerializeField] private float _searchAngle;

    //��]�p�x/�b
    [SerializeField] private float _rotationAngle;
    public float RotationAngle { get => _rotationAngle; }


    //�͈͓��ɋ��邩�̔���----------------------------------------------------------------------------------------
    [SerializeField] private TargetChecker[] _areaChecker;
    public TargetChecker[] AreaChecker { get => _areaChecker; }


    //�]�|���Ă��鎞��------------------------------------------------------------------------------------
    [SerializeField] private float _downTime;
    public float DownTime { get => _downTime; }

    //�{�̂̃��b�V�������_���[----------------------------------------------------------------------------
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    public SkinnedMeshRenderer SkinnedMeshRenderer { get => _skinnedMeshRenderer; set => _skinnedMeshRenderer = value; }


    //�}�e���A���������܂ł̎���--------------------------------------------------------------------------
    [SerializeField] private float _dissoveTime;
    public float DissoveTime { get => _dissoveTime; }

    //�}�e���A���������^�C�~���O--------------------------------------------------------------------------
    [SerializeField] private AnimationCurve _dissoveCurve;
    public AnimationCurve DissoveCurve { get => _dissoveCurve; }

    //ID
    [SerializeField] private string _enemyID;
    public string EnemyID { get => _enemyID; }

    private CollectionScript _collectionScript;
    public CollectionScript CollectionScript { get => _collectionScript;}
    //�U���Ώ�--------------------------------------------------------------------------------------------
    private GameObject _target;
    public GameObject Target { get => _target; }


    //������Ԃ̃t���O-----------------------------------------------------------------------------------
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
    /// �U�����󂯂Ă�����i�L�^����HP�ƍ���HP���r�jReceivedAttack���Ăяo��
    /// �߂�l��ReceivedAttack�Ɠ���
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
        //���G�͈͓�������p�͈͓����Ԃɏ�Q�����������true��Ԃ�
        //�܂茩���ċ�����true

        //���͈͂ɂ��邩�̔���
        if (!TargetChecker(TargetCheckerType.Search))
        {
            _discoverFlg = false;
            return false;
        }
        //���E�ɓ����Ă��邩�̔���
        var from = gameObject.transform.forward;
        var to = _target.transform.position - _rayStartPos.transform.position;
        var planeNormal = Vector3.up;
        var planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
        var planeTo = Vector3.ProjectOnPlane(to, planeNormal);
        // ���ʂɓ��e���ꂽ�x�N�g�����m�̕����t���p�x  ���v���Ő��A�����v���ŕ�
        var signedAngle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);
        if (_searchAngle / 2.0f < Mathf.Abs(signedAngle))
        {
            _discoverFlg = false;
            return false;
        }


        //�Ԃɏ�Q�������邩�̔���
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
        //_target�̕��������B���ŉ��p�x��_turningAngle�܂ŁB
        _turningAngle = Mathf.Abs(_turningAngle);

        var vec = _target.transform.position - transform.position;
        vec.y = 0;
        vec = vec.normalized;


        var nowVec = transform.forward;
        nowVec.y = 0;
        nowVec = nowVec.normalized;

        var signedAngle = Vector3.SignedAngle(nowVec, vec, Vector3.up);

        //��]����p�x�ȏ�Ȃ�_turningAngle�Ő�������
        if (_turningAngle < Mathf.Abs(signedAngle))
        {
            if (signedAngle < 0) _turningAngle = -_turningAngle;
            vec = Quaternion.Euler(0, 0, _turningAngle) * nowVec;
        }

        transform.LookAt(transform.position + vec);

    }

    public bool TargetChecker(TargetCheckerType type)
    {
        //type�͈̔͂ɑΏۂ������true��Ԃ�
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
        //�Ώۂ�����͈͂�TargetCheckerType��S�ĕԂ�
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
        //seconds�b�҂��Ă���action�����s
        //�Ăяo���Ƃ��� _ = WaitForAsync(1,()=>Delete()); �̂悤�Ȍ`��
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        action();
    }

    public void Delete()
    {
        //�������폜
        gameObject.SetActive(false);
    }

}
public enum State
{
    Idle,       //�ҋ@
    Move,       //�ړ�
    Wandering,  //�p�j
    Attack,     //�U��
    Roar,       //�Њd
    Down,       //�]�|
    Death,      //���S
    Escape      //����
}
public enum TargetCheckerType
{
    Non,    //�U�����Ȃ�
    Search, //���G
    Biting, //���݂�
    Tail,   //�K��
    Stomp,  //���݂�
    tackle, //�^�b�N��
    Gore    //�p�œ˂�

}
