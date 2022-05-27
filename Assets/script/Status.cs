using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour, IAttackDamage
{
    
    [SerializeField]private int _hp;
    public int HP { get => _hp; set => _hp = value; }

    [SerializeField]private int _sp;
    public int SP { get => _sp; set => _sp = value; }

    [SerializeField]private int _attack;
    public int Attack { get => _attack; set => _attack = value; }

    [SerializeField]private int _defense;
    public int Defense { get => _defense; set => _defense = value; }
    
    

    private void Start()
    {
    }

    public void OnDamaged(AttackInfo _attackInfo)
    {
        var damage = _attackInfo.Attack - _defense;
        if (damage <= 0) damage = 1;
        _hp -= damage;
        if (_hp < 0) _hp = 0;
    }
}