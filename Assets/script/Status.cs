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
    public HitReaction HitReaction { get => _hitReaction; set => _hitReaction = value; }

    //攻撃の情報
    private AttackInfo _Hitparameter;
    public AttackInfo HitParameter { get => _Hitparameter; set => _Hitparameter = value; }


    private void Start()
    {
        _invincibleFlg = false;
        _downFlg = false;
        _Hitparameter = new AttackInfo(); 

    }
    public void HitReactionReset()
    {
        _hitReaction = HitReaction.nonReaction;
    }
    //ダメージを与えられたらtrueを返す
    public bool OnDamaged(AttackInfo _attackInfo)
    {
        //無敵処理
        if (_invincibleFlg) return false;

        //情報の保存
        _Hitparameter = _attackInfo;

        //実際に受けるダメージの計算
        var damage = _attackInfo.Attack - _defense;
        //ダメージの調整
        if (damage <= 0) damage = 1;

        //被弾リアクションの設定
        _hitReaction = _attackInfo.HitReaction;

        //ダメージを受ける
        _hp -= damage;

        //部位ごとに怯み計算
        var partReceiver = this.gameObject.transform.root.GetComponent<PartCheckerReceiver>();
        Debug.Log(_Hitparameter.HitPart);

        //部位にダメージを与える
        if (partReceiver.PartEnduranceDamage(_Hitparameter.HitPart, damage)) 
        {
            _downFlg = true;
        }
        if (_hp < 0) _hp = 0;
        return true;
    }

}