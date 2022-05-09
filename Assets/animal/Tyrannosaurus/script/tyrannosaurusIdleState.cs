using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusIdleState : TyrannosaurusStateBase
{
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)Tyrannosaurus.AniState.Idle);
    }

    public override void OnUpdate(Tyrannosaurus owner)
    {
        Vector3 diff = owner.target.transform.position - owner.rayStartPos.transform.position;
        float dis = diff.magnitude;
        if (owner.searchRange > dis)
        {
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
            if (owner.searchAngle / 2.0f > Mathf.Abs(signedAngle))
            {
                //間に障害物があるか確認
                Vector3 dir = diff.normalized;
                RaycastHit[] raycastHits = new RaycastHit[50];
                Ray ray = new Ray
                {
                    origin = owner.rayStartPos.transform.position,
                    direction = dir
                };
                int hitCount = Physics.RaycastNonAlloc(ray, raycastHits, dis, 3);
                //プレイヤーはキャラクターコントローラーを使用しいているため、
                //RaycastNonAllocはコリジョンがあるものだけ判定するのでプレイヤーは判定されない
                if (hitCount == 0)
                {
                    owner.ChangeState<TyrannosaurusMoveState>();
                }
            }
        }
    }
}
