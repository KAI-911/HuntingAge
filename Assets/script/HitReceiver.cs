using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
public class HitReceiver : MonoBehaviour
{

    [SerializeField] List<AttackHit> _hitList = new List<AttackHit>();
    public List<AttackHit> Hits { get => _hitList; set => _hitList = value; }

    [SerializeField] private string _mask;
    public string Mask { get => _mask; set => _mask = value; }

    //攻撃ステートになったらそこで切り替える
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
            if (_part == element.AttackPart) element.gameObject.SetActive(!element.gameObject.activeSelf);
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
        AttackInfo info = new AttackInfo();
        //Hitから取得する
        info.PartType = _hit.AttackPart;
        info.CllisionPos = _hit.CollisionPos;
        var hitstatus = _hitObject.transform.root.gameObject.GetComponent<Status>();
        var mystatus = transform.root.gameObject.GetComponent<Status>();
        info.HitPart = _hitObject.GetComponent<PartChecker>();
        Debug.Log(info.PartType);
        if (hitstatus == null || mystatus == null) return;
        info.Attack = mystatus.Attack;
        info.HitReaction = _hitReaction;
        var success = hitstatus.OnDamaged(info);
        //ダメージを与えられたらヒットエフェクトを出す
        if (success)
        {
            Instantiate(particleObject, info.CllisionPos, Quaternion.identity);
        }
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
    axe,
    spear
}
public enum HitReaction
{
    nonReaction,    //反応しない
    lowReaction,    //軽くのけぞる
    middleReaction, //しりもちをつく
    highReaction    //吹っ飛ぶ
}

