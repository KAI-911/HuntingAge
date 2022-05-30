using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMiddleHitAction : StateMachineBehaviour
{
    [SerializeField] float power;
    [SerializeField] float minusPower;

    float keepPower;
    Player _player;
    Vector3 dir;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        keepPower = power;
        _player = animator.gameObject.GetComponent<Player>();
        var hitPos = _player.GetComponent<Status>().HitParameter.CllisionPos;
        dir = _player.transform.position - hitPos;
        dir.y = 0;
        dir = dir.normalized;
        dir *= power;
        var rg = _player.GetComponent<Rigidbody>();
        rg.AddForce(dir, ForceMode.Impulse);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime < 0.5f)
        {

            var rg = _player.GetComponent<Rigidbody>();
            rg.AddForce(dir, ForceMode.Impulse);
            power *= minusPower;
            dir = dir.normalized;
            dir *= power;

        }

    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        power = keepPower;
    }
}
