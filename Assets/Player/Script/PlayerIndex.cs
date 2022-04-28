using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    private Camera playerCamera = null;         //プレイヤーを見るカメラ
    public GameObject avater = null;            //アバター
    public Transform AimPos;                    //注視点
    public Animator Animator;                   //アニメーションの制御用
    public CharacterController controller;      //プレイヤーの制御
    public float jumpSpeed = 8.0F;              //ジャンプ力
    public float dodgeSpeed = 8.0F;             //回避するときの移動速度
    public float gravity = 20.0F;               //重力の大きさ
    public float turnSmoothing = 0.98F;         //振り向きの早さ
    public float jumpInterval = 1.0f;           //連続でジャンプできる間隔
    public float sneakSpeed = 1.0F;             //しゃがみ歩き速度
    public float walkSpeed = 3.0F;              //歩き速度
    public float dushSpeed = 6.0F;              //走り速度
    //アニメーションで進む早さ
    public float AniSneakSpeed;
    public float AniWalkSpeed;
    public float AniDushSpeed;

    private float jumpIntervalCount = 0.0f;
    private Vector3 moveDirection = Vector3.zero;//移動方向の保存
    private bool sneakFlg = false;//しゃがんでいる==1、それ以外==0
    public enum AniState
    {
        Idle,
        Sneak,
        SneakingWalk,
        Walk,
        Dush,
        Dodge,
        Jump
    };
    private static readonly PlayerIdleState playerIdleState = new PlayerIdleState();//待機
    private static readonly PlayerSneakState playerSneakState = new PlayerSneakState();//しゃがみ待機
    private static readonly PlayerSneakingWalkState playerSneakingWalkState = new PlayerSneakingWalkState();//しゃがみ移動
    private static readonly PlayerWalkState playerWalkState = new PlayerWalkState();//歩き
    private static readonly PlayerDushState playerDushState = new PlayerDushState();//走り
    private static readonly PlayerDodgeState playerDodgeState = new PlayerDodgeState();//回避
    private static readonly PlayerJumpState playerJumpState = new PlayerJumpState();//ジャンプ

    private PlayerStateBase currentState = playerIdleState;


}
