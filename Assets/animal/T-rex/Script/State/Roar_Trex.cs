using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roar_Trex : StateBase_Trex
{
    public override void OnEnter(Trex owner, StateBase_Trex prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Roar);

    }
    public override void OnExit(Trex owner, StateBase_Trex nextState)
    {

    }
    public override void OnUpdate(Trex owner)
    {
        Debug.Log("roar");
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));

    }
    public override void OnFixedUpdate(Trex owner)
    {

    }
    public override void OnAnimationEvent(Trex owner, AnimationEvent animationEvent)
    {
        if(animationEvent.stringParameter=="End")
        {
            Debug.Log("à–ädíEèo");
            owner.ChangeState<Move_Trex>();
        }
    }

    public override void OnCollisionStay(Trex owner, Collision collision)
    {

    }
}
