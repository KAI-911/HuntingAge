using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
public partial class Player : MonoBehaviour
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


    //インプットシステム
    private InputControls _inputMove;
    private InputAction _inputMoveAction;
    public InputAction InputMoveAction { get => _inputMoveAction; }

    //最大速度
    [SerializeField] private float _maxSpeed;
    public float DashSpeed { get => _maxSpeed; set => _maxSpeed = value; }
    //ジャンプ力
    [SerializeField] private float _jumpPowor;
    public float JumpPowor { get => _jumpPowor; }
    //移動ベクトル
    private Vector3 _moveDirection = Vector3.zero;
    public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
    //カメラ
    [SerializeField] private Camera _playerCamera;
    public Camera PlayerCamera { get => _playerCamera; set => _playerCamera = value; }
    //着地判定
    [SerializeField] private GroundChecker _groundChecker;
    public GroundChecker GroundChecker { get => _groundChecker; }
    //回転
    private Quaternion _targetRotation;
    //今の状態
    private PlayerStateBase _currentState;

    private WeponChange _weponChange;
    public WeponChange WeponChange { get => _weponChange; set => _weponChange = value; }
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _status = GetComponent<Status>();
        _weponChange = GetComponent<WeponChange>();
        _targetRotation = transform.rotation;
        _inputMove = new InputControls();
        _currentState = new LocomotionState();
        _currentState.OnEnter(this, null);
    }

    private void OnEnable()
    {
        _inputMove.Player.Jump.started += Jump;
        _inputMove.Player.Dodge.started += Dodge;
        _inputMove.Player.StrongAttack.started += StrongAttack;

        _inputMoveAction = _inputMove.Player.Move;
        _inputMove.Player.Enable();
    }

    private void OnDisable()
    {
        _inputMove.Player.Jump.started -= Jump;
        _inputMove.Player.Dodge.started -= Dodge;
        _inputMove.Player.StrongAttack.started -= StrongAttack;

        _inputMove.Player.Disable();
    }

    void Start()
    {
        _weponChange.Change(WeponChange.WeponType.Axe);

    }

    void Update()
    {
        _currentState.OnUpdate(this);
        _animator.SetBool("IsGround", _groundChecker.IsGround());

        if (_status.HitReaction != HitReaction.nonReaction &&
            _currentState.GetType() != typeof(HitReactionState))
        {
            ChangeState<HitReactionState>();
        }
    }

    private void FixedUpdate()
    {
        _currentState.OnFixedUpdate(this);
    }

    private void OnCollisionStay(Collision collision)
    {
        _currentState.OnCollisionStay(this, collision);
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
        if (_currentState.GetType() != typeof(LocomotionState)) return;
        ChangeState<StrongAttack>();

    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (!_groundChecker.IsGround()) return;
        if (_currentState.GetType() != typeof(LocomotionState)) return;
        ChangeState<JumpState>();
    }
    private void Dodge(InputAction.CallbackContext obj)
    {
        if (!_groundChecker.IsGround()) return;
        if (_currentState.GetType() != typeof(LocomotionState)) return;
        if (_inputMoveAction.ReadValue<Vector2>().sqrMagnitude <= 0.1f) return;
        ChangeState<DodgeState>();
    }
    public void LookAt()
    {
        Vector3 direction = _moveDirection;
        direction.y = 0;
        if (_moveDirection.sqrMagnitude > 0.1f)
        {
            _targetRotation = Quaternion.LookRotation(direction);
            _rigidbody.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, 900 * Time.fixedDeltaTime);
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

}
public enum PlayerAnimationState
{
    Locomotion,
    Jump,
    Fall,
    Dodge,
    StrongAttack,
    HitReaction
}