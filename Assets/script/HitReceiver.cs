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
        //ダメージ処理をするために攻撃しているオブジェクトと攻撃されているオブジェクトからステータスを取得
        var hitstatus = _hitObject.transform.root.gameObject.GetComponent<Status>();
        var mystatus = transform.root.gameObject.GetComponent<Status>();
        if (hitstatus == null || mystatus == null) return;

        AttackInfo attackInfo = new AttackInfo();
        //ダメージ処理に必要な情報をまとめる
        attackInfo.Attack = mystatus.Attack;//攻撃力
        attackInfo.PartType = _hit.AttackPart;//攻撃している部位
        attackInfo.HitPart = _hit.HitPart;//攻撃されてる部位
        attackInfo.CllisionPos = _hit.CollisionPos;//衝突している地点
        attackInfo.HitReaction = _hitReaction;//攻撃を受けてどのぐらいリアクションするのか

        bool success = hitstatus.OnDamaged(attackInfo);

        //ダメージを与えられたらヒットエフェクトを出す
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
    nonReaction,    //反応しない
    lowReaction,    //軽くのけぞる
    middleReaction, //しりもちをつく
    highReaction    //吹っ飛ぶ
}

