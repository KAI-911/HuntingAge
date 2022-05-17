using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusRoarState : TyrannosaurusStateBase
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)Tyrannosaurus.AniState.Roar);
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

    }
}
