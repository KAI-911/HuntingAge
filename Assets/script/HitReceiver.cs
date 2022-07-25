using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class HitReceiver : MonoBehaviour
{

    [SerializeField] List<AttackHit> _hitList = new List<AttackHit>();
    public List<AttackHit> Hits { get => _hitList; set => _hitList = value; }

    [SerializeField] private string _mask;
    public string Mask { get => _mask; set => _mask = value; }

    //�U���X�e�[�g�ɂȂ����炻���Ő؂�ւ���
    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; set => _hitReaction = value; }


    [SerializeField] GameObject particleObject;



    private void Start()
    {

        foreach (var element in _hitList)
        {
            element.Mask = _mask;
        }
    }

    public void ChangeAttackFlg(PartType _part)
    {
        foreach (var element in _hitList)
        {
            if (_part == element.AttackPart)
            {
                element.gameObject.SetActive(!element.gameObject.activeSelf);
            }
        }
    }
    public void AttackFlgReset()
    {
        foreach (var element in _hitList)
        {
            element.gameObject.SetActive(false);
        }
    }

    public virtual void OnHit(GameObject _hitObject, AttackHit _hit)
    {
        //�_���[�W���������邽�߂ɍU�����Ă���I�u�W�F�N�g�ƍU������Ă���I�u�W�F�N�g����X�e�[�^�X���擾
        var hitstatus = _hitObject.transform.root.gameObject.GetComponent<Status>();
        var mystatus = transform.root.gameObject.GetComponent<Status>();
        if (hitstatus == null || mystatus == null) return;

        AttackInfo attackInfo = new AttackInfo();
        //�_���[�W�����ɕK�v�ȏ����܂Ƃ߂�
        attackInfo.Attack = mystatus.Attack;//�U����
        attackInfo.PartType = _hit.AttackPart;//�U�����Ă��镔��
        attackInfo.HitPart = _hit.HitPart;//�U������Ă镔��
        attackInfo.CllisionPos = _hit.CollisionPos;//�Փ˂��Ă���n�_
        attackInfo.HitReaction = _hitReaction;//�U�����󂯂Ăǂ̂��炢���A�N�V��������̂�

        bool success = hitstatus.OnDamaged(attackInfo);

        //�_���[�W��^����ꂽ��q�b�g�G�t�F�N�g���o��
        if (success) Instantiate(particleObject, attackInfo.CllisionPos, Quaternion.identity);
    }

    public void AddHitObject(AttackHit hit)
    {
        if (hit == null)
        {
            Debug.Log("NULL");
            return;
        }
        hit.Mask = _mask;
        _hitList.Add(hit);
    }
}


public interface IAttackDamage : IEventSystemHandler
{
    public bool OnDamaged(AttackInfo _attackInfo);
}
public enum PartType
{
    notSet,
    head,
    body,
    rightLeg,
    leftLeg,
    tail,
    weapon
}
public enum HitReaction
{
    nonReaction,    //�������Ȃ�
    lowReaction,    //�y���̂�����
    middleReaction, //�����������
    highReaction    //�������
}

