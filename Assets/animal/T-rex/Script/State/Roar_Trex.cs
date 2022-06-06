using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roar_Trex : StateBase
{
    public override void OnEnter(Enemy owner, StateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Roar);

    }
    public override void OnExit(Enemy owner, StateBase nextState)
    {

    }
    public override void OnUpdate(Enemy owner)
    {
        Debug.Log("roar");
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));

    }
    public override void OnFixedUpdate(Enemy owner)
    {

    }
    public override void OnAnimationEvent(Enemy owner, AnimationEvent animationEvent)
    {
        if(animationEvent.stringParameter=="End")
        {
            Debug.Log("à–ädíEèo");
            owner.ChangeState<Move_Trex>();
        }
    }
    public override void OnCollisionStay(Enemy owner, Collision collision)
    {

    }
}
