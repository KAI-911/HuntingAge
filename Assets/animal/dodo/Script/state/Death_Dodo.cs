using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death_Dodo : StateBase_Dodo
{
    float time;
    Vector4 color;

    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Death);
        owner.Status.InvincibleFlg = true;
        time = 0;
        if (owner.ShadowRenderer.material.HasProperty("_ShadowColor"))
        {
            color = owner.ShadowRenderer.material.GetColor("_ShadowColor");
        }
        if (owner.QuestManager != null) owner.QuestManager.AddKillEnemy(owner.EnemyID);

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
            if (mat.HasProperty("_Threshold"))
            {
                mat.SetFloat("_Threshold", owner.DissoveCurve.Evaluate(rate));
            }
        }

        //âeÇè¡ÇµÇƒÇ¢Ç≠
        var shadowMat = owner.ShadowRenderer.materials;
        foreach (var mat in shadowMat)
        {
            if (mat.HasProperty("_ShadowColor"))
            {
                var setColor = color;
                setColor.w = color.w * owner.ShadowCurve.Evaluate(rate);
                mat.SetColor("_ShadowColor", setColor);
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
        }
    }
    public override void OnCollisionStay(Dodo owner, Collision collision)
    {

    }
}
