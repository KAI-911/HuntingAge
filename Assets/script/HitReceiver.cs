using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class HitReceiver : MonoBehaviour
{

    [SerializeField] List<Hit> _hitList = new List<Hit>();
    public List<Hit> Hits { get => _hitList; set => _hitList = value; }

    [SerializeField] private string _maskTag;
    public string MaskTag { get => _maskTag; set => _maskTag = value; }

    //�U���X�e�[�g�ɂȂ����炻���Ő؂�ւ���
    private HitReaction _hitReaction;
    public HitReaction HitReaction { get => _hitReaction; set => _hitReaction = value; }

    [SerializeField] GameObject particleObject;
    private void Start()
    {
        foreach (var element in _hitList)
        {
            element.MaskTag = _maskTag;
        }
    }

    public void ChangeAttackFlg(PartType _part)
    {
        foreach (var element in _hitList)
        {
            if (_part == element.Part) element.gameObject.SetActive(!element.gameObject.activeSelf);
        }
    }
    public void AttackFlgReset()
    {
        foreach (var element in _hitList)
        {
            element.gameObject.SetActive(false);
        }
    }

    public virtual void OnHit(GameObject _hitObject, Hit _hit)
    {
        AttackInfo info = new AttackInfo();
        //Hit����擾����
        info.PartType = _hit.Part;
        info.CllisionPos = _hit.CollisionPos;
        var hitstatus = _hitObject.transform.root.gameObject.GetComponent<Status>();
        var mystatus = transform.root.gameObject.GetComponent<Status>();

        if (hitstatus == null || mystatus == null) return;
        info.Attack = mystatus.Attack;
        info.HitReaction = _hitReaction;
        Debug.Log("�����蔻���OK");
        var success = hitstatus.OnDamaged(info);
        //�_���[�W��^����ꂽ��q�b�g�G�t�F�N�g���o��
        if (success)
        {
            Instantiate(particleObject, info.CllisionPos, Quaternion.identity);
        }
    }

    public void AddHitObject(Hit hit)
    {
        if (hit == null)
        {
            Debug.Log("NULL");
            return;
        }
        hit.MaskTag = _maskTag;
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
    axe,
    spear
}
public enum HitReaction
{
    nonReaction,    //�������Ȃ�
    lowReaction,    //�y���̂�����
    middleReaction, //�����������
    highReaction    //�������
}

