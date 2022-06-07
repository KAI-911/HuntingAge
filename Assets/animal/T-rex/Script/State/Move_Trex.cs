using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_Trex : StateBase
{
    public override void OnEnter(Enemy owner, StateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Move);
    }
    public override void OnExit(Enemy owner, StateBase nextState)
    {

    }
    public override void OnUpdate(Enemy owner)
    {
        Debug.Log("move");
        owner.NavMeshAgent.destination = owner.Target.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));
        var list = owner.TargetChecker();

        //searchà»äOÇ≈çUåÇîªíËÇ™Ç†ÇÍÇŒçUåÇÇ…à⁄çs
        foreach (var item in list)
        {
            if (item == TargetCheckerType.Search) continue;
            owner.ChangeState<Attack_Trex>();
            return;
        }
        


    }
    public override void OnFixedUpdate(Enemy owner)
    {

    }
    public override void OnAnimationEvent(Enemy owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Enemy owner, Collision collision)
    {

    }
}
