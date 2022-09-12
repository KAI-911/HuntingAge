using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Rhino : StateBase_Rhino
{
    float time;

    public override void OnEnter(Rhino owner, StateBase_Rhino prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Death);
        owner.Animator.SetTrigger("Down");
        owner.Status.InvincibleFlg = true;
        time = 0;
        owner.Death();

    }
    public override void OnExit(Rhino owner, StateBase_Rhino nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Rhino owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;

        time += Time.deltaTime;
        float rate = time / owner.DissoveTime;

        //ñ{ëÃÇè¡ÇµÇƒÇ¢Ç≠
        var myMat = owner.SkinnedMeshRenderer.material;
        if (myMat.HasProperty("_Threshold"))
        {
            myMat.SetFloat("_Threshold", owner.DissoveCurve.Evaluate(rate));
            if (owner.DissoveCurve.Evaluate(rate) > 0)
            {
                owner.CollectionScript.gameObject.SetActive(false);
                owner.SkinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }

        if (rate >= 1)
        {
            owner.Delete();
        }
    }
    public override void OnFixedUpdate(Rhino owner)
    {

    }
    public override void OnAnimationEvent(Rhino owner, AnimationEvent animationEvent)
    {
        if (animationEvent.stringParameter == "End")
        {
            //ëÃÇÃìñÇΩÇËîªíËÇè¡Ç∑
            var colls = owner.gameObject.GetComponentsInChildren<Collider>();
            foreach (var item in colls)
            {
                item.enabled = false;
            }
            owner.CollectionScript.gameObject.SetActive(true);

        }

    }
    public override void OnCollisionStay(Rhino owner, Collision collision)
    {

    }
}
