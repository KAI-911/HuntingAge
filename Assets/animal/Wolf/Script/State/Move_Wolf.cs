using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Wolf : SatetBase_Wolf
{
    public override void OnEnter(Wolf owner, SatetBase_Wolf prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);

        owner.NavMeshAgent.speed = 5;
    }
    public override void OnExit(Wolf owner, SatetBase_Wolf nextState)
    {

    }
    public override void OnUpdate(Wolf owner)
    {
        owner.NavMeshAgent.destination = owner.Target.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));
        var list = owner.TargetChecker();

        //searchà»äOÇ≈çUåÇîªíËÇ™Ç†ÇÍÇŒçUåÇÇ…à⁄çs
        foreach (var item in list)
        {
            if (item == TargetCheckerType.Search) continue;
            owner.ChangeState<Attack_Wolf>();
            return;
        }
        if (!owner.DiscoverFlg)
        {
            owner.ChangeState<Idle_Wolf>();
        }

        if (owner.Search() == false)
        {
            owner.ChangeState<Wandering_Wolf>();
            return;
        }
    }
    public override void OnFixedUpdate(Wolf owner)
    {

    }
    public override void OnAnimationEvent(Wolf owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Wolf owner, Collision collision)
    {

    }
}
