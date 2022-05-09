using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
public partial class Player : MonoBehaviour, IAttackDamage
{

    void Start()
    {
        playerCamera = Camera.main;
        currentState.OnEnter(this, null);
    }

    void Update()
    {
        Vector3 move = moveDirection;
        move.y = 0;
        Animator.SetFloat("move", Mathf.Clamp01(move.magnitude));
        currentState.OnUpdate(this);
        jumpIntervalCount += Time.deltaTime;
    }

    private void AnimetionEvent(AnimationEvent _num)
    {
        int i = _num.intParameter;
        currentState.OnAnimetionFunction(this, i);
    }
    private void AnimetionEnd(AnimationEvent _num)
    {
        int i = _num.intParameter;
        currentState.OnAnimetionEnd(this, i);
    }
    private void AnimetionStart(AnimationEvent _num)
    {
        int i = _num.intParameter;
        currentState.OnAnimetionStart(this, i);
    }

    public void OnDamaged(AttackInfo _attackInfo)
    {
        Debug.Log(_attackInfo.name + "から" + _attackInfo.damage + "のダメージを受けた");
    }


    /// <summary>
    /// ステートの変更
    /// </summary>
    /// <param name="nextState"></param>
    private void ChangeState(PlayerStateBase nextState)
    {
        currentState.OnExit(this, nextState);
        nextState.OnEnter(this, currentState);
        currentState = nextState;
    }


    /// <summary>
    /// 移動速度とアニメーション移動速度を渡すと
    /// WASDで移動してくれる
    /// </summary>
    /// <param name="_moveSpeed"></param>
    /// <param name="_aniMoveSpeed"></param>
    private void PlayerMove(float _moveSpeed, float _aniMoveSpeed)
    {
        Vector3 inputVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (controller.isGrounded)
        {
            //移動方向をカメラ基準に直す
            moveDirection = inputVec;
            Vector3 camRot = playerCamera.transform.rotation.eulerAngles;
            moveDirection = Quaternion.Euler(0, camRot.y, 0) * moveDirection;
            //各種移動スピードの変更
            moveDirection *= _moveSpeed;
            //足滑りしないように移動アニメーションの再生速度の調整
            float aniSpeed = moveDirection.magnitude / _aniMoveSpeed;
            Animator.SetFloat("AniSpeed", aniSpeed);
        }
        //進行方向に向く
        Vector3 rotateTarget = new Vector3(moveDirection.x, 0, moveDirection.z);
        if (rotateTarget.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(rotateTarget);
            avater.transform.rotation = Quaternion.Lerp(lookRotation, avater.transform.rotation, turnSmoothing);
        }
        //重力を加える
        moveDirection.y -= gravity * Time.deltaTime;
        //移動実行
        controller.Move(moveDirection * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("attackHit"))
        //{
        //    var damagetarget = other.GetComponent<IAttackDamage>();
        //    if (damagetarget != null)
        //    {
        //        other.GetComponent<IAttackDamage>().OnDamaged(_attackPoint);
        //    }
        //    _ = Damage(other.transform.position);
        //}
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Untagged")) return;
        Debug.Log("player  "+hit.collider.gameObject.tag);
    }
    private async Task Damage(Vector3 _otherPos)
    {
        ChangeState(playerKnockBackState);
        moveDirection = transform.position - _otherPos;
        moveDirection = moveDirection.normalized * 10f;
        await Task.Delay(TimeSpan.FromSeconds(0.5f));
        moveDirection = Vector3.zero;
    }
}
