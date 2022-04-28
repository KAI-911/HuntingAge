using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class tyrannosaurus : MonoBehaviour
{
    public GameObject avater = null;            //アバター
    public Animator Animator;                   //アニメーションの制御用
    public int angularSpeed = 160;         //回転の最高速度（単位： 度／秒）
    public float walkSpeed = 5.0F;              //歩き速度
    //アニメーションで進む早さ
    public float aniWalkSpeed;
    private NavMeshAgent agent;                 //ナビメッシュ
    private GameObject target;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target =GameObject.FindWithTag("Player");
        agent.speed = walkSpeed;
        agent.acceleration = walkSpeed;
        agent.angularSpeed = angularSpeed;
    }

    private void Update()
    {
        //足滑りしないように移動アニメーションの再生速度の調整
        Animator.SetFloat("AniSpeed", agent.speed / aniWalkSpeed);
        agent.destination = target.transform.position;
    }

}
