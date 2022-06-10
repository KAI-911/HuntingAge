using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase_Trex
{
    public virtual void OnEnter(Trex owner, StateBase_Trex prevState)
    {

    }
    public virtual void OnExit(Trex owner, StateBase_Trex nextState)
    {

    }
    public virtual void OnUpdate(Trex owner)
    {

    }
    public virtual void OnFixedUpdate(Trex owner)
    {

    }
    public virtual void OnAnimationEvent(Trex owner, AnimationEvent animationEvent)
    {

    }
    public virtual void OnCollisionStay(Trex owner, Collision collision)
    {

    }
}
