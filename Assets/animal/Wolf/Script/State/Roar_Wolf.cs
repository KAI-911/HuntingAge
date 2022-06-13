using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roar_Wolf : SatetBase_Wolf
{
    public override void OnEnter(Wolf owner, SatetBase_Wolf prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Roar);

    }
    public override void OnExit(Wolf owner, SatetBase_Wolf nextState)
    {

    }
    public override void OnUpdate(Wolf owner)
    {
        Debug.Log("roar");
        owner.NavMeshAgent.destination = owner.transform.position;
        owner.LookToTarget((int)(owner.RotationAngle * Time.deltaTime));

    }
    public override void OnFixedUpdate(Wolf owner)
    {

    }
    public override void OnAnimationEvent(Wolf owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "End")
        {
            Debug.Log("à–ädíEèo");
            owner.ChangeState<Move_Wolf>();
        }
    }
    public override void OnCollisionStay(Wolf owner, Collision collision)
    {

    }
}
