using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
public partial class Player : MonoBehaviour
{

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
        if (other.CompareTag("attackHit"))
        {
            _ = Damage(other.transform.position);
        }
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
