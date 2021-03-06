using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInfo
{
    private int _attack;
    public int Attack { get => _attack; set => _attack = value; }

    //攻撃をした部位
    private PartType _partType;
    public PartType PartType { get => _partType; set => _partType = value; }

    private Vector3 _collisionPos;
    public Vector3 CllisionPos { get => _collisionPos; set => _collisionPos = value; }

    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; set => _hitReaction = value; }

    //攻撃を受けた部位
    private PartType _hitPart;
    public PartType HitPart { get => _hitPart; set => _hitPart = value; }

}
