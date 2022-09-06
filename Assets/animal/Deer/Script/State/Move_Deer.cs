using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Deer : StateBase_Deer
{
    public override void OnEnter(Deer owner, StateBase_Deer prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);
    }
    public override void OnExit(Deer owner, StateBase_Deer nextState)
    {

    }
    public override void OnUpdate(Deer owner)
    {
        owner.NavMeshAgent.destination = owner.Target.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));
        var list = owner.TargetChecker();

        //searchΘOΕU»θͺ κΞUΙΪs
        foreach (var item in list)
        {
            if (item == TargetCheckerType.Search) continue;
            owner.ChangeState<Attack_Deer>();
            return;
        }
        if (!owner.DiscoverFlg)
        {
            owner.ChangeState<Idle_Deer>();
        }

        if (owner.Search() == false)
        {
            owner.ChangeState<Wandering_Deer>();
            return;
        }

    }
    public override void OnFixedUpdate(Deer owner)
    {

    }
    public override void OnAnimationEvent(Deer owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Deer owner, Collision collision)
    {

    }
}
