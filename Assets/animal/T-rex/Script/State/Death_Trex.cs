using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Trex : StateBase
{
    float time;
    Vector4 color;

    public override void OnEnter(Enemy owner, StateBase prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Death);
        owner.Animator.SetTrigger("Down");
        owner.Status.InvincibleFlg = true;
        time = 0;
        if (owner.ShadowRenderer.material.HasProperty("_ShadowColor"))
        {
            color = owner.ShadowRenderer.material.GetColor("_ShadowColor");
        }


    }
    public override void OnExit(Enemy owner, StateBase nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Enemy owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;

        time += Time.deltaTime;
        float rate = time / owner.DissoveTime;

        //–{‘Ì‚ğÁ‚µ‚Ä‚¢‚­
        var myMat = owner.SkinnedMeshRenderer.material;
        if (myMat.HasProperty("_Threshold"))
        {
            myMat.SetFloat("_Threshold", owner.DissoveCurve.Evaluate(rate));
        }

        //‰e‚ğÁ‚µ‚Ä‚¢‚­
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
    public override void OnFixedUpdate(Enemy owner)
    {

    }
    public override void OnAnimationEvent(Enemy owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "End")
        {

            //‘Ì‚Ì“–‚½‚è”»’è‚ğÁ‚·
            var colls = owner.gameObject.GetComponentsInChildren<Collider>();
            foreach (var item in colls)
            {
                item.enabled = false;
            }
        }

    }
    public override void OnCollisionStay(Enemy owner, Collision collision)
    {

    }
}
