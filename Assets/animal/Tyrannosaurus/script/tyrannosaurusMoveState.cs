using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusMoveState : TyrannosaurusStateBase
{
    Hit bitingHit = null;
    Hit tailHit = null;
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)Tyrannosaurus.AniState.Move);
        foreach (var element in owner.receiver.hits)
        {
            if (element.GetPart() == HitReceiver.Part.bitingAttackTrigger)
            {
                bitingHit = element;
            }
            else if (element.GetPart() == HitReceiver.Part.tailAttackTrigger)
            {
                tailHit = element;
            }
        }

    }

    public override void OnUpdate(Tyrannosaurus owner)
    {
        //足滑りしないように移動アニメーションの再生速度の調整
        owner.Animator.SetFloat("AniSpeed", owner.agent.speed / owner.aniWalkSpeed);
        owner.agent.destination = owner.target.transform.position;

        //Random.InitState(System.DateTime.Now.Millisecond);
        int ran = Random.Range(0, 1000);
        if (bitingHit != null && bitingHit.GetTrigger() && ran < 5)
        {
            owner.ChangeState<TyrannosaurusBitingAttackState>();
        }
        if (tailHit != null && tailHit.GetTrigger() && ran < 5)
        {
            owner.ChangeState<TyrannosaurusTailAttackState>();
        }
    }
}
