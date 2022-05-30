using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDodgeAction : StateMachineBehaviour
{
    [SerializeField] float power;
    [SerializeField] float minusPower;

    float keepPower;
    private Player _player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = animator.gameObject.GetComponent<Player>();
        keepPower = power;
        _player.MoveDirection = Vector3.zero;
        _player.MoveDirection += _player.InputMoveAction.ReadValue<Vector2>().x * _player.GetCameraRight(_player.PlayerCamera);
        _player.MoveDirection += _player.InputMoveAction.ReadValue<Vector2>().y * _player.GetCameraForward(_player.PlayerCamera);
        var resetY = _player.MoveDirection;
        resetY.y = 0;
        _player.MoveDirection = resetY;
        _player.MoveDirection = _player.MoveDirection.normalized * power;
        //ˆÚ“®•ûŒü‚ÉŒü‚­
        _player.transform.LookAt(_player.transform.position + _player.MoveDirection);

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 0.6f)
        {
            power *= minusPower;
            _player.MoveDirection = _player.MoveDirection.normalized * power;
        }
    }
    

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        power = keepPower;
    }
}
