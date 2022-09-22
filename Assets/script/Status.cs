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

    //���G
    [SerializeField] private bool _invincibleFlg;
    public bool InvincibleFlg { get => _invincibleFlg; set => _invincibleFlg = value; }

    //�]�|
    [SerializeField] private bool _downFlg;
    public bool DownFlg { get => _downFlg; set => _downFlg = value; }

    //�U�����󂯂����̔���
    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; set => _hitReaction = value; }

    //�U���̏��
    private AttackInfo _Hitparameter;
    public AttackInfo HitParameter { get => _Hitparameter; set => _Hitparameter = value; }
    //�h��͂𖳎�����{��
    public float _ignoreDefense = 0;

    //�U���͂ɂ�����{��
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
    //�_���[�W��^����ꂽ��true��Ԃ�
    public bool OnDamaged(AttackInfo _attackInfo)
    {
        //���G����
        if (_invincibleFlg) return false;

        //���̕ۑ�
        _Hitparameter = _attackInfo;

        //���ۂɎ󂯂�_���[�W�̌v�Z
        int damage = (int)(_attackInfo.Attack * _attackInfo.MagnificationAttack - _defense * (1 - _attackInfo.IgnoreDefense));
        //�_���[�W�̒���
        if (damage <= 0) damage = 1;

        //��e���A�N�V�����̐ݒ�
        _hitReaction = _attackInfo.HitReaction;

        //�_���[�W���󂯂�
        _hp -= damage;
        Debug.Log("�U�����ꂽ��");
        Debug.Log("�h��@" + Defense);
        Debug.Log("�U�����鑤");
        Debug.Log("�U���@" + _attackInfo.Attack);
        Debug.Log("�h�䖳���{���@" + _ignoreDefense);
        Debug.Log("�U���{���@" + _magnificationAttack);
        Debug.Log("�_���[�W�@" + damage);

        //���ʂ��Ƃɋ��݌v�Z
        var partReceiver = this.gameObject.transform.root.GetComponent<PartCheckerReceiver>();

        //���ʂɃ_���[�W��^����
        if (partReceiver.PartEnduranceDamage(_Hitparameter.HitPart, damage))
        {
            _downFlg = true;
        }
        if (_hp < 0) _hp = 0;
        return true;
    }
    public void SetMagnificationAttack(float _magnAttack)
    {
        Debug.Log("�U���{���ݒ�" + _magnAttack);
        _magnificationAttack = _magnAttack;
    }
    public void SetIgnoreDefense(float _ignDefense)
    {
        _ignoreDefense = _ignDefense;
    }
}