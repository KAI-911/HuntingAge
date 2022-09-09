using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape_Dodo : StateBase_Dodo
{
    private Vector3 _escapePos;
    private RunOnce _runOnce;
    float _time;
    float speed = 2.5f;
    float defaultSpeed;

    public override void OnEnter(Dodo owner, StateBase_Dodo prevState)
    {
        defaultSpeed = owner.NavMeshAgent.speed;
        //逃げる速度に変更
        owner.NavMeshAgent.speed = speed;

        owner.Animator.SetInteger("AniState", (int)State.Escape);
        owner.Animator.Play("escape", 0, Random.Range(0f, 1f));
        float nearDis = float.MaxValue;
        //一番近い逃げる場所を目指す
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
        //茂みまで到達したら消えていく
        if ((owner.NavMeshAgent.destination - owner.transform.position).sqrMagnitude < (owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance))
        {
            _runOnce.Run(() =>
            {
                owner.Status.InvincibleFlg = true;
                _time = 0;
                //体の当たり判定を消す
                var colls = owner.gameObject.GetComponentsInChildren<Collider>();
                foreach (var item in colls)
                {
                    item.enabled = false;
                }
                owner.SkinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            });

            _time += Time.deltaTime;
            float rate = _time / 3;

            //本体を消していく
            var Mat = owner.SkinnedMeshRenderer.materials;
            foreach (var mat in Mat)
            {
                if (mat.HasProperty("_Dither"))
                {
                    Debug.Log("消えていく");
                    mat.SetFloat("_Dither", owner.DitherCurve.Evaluate(rate));
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
