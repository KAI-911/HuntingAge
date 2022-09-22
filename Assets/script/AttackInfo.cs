using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackInfo
{
    private int _attack;
    public int Attack { get => _attack; set => _attack = value; }

    //UŒ‚‚ð‚µ‚½•”ˆÊ
    private PartType _partType;
    public PartType PartType { get => _partType; set => _partType = value; }
    //Õ“Ë’n“_
    private Vector3 _collisionPos;
    public Vector3 CllisionPos { get => _collisionPos; set => _collisionPos = value; }
    //UŒ‚‚ð‚­‚ç‚Á‚½‚Æ‚«‚ÌƒŠƒAƒNƒVƒ‡ƒ“
    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; set => _hitReaction = value; }

    //UŒ‚‚ðŽó‚¯‚½•”ˆÊ
    private PartType _hitPart;
    public PartType HitPart { get => _hitPart; set => _hitPart = value; }

    //–hŒä—Í‚ð–³Ž‹‚·‚é”{—¦
    private float _ignoreDefense = 0;
    public float IgnoreDefense { get => _ignoreDefense; set => _ignoreDefense = value; }

    //UŒ‚—Í‚É‚©‚©‚é”{—¦
    private float _magnificationAttack = 1;
    public float MagnificationAttack { get => _magnificationAttack; set => _magnificationAttack = value; }


}
