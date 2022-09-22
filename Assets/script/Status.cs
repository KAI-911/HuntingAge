using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Status : MonoBehaviour, IAttackDamage
{
    [SerializeField] private int _maxHp;
    [SerializeField] private int _hp;
    public int HP { get => _hp; set => _hp = value; }
    public int MaxHP { get => _maxHp; set => _maxHp = value; }

    [SerializeField] private int _maxSp;
    [SerializeField] private int _sp;
    public int SP { get => _sp; set => _sp = value; }
    public int MaxSP { get => _maxSp; set => _maxSp = value; }

    [SerializeField] private int _attack;
    public int Attack { get => _attack; set => _attack = value; }

    [SerializeField] private int _defense;
    public int Defense { get => _defense; set => _defense = value; }

    //–³“G
    [SerializeField] private bool _invincibleFlg;
    public bool InvincibleFlg { get => _invincibleFlg; set => _invincibleFlg = value; }

    //“]“|
    [SerializeField] private bool _downFlg;
    public bool DownFlg { get => _downFlg; set => _downFlg = value; }

    //UŒ‚‚ğó‚¯‚½‚Ì”½‰
    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; set => _hitReaction = value; }

    //UŒ‚‚Ìî•ñ
    private AttackInfo _Hitparameter;
    public AttackInfo HitParameter { get => _Hitparameter; set => _Hitparameter = value; }
    //–hŒä—Í‚ğ–³‹‚·‚é”{—¦
    public float _ignoreDefense = 0;

    //UŒ‚—Í‚É‚©‚©‚é”{—¦
    public float _magnificationAttack = 1;

    private void Start()
    {
        _invincibleFlg = false;
        _downFlg = false;
        _Hitparameter = new AttackInfo();
        _ignoreDefense = 1;
        _magnificationAttack = 1;

    }
    public void HitReactionReset()
    {
        _hitReaction = HitReaction.nonReaction;
    }
    //ƒ_ƒ[ƒW‚ğ—^‚¦‚ç‚ê‚½‚çtrue‚ğ•Ô‚·
    public bool OnDamaged(AttackInfo _attackInfo)
    {
        //–³“Gˆ—
        if (_invincibleFlg) return false;

        //î•ñ‚Ì•Û‘¶
        _Hitparameter = _attackInfo;

        //ÀÛ‚Éó‚¯‚éƒ_ƒ[ƒW‚ÌŒvZ
        int damage = (int)(_attackInfo.Attack * _attackInfo.MagnificationAttack - _defense * (1 - _attackInfo.IgnoreDefense));
        //ƒ_ƒ[ƒW‚Ì’²®
        if (damage <= 0) damage = 1;

        //”í’eƒŠƒAƒNƒVƒ‡ƒ“‚Ìİ’è
        _hitReaction = _attackInfo.HitReaction;

        //ƒ_ƒ[ƒW‚ğó‚¯‚é
        _hp -= damage;
        Debug.Log("UŒ‚‚³‚ê‚½‘¤");
        Debug.Log("–hŒä@" + Defense);
        Debug.Log("UŒ‚‚·‚é‘¤");
        Debug.Log("UŒ‚@" + _attackInfo.Attack);
        Debug.Log("–hŒä–³‹”{—¦@" + _ignoreDefense);
        Debug.Log("UŒ‚”{—¦@" + _magnificationAttack);
        Debug.Log("ƒ_ƒ[ƒW@" + damage);

        //•”ˆÊ‚²‚Æ‚É‹¯‚İŒvZ
        var partReceiver = this.gameObject.transform.root.GetComponent<PartCheckerReceiver>();

        //•”ˆÊ‚Éƒ_ƒ[ƒW‚ğ—^‚¦‚é
        if (partReceiver.PartEnduranceDamage(_Hitparameter.HitPart, damage))
        {
            _downFlg = true;
        }
        if (_hp < 0) _hp = 0;
        return true;
    }
    public void SetMagnificationAttack(float _magnAttack)
    {
        Debug.Log("UŒ‚”{—¦İ’è" + _magnAttack);
        _magnificationAttack = _magnAttack;
    }
    public void SetIgnoreDefense(float _ignDefense)
    {
        _ignoreDefense = _ignDefense;
    }
}