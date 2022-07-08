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
    //�_���[�W��^����ꂽ��true��Ԃ�
    public bool OnDamaged(AttackInfo _attackInfo)
    {
        //���G����
        if (_invincibleFlg) return false;

        //���̕ۑ�
        _Hitparameter = _attackInfo;

        //���ۂɎ󂯂�_���[�W�̌v�Z
        var damage = _attackInfo.Attack - _defense;
        //�_���[�W�̒���
        if (damage <= 0) damage = 1;

        //��e���A�N�V�����̐ݒ�
        _hitReaction = _attackInfo.HitReaction;

        //�_���[�W���󂯂�
        _hp -= damage;

        //���ʂ��Ƃɋ��݌v�Z
        var partReceiver = this.gameObject.transform.root.GetComponent<PartCheckerReceiver>();
        Debug.Log(_Hitparameter.HitPart);

        //���ʂɃ_���[�W��^����
        if (partReceiver.PartEnduranceDamage(_Hitparameter.HitPart, damage)) 
        {
            _downFlg = true;
        }
        if (_hp < 0) _hp = 0;
        return true;
    }

}