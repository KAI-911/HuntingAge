using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfState
{
    public virtual void OnEnter(Enemy owner, WolfState prevState)
    {

    }
    public virtual void OnExit(Enemy owner, WolfState nextState)
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
