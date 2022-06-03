using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusDeathState : TyrannosaurusState
{
    float time;
    Vector4 color;
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {

        owner.Animator.SetInteger("AniState", (int)TyrannosaurusAnimationState.Death);
        owner.Animator.SetTrigger("Down");
        owner.Status.InvincibleFlg = true;
        time = 0;
        if (owner.ShadowRenderer.material.HasProperty("_ShadowColor"))
        {
            color = owner.ShadowRenderer.material.GetColor("_ShadowColor");
        }
    }
    public override void OnExit(Tyrannosaurus owner, TyrannosaurusState nextState)
    {

    }
    public override void OnUpdate(Tyrannosaurus owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;

        time += Time.deltaTime;
        float rate = time / owner.DissoveTime;

        //ñ{ëÃÇè¡ÇµÇƒÇ¢Ç≠
        var myMat = owner.SkinnedMeshRenderer.material;
        if (myMat.HasProperty("_Threshold"))
        {
            myMat.SetFloat("_Threshold", owner.DissoveCurve.Evaluate(rate));
        }

        //âeÇè¡ÇµÇƒÇ¢Ç≠
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
    public override void OnFixedUpdate(Tyrannosaurus owner)
    {

    }
    public override void OnAnimationEvent(Tyrannosaurus owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "End")
        {
            //owner.SkinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
           
            //ëÃÇÃìñÇΩÇËîªíËÇè¡Ç∑
            var colls = owner.gameObject.GetComponentsInChildren<Collider>();
            foreach (var item in colls)
            {
                item.enabled = false;
            }
        }
    }
    public override void OnCollisionStay(Tyrannosaurus owner, Collision collision)
    {

    }
}
