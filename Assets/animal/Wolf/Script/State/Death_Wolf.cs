using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Wolf : SatetBase_Wolf
{
    float time;
    Vector4 color;

    public override void OnEnter(Wolf owner, SatetBase_Wolf prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Death);
        owner.Animator.SetTrigger("Down");
        owner.Status.InvincibleFlg = true;
        time = 0;
        if (owner.ShadowRenderer.material.HasProperty("_ShadowColor"))
        {
            color = owner.ShadowRenderer.material.GetColor("_ShadowColor");
        }
        if (owner.QuestManager != null) owner.QuestManager.AddKillEnemy(owner.EnemyID);


    }
    public override void OnExit(Wolf owner, SatetBase_Wolf nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Wolf owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;

        time += Time.deltaTime;
        float rate = time / owner.DissoveTime;

        //本体を消していく
        var myMat = owner.SkinnedMeshRenderer.material;
        if (myMat.HasProperty("_Threshold"))
        {
            myMat.SetFloat("_Threshold", owner.DissoveCurve.Evaluate(rate));
        }

        //影を消していく
        var shadowMat = owner.ShadowRenderer.material;
        if (shadowMat.HasProperty("_ShadowColor"))
        {
            var setColor = color;
            setColor.w = color.w * owner.ShadowCurve.Evaluate(rate);
            shadowMat.SetColor("_ShadowColor", setColor);
        }

        if (rate >= 1)
        {
            owner.Delete();
        }
    }
    public override void OnFixedUpdate(Wolf owner)
    {

    }
    public override void OnAnimationEvent(Wolf owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "End")
        {

            //体の当たり判定を消す
            var colls = owner.gameObject.GetComponentsInChildren<Collider>();
            foreach (var item in colls)
            {
                item.enabled = false;
            }
        }

    }
    public override void OnCollisionStay(Wolf owner, Collision collision)
    {

    }
}
