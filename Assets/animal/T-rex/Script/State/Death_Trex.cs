using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Trex : StateBase_Trex
{
    float time;

    public override void OnEnter(Trex owner, StateBase_Trex prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Death);
        owner.Animator.SetTrigger("Down");
        owner.Status.InvincibleFlg = true;
        time = 0;
        owner.Death();

    }
    public override void OnExit(Trex owner, StateBase_Trex nextState)
    {
        owner.Status.DownFlg = false;
    }
    public override void OnUpdate(Trex owner)
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
    public override void OnFixedUpdate(Trex owner)
    {

    }
    public override void OnAnimationEvent(Trex owner, AnimationEvent animationEvent)
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
    public override void OnCollisionStay(Trex owner, Collision collision)
    {

    }
}
