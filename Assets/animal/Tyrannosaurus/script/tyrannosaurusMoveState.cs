using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusMoveState : TyrannosaurusStateBase
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)Tyrannosaurus.AniState.Move);
    }

    public override void OnUpdate(Tyrannosaurus owner)
    {
        //足滑りしないように移動アニメーションの再生速度の調整
        owner.Animator.SetFloat("AniSpeed", owner.agent.speed / owner.aniWalkSpeed);
        owner.agent.destination = owner.target.transform.position;
        if ((owner.target.transform.position - owner.gameObject.transform.position).magnitude < owner.agent.stoppingDistance)
        {
            Vector3 diff = owner.target.transform.position - owner.rayStartPos.transform.position;
            // 空間上のベクトル
            var from = owner.gameObject.transform.forward;
            var to = diff;
            // 平面の法線ベクトル（上向きベクトルとする）
            var planeNormal = Vector3.up;
            // 平面に投影されたベクトルを求める
            var planeFrom = Vector3.ProjectOnPlane(from, planeNormal);
            var planeTo = Vector3.ProjectOnPlane(to, planeNormal);
            // 平面に投影されたベクトル同士の符号付き角度
            // 時計回りで正、反時計回りで負
            var signedAngle = Vector3.SignedAngle(planeFrom, planeTo, planeNormal);

            owner.ChangeState<TyrannosaurusBitingAttackState>();
        }
    }
}
