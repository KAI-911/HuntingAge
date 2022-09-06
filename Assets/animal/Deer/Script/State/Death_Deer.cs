using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Deer : StateBase_Deer
{

    float time;

    public override void OnEnter(Deer owner, StateBase_Deer prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Death);
        owner.Animator.SetTrigger("Down");
        owner.Status.InvincibleFlg = true;
        time = 0;
        owner.Death();

    }
    public override void OnExit(Deer owner, StateBase_Deer nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Deer owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;

        time += Time.deltaTime;
        float rate = time / owner.DissoveTime;

        //ñ{ëÃÇè¡ÇµÇƒÇ¢Ç≠
        var myMat = owner.SkinnedMeshRenderer.material;
        if (myMat.HasProperty("_Threshold"))
        {
            myMat.SetFloat("_Threshold", owner.DissoveCurve.Evaluate(rate));
            if (owner.DissoveCurve.Evaluate(rate) > 0) owner.CollectionScript.gameObject.SetActive(false);

        }

        if (rate >= 1)
        {
            owner.Delete();
        }
    }
    public override void OnFixedUpdate(Deer owner)
    {

    }
    public override void OnAnimationEvent(Deer owner, AnimationEvent animationEvent)
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
    public override void OnCollisionStay(Deer owner, Collision collision)
    {

    }
}
