using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase
{
    public virtual void OnEnter(Enemy owner, StateBase prevState)
    {

    }
    public virtual void OnExit(Enemy owner, StateBase nextState)
    {

    }
    public virtual void OnUpdate(Enemy owner)
    {

    }
    public virtual void OnFixedUpdate(Enemy owner)
    {

    }
    public virtual void OnAnimationEvent(Enemy owner, AnimationEvent animationEvent)
    {

    }
    public virtual void OnCollisionStay(Enemy owner, Collision collision)
    {

    }
}
