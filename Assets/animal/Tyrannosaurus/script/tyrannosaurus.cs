using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;

public class Tyrannosaurus : MonoBehaviour
{
    //�i�r���b�V���G�[�W�F���g�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    private NavMeshAgent _navMeshAgent;
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; set => _navMeshAgent = value; }

    //�A�j���[�^�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    private Animator _animator;
    public Animator Animator { get => _animator; set => _animator = value; }

    //���݂̏�ԁ[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    private TyrannosaurusState _currentState;

    //��b�X�e�[�^�X
    private Status _status;
    public Status Status { get => _status; set => _status = value; }

    //�U������[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    private HitReceiver _hitReceiver;
    public HitReceiver HitReceiver { get => _hitReceiver; set => _hitReceiver = value; }


    //�x����Ԃ̃t���O�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    private bool _warningFlg;
    public bool WarningFlg { get => _warningFlg; set => _warningFlg = value; }

    //������Ԃ̃t���O�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    private bool _discoverFlg;
    public bool DiscoverFlg { get => _discoverFlg; set => _discoverFlg = value; }

    //�x����ԂŌ����|�C���g�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private GameObject[] _warningPos;
    public GameObject[] WaningPos { get => _warningPos; }

    //�U���Ώہ[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    private GameObject _target;
    public GameObject Target { get => _target; }

    //��Q�������������f���郌�C����̔��ˈʒu�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private GameObject _rayStartPos;

    //����p�͈́[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private float _searchAngle;

    //���G�͈́[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private TargetChecker _searchArea;
    public TargetChecker SearchArea { get => _searchArea; }


    //���݂��U���͈́[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private TargetChecker _bitingAttackArea;
    public TargetChecker BitingAttackArea { get => _bitingAttackArea; }

    //�K���U���͈́[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private TargetChecker _tailAttackArea;
    public TargetChecker TailAttackArea { get => _tailAttackArea; }
   
    //�K���U���͈́[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
    [SerializeField] private TargetChecker _stompAttackArea;
    public TargetChecker StompAttackArea { get => _stompAttackArea; }

    //�]�|���Ă��鎞�ԁ[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[
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
        //���͈͂ɂ��邩�̔���
        if (!_searchArea.TriggerHit) return false;

        //���E�ɓ����Ă��邩�̔���
        var from = gameObject.transform.forward;
        var to = _target.transform.position - _rayStartPos.transform.position;
        // ���ʂ̖@���x�N�g���i������x�N�g���Ƃ���j
        var planeNormal = Vector3.up;
        // ���ʂɓ��e���ꂽ�x�N�g�������߂�
        var planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
        var planeTo = Vector3.ProjectOnPlane(to, planeNormal);
        // ���ʂɓ��e���ꂽ�x�N�g�����m�̕����t���p�x  ���v���Ő��A�����v���ŕ�
        var signedAngle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);
        if (_searchAngle / 2.0f < Mathf.Abs(signedAngle)) return false;

        //�Ԃɏ�Q�������邩�̔���
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

