using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour
{
    private static readonly PlayerIdleState playerIdleState = new PlayerIdleState();//待機
    private static readonly PlayerSneakState playerSneakState = new PlayerSneakState();//しゃがみ待機
    private static readonly PlayerSneakingWalkState playerSneakingWalkState = new PlayerSneakingWalkState();//しゃがみ移動
    private static readonly PlayerWalkState playerWalkState = new PlayerWalkState();//歩き
    private static readonly PlayerDushState playerDushState = new PlayerDushState();//走り
    private static readonly PlayerDodgeState playerDodgeState = new PlayerDodgeState();//回避
    private static readonly PlayerJumpState playerJumpState = new PlayerJumpState();//ジャンプ

    private  PlayerStateBase currentState = playerIdleState;

    void Start()
    {
        playerCamera = Camera.main;
        currentState.OnEnter(this, null);
    }

    void Update()
    {
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
    private void PlayerMove(float _moveSpeed,float _aniMoveSpeed)
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
        //重力を加えて移動実行
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

}
