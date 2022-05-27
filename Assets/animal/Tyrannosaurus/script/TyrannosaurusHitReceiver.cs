using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusHitReceiver : HitReceiver
{
    private void Start()
    {
        foreach (var element in Hits)
        {
            element.MaskTag = "Player";
        }

    }
    public override void OnHit(GameObject _hitObject, Hit _hit)
    {
        AttackInfo info = new AttackInfo();
        //Hit‚©‚çŽæ“¾‚·‚é
        info.PartType = _hit.Part;
        info.CllisionPos = _hit.CollisionPos;
        var status = _hitObject.transform.root.gameObject.GetComponent<Status>();
        info.Attack = status.Attack;
        info.HitReaction = HitReaction.nonReaction;
        status.OnDamaged(info);
    }

}
