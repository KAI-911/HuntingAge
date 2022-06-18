using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase_Dodo
{
    public virtual void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {

    }
    public virtual void OnExit(Dodo owner, StateBase_Dodo nextState)
    {

    }
    public virtual void OnUpdate(Dodo owner)
    {

    }
    public virtual void OnFixedUpdate(Dodo owner)
    {

    }
    public virtual void OnAnimationEvent(Dodo owner, AnimationEvent animationEvent)
    {

    }
    public virtual void OnCollisionStay(Dodo owner, Collision collision)
    {

    }
}
