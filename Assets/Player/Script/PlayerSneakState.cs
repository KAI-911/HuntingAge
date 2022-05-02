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
            //もう一度しゃがみボタンかジャンプボタンが押されたら待機状態へ
            if (Input.GetButtonDown("Dodge") || Input.GetButtonDown("Jump"))
            {
                owner.ChangeState(playerIdleState);
                return;
            }

            //WASDが押されたらしゃがみ歩き状態へ
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

