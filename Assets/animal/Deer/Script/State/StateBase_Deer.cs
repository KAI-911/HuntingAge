using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase_Deer
{
    public virtual void OnEnter(Deer owner, StateBase_Deer prevState)
    {

    }
    public virtual void OnExit(Deer owner, StateBase_Deer nextState)
    {

    }
    public virtual void OnUpdate(Deer owner)
    {

    }
    public virtual void OnFixedUpdate(Deer owner)
    {

    }
    public virtual void OnAnimationEvent(Deer owner, AnimationEvent animationEvent)
    {

    }
    public virtual void OnCollisionStay(Deer owner, Collision collision)
    {

    }
}
