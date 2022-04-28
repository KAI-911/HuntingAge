using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public partial class tyrannosaurus : MonoBehaviour
{
    public GameObject avater = null;            //�A�o�^�[
    public Animator Animator;                   //�A�j���[�V�����̐���p
    public int angularSpeed = 160;         //��]�̍ō����x�i�P�ʁF �x�^�b�j
    public float walkSpeed = 5.0F;              //�������x
    //�A�j���[�V�����Ői�ޑ���
    public float aniWalkSpeed;
    private NavMeshAgent agent;                 //�i�r���b�V��
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
        //�����肵�Ȃ��悤�Ɉړ��A�j���[�V�����̍Đ����x�̒���
        Animator.SetFloat("AniSpeed", agent.speed / aniWalkSpeed);
        agent.destination = target.transform.position;
    }

}
