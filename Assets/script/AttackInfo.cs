using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInfo
{
    private int _attack;
    public int Attack { get => _attack; set => _attack = value; }

    //�U������������
    private PartType _partType;
    public PartType PartType { get => _partType; set => _partType = value; }
    //�Փ˒n�_
    private Vector3 _collisionPos;
    public Vector3 CllisionPos { get => _collisionPos; set => _collisionPos = value; }
    //�U������������Ƃ��̃��A�N�V����
    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; set => _hitReaction = value; }

    //�U�����󂯂�����
    private PartType _hitPart;
    public PartType HitPart { get => _hitPart; set => _hitPart = value; }

    //�h��͂𖳎�����{��
    private float _ignoreDefense = 0;
    public float IgnoreDefense { get => _ignoreDefense; set => _ignoreDefense = value; }

    //�U���͂ɂ�����{��
    private float _magnificationAttack = 1;
    public float MagnificationAttack { get => _magnificationAttack; set => _magnificationAttack = value; }


}
