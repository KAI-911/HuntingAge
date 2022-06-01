using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Status : MonoBehaviour, IAttackDamage
{

    [SerializeField] private int _hp;
    public int HP { get => _hp; set => _hp = value; }

    [SerializeField] private int _sp;
    public int SP { get => _sp; set => _sp = value; }

    [SerializeField] private int _attack;
    public int Attack { get => _attack; set => _attack = value; }

    [SerializeField] private int _defense;
    public int Defense { get => _defense; set => _defense = value; }

    //無敵
    [SerializeField] private bool _invincibleFlg;
    public bool InvincibleFlg { get => _invincibleFlg; set => _invincibleFlg = value; }
    
    //転倒
    [SerializeField] private bool _downFlg;
    public bool DownFlg { get => _downFlg; set => _downFlg = value; }
    
    //攻撃を受けた時の反応
    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; }

    //攻撃の情報
    private AttackInfo _Hitarameter;
    public AttackInfo HitParameter { get => _Hitarameter; }

    private void Start()
    {
        _invincibleFlg = false;
        _downFlg = false;
    }
    public void HitReactionReset()
    {
        _hitReaction = HitReaction.nonReaction;
    }
    //ダメージを与えられたらtrueを返す
    public bool OnDamaged(AttackInfo _attackInfo)
    {
        if (_invincibleFlg) return false;
        _Hitarameter = _attackInfo;
        var damage = _attackInfo.Attack - _defense;
        _hitReaction = _attackInfo.HitReaction;
        if (damage <= 0) damage = 1;
        _hp -= damage;
        if(_attackInfo.HitPart.PartEnduranceDamage(damage))
        {
            _downFlg = true;
        }
        if (_hp < 0) _hp = 0;
        return true;
    }
}