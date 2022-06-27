using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_Dodo : StateBase_Dodo
{
    private Vector3 _escapePos;
    private RunOnce _runOnce;
    float _time;
    Vector4 _color;
    
    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        owner.Animator.SetInteger("AniState", (int)State.Escape);
        owner.Animator.Play("escape", 0, Random.Range(0f, 1f));
        float nearDis = float.MaxValue;
        //àÍî‘ãﬂÇ¢ì¶Ç∞ÇÈèÍèäÇñ⁄éwÇ∑
        foreach (var pos in owner.EscapePos)
        {
            var dis = (owner.transform.position - pos).magnitude;
            if (nearDis > dis)
            {
                _escapePos = pos;
                nearDis = dis;
            }
        }
        owner.NavMeshAgent.destination = _escapePos;

        _runOnce = new RunOnce();

    }
    public override void OnExit(Dodo owner, StateBase_Dodo nextState)
    {

    }
    public override void OnUpdate(Dodo owner)
    {
        Debug.Log("Escape");
        //ñŒÇ›Ç‹Ç≈ìûíBÇµÇΩÇÁè¡Ç¶ÇƒÇ¢Ç≠
        if ((owner.NavMeshAgent.destination - owner.transform.position).sqrMagnitude < (owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance))
        {
            _runOnce.Run(() =>
            {
                owner.Status.InvincibleFlg = true;
                _time = 0;
                if (owner.ShadowRenderer.material.HasProperty("_ShadowColor"))
                {
                    _color = owner.ShadowRenderer.material.GetColor("_ShadowColor");
                }
                //ëÃÇÃìñÇΩÇËîªíËÇè¡Ç∑
                var colls = owner.gameObject.GetComponentsInChildren<Collider>();
                foreach (var item in colls)
                {
                    item.enabled = false;
                }
            });

            _time += Time.deltaTime;
            float rate = _time / 3;

            //ñ{ëÃÇè¡ÇµÇƒÇ¢Ç≠
            var Mat = owner.SkinnedMeshRenderer.materials;
            foreach (var mat in Mat)
            {
                if (mat.HasProperty("_Dither"))
                {
                    Debug.Log("è¡Ç¶ÇƒÇ¢Ç≠");
                    mat.SetFloat("_Dither", owner.DitherCurve.Evaluate(rate));
                }
            }

            //âeÇè¡ÇµÇƒÇ¢Ç≠
            var shadowMat = owner.ShadowRenderer.materials;
            foreach (var mat in shadowMat)
            {
                if (mat.HasProperty("_ShadowColor"))
                {
                    var setColor = _color;
                    setColor.w = _color.w * owner.ShadowCurve.Evaluate(rate);
                    mat.SetColor("_ShadowColor", setColor);
                }
            }


            if (rate >= 1)
            {
                owner.Delete();
            }

        }
    }
    public override void OnFixedUpdate(Dodo owner)
    {
    }
    public override void OnAnimationEvent(Dodo owner, AnimationEvent animationEvent)
    {

    }
    public override void OnCollisionStay(Dodo owner, Collision collision)
    {

    }
}
