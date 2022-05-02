using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tyrannosaurusBitingAttackState : tyrannosaurusStateBase
{
    public override void OnEnter(tyrannosaurus owner, tyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)tyrannosaurus.AniState.BitingAttack);
    }

    public override void OnUpdate(tyrannosaurus owner)
    {
        owner.agent.destination = owner.gameObject.transform.position;
    }
    public override void OnAnimetionEnd(tyrannosaurus owner, int _num)
    {
        owner.ChangeState<tyrannosaurusMoveState>();
    }
    public override void OnAnimetionFunction(tyrannosaurus owner, int _num)
    {
        if (_num == 0)
        {
            owner.bitingColl.SetActive(true);
        }
        else if (_num == 1)
        {
            owner.bitingColl.SetActive(false);
        }
    }
}
