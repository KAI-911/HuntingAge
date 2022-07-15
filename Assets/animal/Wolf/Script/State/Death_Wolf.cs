using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Wolf : SatetBase_Wolf
{
    float time;

    public override void OnEnter(Wolf owner, SatetBase_Wolf prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Death);
        owner.Animator.SetTrigger("Down");
        owner.Status.InvincibleFlg = true;
        time = 0;
        owner.Death();

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

        //ñ{ëÃÇè¡ÇµÇƒÇ¢Ç≠
        var myMat = owner.SkinnedMeshRenderer.material;
        if (myMat.HasProperty("_Dissolve"))
        {
            myMat.SetFloat("_Dissolve", owner.DissoveCurve.Evaluate(rate));
            if (owner.DissoveCurve.Evaluate(rate) > 0) owner.CollectionScript.gameObject.SetActive(false);

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
            //ëÃÇÃìñÇΩÇËîªíËÇè¡Ç∑
            var colls = owner.gameObject.GetComponentsInChildren<Collider>();
            foreach (var item in colls)
            {
                item.enabled = false;
            }
            owner.SkinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            owner.CollectionScript.gameObject.SetActive(true);

        }

    }
    public override void OnCollisionStay(Wolf owner, Collision collision)
    {

    }
}
