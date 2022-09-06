using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
public partial class Player : Singleton<Player>
{
    private PlayerStatusData _statusData;
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
    public string WeaponID { get => _weaponID; set => _weaponID = value; }

    //復活用
    [SerializeField] private List<Position> _startPos;
    public List<Position> StartPos { get => _startPos; set => _startPos = value; }


    //採取用
    private CollectionScript _collectionScript;
    public CollectionScript CollectionScript { get => _collectionScript; set => _collectionScript = value; }

    //ポップアップ
    private PopImage _popImage;
    protected override void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _status = GetComponent<Status>();
        _statusData = GetComponent<PlayerStatusData>();
        _targetRotation = transform.rotation;
        _inputMove = new InputControls();
        _currentState = new LocomotionState();
        _currentState.OnEnter(this, null);
        base.Awake();
    }
    private void OnEnable()
    {
        //_inputMove.Player.Jump.started += Jump;
        _inputMove.Player.Collection.started += Collect;
        _inputMove.Player.Dodge.started += Dodge;
        _inputMove.Player.StrongAttack.started += StrongAttack;
        _inputMove.Player.WeakAttack.started += WeakAttack;
        _inputMoveAction = _inputMove.Player.Move;
        _inputMove.Player.Enable();
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }



    private void OnDisable()
    {
        //_inputMove.Player.Jump.started -= Jump;
        _inputMove.Player.Collection.started -= Collect;
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

    }

    void Update()
    {
        Debug.Log(_currentState.GetType());
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
            _currentState.GetType() != typeof(DeathState) &&
            _currentState.GetType() != typeof(VillageAction))
        {
            ChangeState<DeathState>();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CollectionPoint"))
        {
            Debug.Log("採取ポイント");
            //採取回数が０以下なら何もしない
            if (other.GetComponent<CollectionScript>().CollectableTimes <= 0) return;

            if (_collectionScript != null)
            {
                _collectionScript.DeleteImage();
            }
            _collectionScript = other.GetComponent<CollectionScript>();
            _collectionScript.CreateImage();
        }
        if (other.CompareTag("PopImage"))
        {
            if (_popImage != null)
            {
                _popImage.DeleteImage();
            }
            _popImage = other.GetComponent<PopImage>();
            _popImage.CreateImage();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("CollectionPoint"))
        {
            //採取回数が０以下なら表示を消す
            if (_collectionScript != null && other.GetComponent<CollectionScript>().CollectableTimes <= 0)
            {
                _collectionScript.DeleteImage();
                _collectionScript = null;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CollectionPoint"))
        {
            if (_collectionScript != null)
            {
                _collectionScript.DeleteImage();
                _collectionScript = null;
            }
        }
        if (other.CompareTag("PopImage"))
        {
            if (_popImage != null)
            {
                _popImage.DeleteImage();
                _popImage = null;
            }
        }

    }
    private void OnAnimationEvent(AnimationEvent animationEvent)
    {
        _currentState.OnAnimationEvent(this, animationEvent);
    }

    private void OnPlaySE(AnimationEvent animationEvent)
    {
        if (animationEvent.objectReferenceParameter != null)
        {
            Instantiate(animationEvent.objectReferenceParameter);
        }
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
        if (GameManager.Instance.UIItemView.IsSelect()) return;
        _currentState.OnStrongAttack(this);
    }
    private void WeakAttack(InputAction.CallbackContext obj)
    {
        if (!_isAction) return;
        if (GameManager.Instance.UIItemView.IsSelect()) return;
        _currentState.OnWeakAttack(this);
    }
    private void Collect(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.UIItemView.IsSelect()) return;
        _currentState.OnCollect(this);
    }
    private void Jump(InputAction.CallbackContext obj)
    {
        if (!_isAction) return;
        if (GameManager.Instance.UIItemView.IsSelect()) return;
        _currentState.OnJump(this);
    }
    private void Dodge(InputAction.CallbackContext obj)
    {
        if (!_isAction) return;
        if (GameManager.Instance.UIItemView.IsSelect()) return;
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
        GameManager.Instance.UIItemView.ClearPermanentBuff();
        _status.MaxHP = _statusData.MaxHP;
        _status.MaxSP = _statusData.MaxSP;
        _status.Attack = _statusData.Attack;
        _status.Defense = _statusData.Defense;
        _status.HP = _status.MaxHP;
        _status.SP = _status.MaxSP;

        var pos = StartPos.Find(n => n.scene == GameManager.Instance.Quest.QuestData.Field);
        transform.position = pos.pos[0];
        _animator.SetInteger("HP", _status.HP);
        _status.HitReaction = HitReaction.nonReaction;
        _status.InvincibleFlg = false;
        ChangeState<LocomotionState>();


        Debug.Log("Revival");
    }
    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        if (GameManager.Instance.Quest.IsQuest)
        {
            ChangeWepon(_weaponID);
            _status.MaxHP = _statusData.MaxHP;
            _status.MaxSP = _statusData.MaxSP;
            _status.Attack = _statusData.Attack;
            _status.Defense = _statusData.Defense;
            _status.HP = _status.MaxHP;
            _status.SP = _status.MaxSP;
            ChangeState<LocomotionState>();
        }
        else
        {
            DeleteWepon();
            _status.MaxHP = _statusData.MaxHP;
            _status.MaxSP = _statusData.MaxSP;
            _status.Attack = _statusData.Attack;
            _status.Defense = _statusData.Defense;
            _status.HP = _status.MaxHP;
            _status.SP = _status.MaxSP;
            ChangeState<VillageAction>();
        }
        var pos = StartPos.Find(n => n.scene == GameManager.Instance.NowScene);
        transform.position = pos.pos[0];
    }
    private void ChangeWepon(string weponID)
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
        _status.Attack = GameManager.Instance.WeaponDataList.Dictionary[weponID].AttackPoint;
        if (_weaponID.Contains("weapon1"))
        {
            _animator.SetFloat("wepon", 0);
        }
        else if(_weaponID.Contains("weapon2"))
        {
            _animator.SetFloat("wepon", 1);
        }
    }
    public void DeleteWepon()
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
    Attack,
    HitReaction,
    Death,
    Collection,
    Landing,
    ItemUseing
}
public enum AttackType
{
    WeakAttack,
    StrongAttack
}