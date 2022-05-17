using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HitReceiver : MonoBehaviour
{

    private AttackInfo attackInfo;   //çUåÇèÓïÒ
    public Hit[] hits;

    public enum Part
    {
        notSet,
        head,
        body,
        rightLeg,
        leftLeg,
        tail,
        searchTrigger,
        bitingAttackTrigger,
        tailAttackTrigger
    }

    public void SetAttackFlg(bool _attackFlg, Part _part)
    {
        foreach (var element in hits)
        {
            if (element.GetPart() != _part) continue;
            element.SetAttackFlg(_attackFlg);

        }

    }
    public virtual void OnHit(Part _part, AttackInfo _attackInfo, GameObject _gameObject)
    {
        Debug.Log(_gameObject.tag + "Ç…" + _attackInfo.name + "Ç™ìñÇΩÇ¡ÇΩ");
        var player = _gameObject.GetComponent<Player>();
        if (player != null)
        {
            player.OnDamaged(_attackInfo);
        }
    }

    public virtual void TriggerEnter(Collider _other, AttackInfo _attackInfo)
    {
        Debug.Log("OnTriggerEnter");

    }
    public virtual void TriggerStay(Collider _other, AttackInfo _attackInfo)
    {
        Debug.Log("OnTriggerStay");

    }
    public virtual void TriggerExit(Collider _other, AttackInfo _attackInfo)
    {
        Debug.Log("OnTriggerExit");

    }

}


public interface IAttackDamage : IEventSystemHandler
{
    public void OnDamaged(AttackInfo _attackInfo);
}
public class AttackInfo
{
    public Transform transform = null;
    public int damage = 0;
    public string name = "non";
    public bool attackFlg = false;
}
