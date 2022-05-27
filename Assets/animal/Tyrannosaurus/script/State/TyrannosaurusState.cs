using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusState
{
    public virtual void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {

    }
    public virtual void OnExit(Tyrannosaurus owner, TyrannosaurusState nextState)
    {

    }
    public virtual void OnUpdate(Tyrannosaurus owner)
    {

    }
    public virtual void OnFixedUpdate(Tyrannosaurus owner)
    {

    }
    public virtual void OnAnimationEvent(Tyrannosaurus owner, AnimationEvent animationEvent)
    {

    }
    public virtual void OnCollisionStay(Tyrannosaurus owner, Collision collision)
    {

    }
}
