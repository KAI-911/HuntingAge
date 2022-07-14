using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Dodo : StateBase_Dodo
{
    float time;

    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Death);
        owner.Status.InvincibleFlg = true;
        time = 0;
        Debug.Log("Death_DodoOnEnter");
        owner.Death();
    }
    public override void OnExit(Dodo owner, StateBase_Dodo nextState)
    {
        owner.Status.DownFlg = false;

    }
    public override void OnUpdate(Dodo owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;

        time += Time.deltaTime;
        float rate = time / owner.DissoveTime;

        //ñ{ëÃÇè¡ÇµÇƒÇ¢Ç≠
        var Mat = owner.SkinnedMeshRenderer.materials;
        foreach (var mat in Mat)
        {
            if (mat.HasProperty("_Dissolve"))
            {
                mat.SetFloat("_Dissolve", owner.DissoveCurve.Evaluate(rate));
                if(owner.DissoveCurve.Evaluate(rate)>0)owner.CollectionScript.gameObject.SetActive(false);

            }
        }
            

        if (rate >= 1)
        {
            owner.Delete();
        }

    }
    public override void OnFixedUpdate(Dodo owner)
    {

    }
    public override void OnAnimationEvent(Dodo owner, AnimationEvent animationEvent)
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
    public override void OnCollisionStay(Dodo owner, Collision collision)
    {

    }
}
