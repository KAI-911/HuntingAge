using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : PlayerStateBase
{
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Death);
        owner.Animator.SetTrigger("Change");
        GameManager.Instance.Quest.DeathCount++;
    }
    public override void OnFixedUpdate(Player owner)
    {
        Debug.Log("DeathState");
    }
}
