using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public partial class Tyrannosaurus
{
    [SerializeField] private GameObject avater;        //�A�o�^�[
    public Animator Animator;               //�A�j���[�V�����̐���p
    [SerializeField] private TyrannosaurusStateBase currentState;
    public GameObject target;               //�ǐՑΏ�
    public NavMeshAgent agent;              //�i�r���b�V��
    public HitReceiver receiver;            //�ڐG����̎󂯎��
    [SerializeField] private int angularSpeed = 160;          //��]�̍ō����x�i�P�ʁF �x�^�b�j
    public float walkSpeed = 5.0F;          //�ړ����x
    public float aniWalkSpeed;              //�A�j���[�V�����Ői�ޑ���
    public GameObject rayStartPos;          //���C���ˈʒu
    public float searchRange;               //���G�͈�
    public float searchAngle;               //����p
    public enum AniState
    {
        Idle,
        Move,
        BitingAttack,
        TailAttack,
        Roar
    }

}