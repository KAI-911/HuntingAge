using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionAction : StateMachineBehaviour
{
    Player _player;
    float speed;

    [SerializeField]
    AnimationCurve animationCurve;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = animator.gameObject.GetComponent<Player>();
        speed = animator.GetFloat("speed");
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        speed = animator.GetFloat("speed");
        animator.SetFloat("AniSpeed", animationCurve.Evaluate(speed));
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
