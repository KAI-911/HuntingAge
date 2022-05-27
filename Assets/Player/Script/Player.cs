using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
public partial class Player : MonoBehaviour
{
    //���W�b�h�{�f�B�[
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody { get => _rigidbody; set => _rigidbody = value; }
    //�A�j���[�^�[
    private Animator _animator;
    public Animator Animator { get => _animator; set => _animator = value; }
    //��b�X�e�[�^�X
    private Status _status;
    public Status Status { get => _status; set => _status = value; }

    //�U�����󂯂����̔���
    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; }

    //�U������
    [SerializeField] private HitReceiver _hitReceiver;
    public HitReceiver HitReceiver { get => _hitReceiver; }


    //�C���v�b�g�V�X�e��
    private InputControls _inputMove;
    private InputAction _inputMoveAction;
    public InputAction InputMoveAction { get => _inputMoveAction; }

    //�_�b�V�����x
    [SerializeField] private float _dashSpeed;
    public float DashSpeed { get => _dashSpeed; set => _dashSpeed = value; }
    //�W�����v��
    [SerializeField] private float _jumpPowor;
    public float JumpPowor { get => _jumpPowor; }
    //��𑬓x
    [SerializeField] private float _dodgePowor;
    public float DodgePower { get => _dodgePowor; }
    //�ړ��x�N�g��
    private Vector3 _moveDirection = Vector3.zero;
    public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
    //�J����
    [SerializeField] private Camera _playerCamera;
    public Camera PlayerCamera { get => _playerCamera; set => _playerCamera = value; }
    //���n����
    [SerializeField] private GroundChecker _groundChecker;
    public GroundChecker GroundChecker { get => _groundChecker; }
    //��]
    private Quaternion _targetRotation;
    //���̏��
    private PlayerStateBase _currentState;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _inputMove = new InputControls();
        _targetRotation = transform.rotation;
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
        _hitReaction = HitReaction.nonReaction;
    }

    void Update()
    {
        _currentState.OnUpdate(this);
        _animator.SetBool("IsGround", _groundChecker.IsGround());
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
    //private void Attack2(InputAction.CallbackContext obj)
    //{
    //    //animation�̐؂�ւ�
    //}
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
    StrongAttack
}