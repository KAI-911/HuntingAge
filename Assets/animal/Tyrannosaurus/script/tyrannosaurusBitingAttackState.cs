using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusBitingAttackState : TyrannosaurusStateBase
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)Tyrannosaurus.AniState.BitingAttack);
    }

    public override void OnUpdate(Tyrannosaurus owner)
    {
        //í‚é~Ç∑ÇÈÇΩÇﬂ
        owner.agent.destination = owner.gameObject.transform.position;
    }
    public override void OnAnimetionEnd(Tyrannosaurus owner, int _num)
    {
        owner.ChangeState<TyrannosaurusMoveState>();
    }
    public override void OnAnimetionFunction(Tyrannosaurus owner, int _num)
    {
        //if (_num == 0)
        //{
        //    owner.bitingHit.gameObject.SetActive(true);
        //}
        //else if (_num == 1)
        //{
        //    owner.bitingHit.gameObject.SetActive(false);
        //}
    }
    
}
