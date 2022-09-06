using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Rhino : StateBase_Rhino
{
    public override void OnEnter(Rhino owner, StateBase_Rhino prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);
    }
    public override void OnExit(Rhino owner, StateBase_Rhino nextState)
    {

    }
    public override void OnUpdate(Rhino owner)
    {
        owner.NavMeshAgent.destination = owner.Target.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));
        var list = owner.TargetChecker();

        //searchà»äOÇ≈çUåÇîªíËÇ™Ç†ÇÍÇŒçUåÇÇ…à⁄çs
        foreach (var item in list)
        {
            if (item == TargetCheckerType.Search) continue;
            owner.ChangeState<Attack_Rhino>();
            return;
        }
        if (!owner.DiscoverFlg)
        {
            owner.ChangeState<Idle_Rhino>();
        }

        if (owner.Search() == false)
        {
            owner.ChangeState<Wandering_Rhino>();
            return;
        }
    }
}
