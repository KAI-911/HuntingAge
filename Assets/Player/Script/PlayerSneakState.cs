using UnityEngine;
public partial class Player
{
    public class PlayerSneakState : PlayerStateBase
    {
        public override void OnEnter(Player owner, PlayerStateBase prevState)
        {
            owner.Animator.SetInteger("AniState", (int)AniState.Sneak);
            owner.sneakFlg = true;
        }
        public override void OnUpdate(Player owner)
        {
            owner.PlayerMove(1, 1);
            //������x���Ⴊ�݃{�^�����W�����v�{�^���������ꂽ��ҋ@��Ԃ�
            if (Input.GetButtonDown("Dodge") || Input.GetButtonDown("Jump"))
            {
                owner.ChangeState(playerIdleState);
                return;
            }

            //WASD�������ꂽ�炵�Ⴊ�ݕ�����Ԃ�
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                owner.ChangeState(playerSneakingWalkState);
                return;
            }
        }
        public override void OnExit(Player owner, PlayerStateBase prevState)
        {
            owner.sneakFlg = false;
        }
    }
}

