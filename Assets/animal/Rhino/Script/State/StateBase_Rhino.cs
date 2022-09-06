using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase_Rhino
{
    public virtual void OnEnter(Rhino owner, StateBase_Rhino prevState)
    {

    }
    public virtual void OnExit(Rhino owner, StateBase_Rhino nextState)
    {

    }
    public virtual void OnUpdate(Rhino owner)
    {

    }
    public virtual void OnFixedUpdate(Rhino owner)
    {

    }
    public virtual void OnAnimationEvent(Rhino owner, AnimationEvent animationEvent)
    {

    }
    public virtual void OnCollisionStay(Rhino owner, Collision collision)
    {

    }
}
