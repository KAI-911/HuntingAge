using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
public partial class Player : Singleton<Player>
{
    //リジッドボディー
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }
    //アニメーター
    private Animator _animator;
    public Animator Animator { get => _animator; set => _animator = value; }
    //基礎ステータス
    private Status _status;
    public Status Status { get => _status; set => _status = value; }
    //攻撃判定
    [SerializeField] private HitReceiver _hitReceiver;
    public HitReceiver HitReceiver { get => _hitReceiver; set => _hitReceiver = value; }
    //最大速度
    [SerializeField] private float _maxSpeed;
    public float MaxSpeed { get => _maxSpeed; set => _maxSpeed = value; }
    //ジャンプ力
    [SerializeField] private float _jumpPowor;
    public float JumpPowor { get => _jumpPowor; }
    //カメラ
    [SerializeField] private Camera _playerCamera;
    public Camera PlayerCamera { get => _playerCamera; set => _playerCamera = value; }
    //着地判定
    [SerializeField] private GroundChecker _groundChecker;
    public GroundChecker GroundChecker { get => _groundChecker; }

    //着地時に手をつく速さ(正の値)
    [SerializeField] float _fallLength;
    public float FallLength { get => _fallLength; }
    //採取時間
    [SerializeField] float _collectionTime;
    public float CollectionTime { get => _collectionTime; }

    //インプットシステム
    private InputControls _inputMove;
    private InputAction _inputMoveAction;
    public InputAction InputMoveAction { get => _inputMoveAction; }
    //移動ベクトル
    private Vector3 _moveDirection = Vector3.zero;
    public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
    //回転
    private Quaternion _targetRotation;
    //今の状態
    private PlayerStateBase _currentState;
    public PlayerStateBase CurrentState { get => _currentState; }

    //プレイヤーの移動ができるか
    [SerializeField] private bool _isAction;
    public bool IsAction { get => _isAction; set => _isAction = value; }

    //武器切り替え
    [SerializeField] string _weaponID;
    [SerializeField] GameObject _weaponParent;
    private GameObject _weapon;

    //復活用
    [SerializeField] private List<Position> _startPos;
    public List<Position> StartPos { get => _startPos; set => _startPos = value; }
    public bool CollectionFlg { get => _collectionFlg; set => _collectionFlg = value; }


    //採取用
    private bool _collectionFlg;
    private CollectionScript _collectionScript;
    public CollectionScript CollectionScript { get => _collectionScript; set => _collectionScript = value; }

    protected override void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _status = GetComponent<Status>();
        _targetRotation = transform.rotation;
        _inputMove = new InputControls();
        _currentState = new LocomotionState();
        _currentState.OnEnter(this, null);
        base.Awake();
    }
    private void OnEnable()
    {
        _inputMove.Player.Jump.started += Jump;
        _inputMove.Player.Dodge.started += Dodge;
        _inputMove.Player.StrongAttack.started += StrongAttack;
        _inputMove.Player.WeakAttack.started += WeakAttack;
        _inputMoveAction = _inputMove.Player.Move;
        _inputMove.Player.Enable();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }


    private void OnDisable()
    {
        _inputMove.Player.Jump.started -= Jump;
        _inputMove.Player.Dodge.started -= Dodge;
        _inputMove.Player.StrongAttack.started -= StrongAttack;
        _inputMove.Player.WeakAttack.started -= WeakAttack;
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;

        _inputMove.Player.Disable();
    }

    void Start()
    {
        _animator.SetInteger("HP", _status.HP);
        _isAction = true;
        WeponChange(_weaponID);
    }

    void Update()
    {
        _currentState.OnUpdate(this);
        _animator.SetBool("IsGround", _groundChecker.IsGround());
        _animator.SetInteger("HP", _status.HP);
        if (_status.HitReaction != HitReaction.nonReaction &&
            _currentState.GetType() != typeof(HitReactionState) &&
            _currentState.GetType() != typeof(DeathState) &&
            _currentState.GetType() != typeof(VillageAction))
        {
            ChangeState<HitReactionState>();
        }
        if (_status.HP == 0 &&
            _currentState.GetType() != typeof(DeathState)&&
            _currentState.GetType() != typeof(VillageAction))
        {
            ChangeState<DeathState>();
        }

        Debug.Log(_currentState.GetType().ToString());
    }

    private void FixedUpdate()
    {
        _currentState.OnFixedUpdate(this);
    }

    private void OnCollisionStay(Collision collision)
    {
        _currentState.OnCollisionStay(this, collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CollectionPoint"))
        {
            Debug.Log("採取ポイント");
            _collectionFlg = true;
            _collectionScript = other.transform.root.GetComponent<CollectionScript>();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("CollectionPoint"))
        {
            Debug.Log("採取ポイント");
            _collectionFlg = true;
        }
       
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CollectionPoint"))
        {
            _collectionFlg = false;
        }

    }
    private void OnAnimationEvent(AnimationEvent animationEvent)
    {
        _currentState.OnAnimationEvent(this, animationEvent);
    }

    public Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    public Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }


    private void StrongAttack(InputAction.CallbackContext obj)
    {
        if (!_isAction) return;
        _currentState.OnStrongAttack(this);
    }
    private void WeakAttack(InputAction.CallbackContext obj)
    {
        if (!_isAction) return;
        _currentState.OnWeakAttack(this);
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (!_isAction) return;
        _currentState.OnJump(this);
    }
    private void Dodge(InputAction.CallbackContext obj)
    {
        if (!_isAction) return;
        _currentState.OnDodge(this);
    }
    public void LookAt(float _turningAngle = 900)
    {
        Vector3 direction = _moveDirection;
        direction.y = 0;
        if (_moveDirection.sqrMagnitude > 0.1f)
        {
            _targetRotation = Quaternion.LookRotation(direction);
            _rigidbody.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _turningAngle * Time.fixedDeltaTime);
        }
        else
        {
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
    public void ChangeState<T>() where T : PlayerStateBase, new()
    {
        var nextState = new T();
        _currentState.OnExit(this, nextState);
        nextState.OnEnter(this, _currentState);
        _currentState = nextState;
    }
    public async Task WaitForAsync(float seconds, Action action)
    {
        await Task.Delay(TimeSpan.FromSeconds(seconds));
        action();
    }

    public void Revival()
    {
        _status.HP = _status.MaxHP;
        var pos = StartPos.Find(n => n.scene == GameManager.Instance.Quest.QuestData.Field);
        transform.position = pos.pos[0];
        ChangeState<LocomotionState>();
        _animator.SetInteger("HP", _status.HP);
        _status.HitReaction = HitReaction.nonReaction;
    }
    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        if(GameManager.Instance.Quest.IsQuest)
        {
            ChangeState<LocomotionState>();
        }
        else
        {
            _status.HP = _status.MaxHP;
            ChangeState<VillageAction>();
        }
        var pos = StartPos.Find(n => n.scene == GameManager.Instance.NowScene);
        transform.position = pos.pos[0];
    }
    public void WeponChange(string weponID)
    {
        if (!GameManager.Instance.WeaponDataList.Dictionary.ContainsKey(weponID)) return;
        _weaponID = weponID;
        //既に武器を持っていたらそれと削除
        if (_weapon != null)
        {
            Destroy(_weapon);
            _weapon = null;
            Resources.UnloadUnusedAssets();
        }
        //インスタンス化
        var path = GameManager.Instance.WeaponDataList.Dictionary[weponID].weaponPath;
        _weapon = Instantiate(Resources.Load(path), _weaponParent.transform.position, _weaponParent.transform.rotation) as GameObject;
        _weapon.transform.SetParent(_weaponParent.transform);
        _weapon.transform.localScale = new Vector3(1, 1, 1);
    }
    public void WeponDelete()
    {
        if (_weapon != null)
        {
            Destroy(_weapon);
            _weapon = null;
            Resources.UnloadUnusedAssets();
        }
    }
}
public enum PlayerAnimationState
{
    Locomotion,
    Jump,
    Fall,
    Dodge,
    StrongAttack,
    HitReaction,
    Death,
    Collection,
    Landing
}
public enum AttackType
{
    WeakAttack,
    StrongAttack
}