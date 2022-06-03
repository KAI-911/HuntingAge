using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusDeathState : TyrannosaurusState
{
    float time;
    public override void OnEnter(Tyrannosaurus owner, TyrannosaurusState prevState)
    {

        owner.Animator.SetInteger("AniState", (int)TyrannosaurusAnimationState.Death);
        owner.Animator.SetTrigger("Down");
        owner.Status.InvincibleFlg = true;
        time = 0;
    }
    public override void OnExit(Tyrannosaurus owner, TyrannosaurusState nextState)
    {

    }
    public override void OnUpdate(Tyrannosaurus owner)
    {
        owner.NavMeshAgent.destination = owner.transform.position;

        time += Time.deltaTime;
        float rate = time / owner.DissoveTime;
        var myMat = owner.Renderer.material;
        if (myMat.HasProperty("_Threshold"))
        {
            myMat.SetFloat("_Threshold", owner.DissoveCurve.Evaluate(rate));
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
            owner.SkinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
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
