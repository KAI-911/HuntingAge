using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public partial class Tyrannosaurus
{
    [SerializeField] private GameObject avater;        //アバター
    public Animator Animator;               //アニメーションの制御用
    [SerializeField] private TyrannosaurusStateBase currentState;
    public GameObject target;               //追跡対象
    public NavMeshAgent agent;              //ナビメッシュ
    public HitReceiver receiver;            //接触判定の受け取り
    [SerializeField] private int angularSpeed = 160;          //回転の最高速度（単位： 度／秒）
    public float walkSpeed = 5.0F;          //移動速度
    public float aniWalkSpeed;              //アニメーションで進む早さ
    public GameObject rayStartPos;          //レイ発射位置
    public float searchRange;               //索敵範囲
    public float searchAngle;               //視野角
    public enum AniState
    {
        Idle,
        Move,
        BitingAttack,
        TailAttack,
        Roar
    }

}