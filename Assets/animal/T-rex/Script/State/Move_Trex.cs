using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Trex : StateBase_Trex
{
    public override void OnEnter(Trex owner, StateBase_Trex prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);
    }
    public override void OnExit(Trex owner, StateBase_Trex nextState)
    {

    }
    public override void OnUpdate(Trex owner)
    {
        owner.NavMeshAgent.destination = owner.Target.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));
        var list = owner.TargetChecker();

        //searchΘOΕU»θͺ κΞUΙΪs
        foreach (var item in list)
        {
            if (item == TargetCheckerType.Search) continue;
            owner.ChangeState<Attack_Trex>();
            return;
        }
        if(!owner.DiscoverFlg)
        {
            owner.ChangeState<Idle_Trex>();
        }

    }
    public override void OnFixedUpdate(Trex owner)
    {

    }
    public override void OnAnimationEvent(Trex owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Trex owner, Collision collision)
    {

    }
}
