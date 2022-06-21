using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateBase
{

    public virtual void OnEnter(Player owner, PlayerStateBase prevState)
    {

    }
    public virtual void OnExit(Player owner, PlayerStateBase nextState)
    {

    }
    public virtual void OnUpdate(Player owner)
    {

    }
    public virtual void OnFixedUpdate(Player owner)
    {

    }
    public virtual void OnAnimationEvent(Player owner,AnimationEvent animationEvent)
    {

    }
    public virtual void OnCollisionStay(Player owner, Collision collision)
    {

    }
    public virtual void OnDodge(Player owner)
    {

    }    
    
    public virtual void OnJump(Player owner)
    {

    }


}
