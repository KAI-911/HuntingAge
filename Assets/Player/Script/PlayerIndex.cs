using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public partial class Player
{
    [SerializeField] private Camera playerCamera = null;            //プレイヤーを見るカメラ
    [SerializeField] private CharacterController controller;        //プレイヤーの制御
    [SerializeField] private Transform AimPos;                      //注視点
    [SerializeField] private float turnSmoothing = 0.98F;           //振り向きの早さ
    public Vector3 gravity = new Vector3(0, -20.0f, 0);                 //重力の大きさ
    //ジャンプ関係ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private float jumpSpeed = 8.0F;                //ジャンプ力
    [SerializeField] private float jumpInterval = 1.0f;             //連続でジャンプできる間隔
    [SerializeField] private float jumpIntervalCount = 0.0f;        //ジャンプ間隔のカウント

    //移動速度関係ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private float sneakMaxSpeed = 1.0F;            //しゃがみ歩き速度
    [SerializeField] private float walkMaxSpeed = 3.0F;             //歩き速度
    [SerializeField] private float dushMaxSpeed = 6.0F;             //走り速度
    public float nowMaxSpeed, nowSpeed;
    [SerializeField] private float dodgeSpeed = 8.0F;               //回避するときの移動速度
    [SerializeField] private bool sneakFlg = false;                 //しゃがんでいる==1、それ以外==0
    public Vector3 moveDirection = Vector3.zero;                    //移動方向の保存

    //animation関係ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
    public Animator Animator;                      //アニメーションの制御用
    //アニメーションで進む早さ
    [SerializeField] private float AniSneakSpeed, AniWalkSpeed, AniDushSpeed;


    //体力ーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーーー
    [SerializeField] private int hitPoint = 100;
    [SerializeField] private Slider HPslider;

    public enum AniState
    {
        Locomotion,
        Dodge,
        Jump,
        Knockback
    };

    private PlayerStateBase currentState;






}
