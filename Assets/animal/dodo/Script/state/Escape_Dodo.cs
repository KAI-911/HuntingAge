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
        //�����鑬�x�ɕύX
        owner.NavMeshAgent.speed = speed;

        owner.Animator.SetInteger("AniState", (int)State.Escape);
        owner.Animator.Play("escape", 0, Random.Range(0f, 1f));
        float nearDis = float.MaxValue;
        //��ԋ߂�������ꏊ��ڎw��
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
        //�΂݂܂œ��B����������Ă���
        if ((owner.NavMeshAgent.destination - owner.transform.position).sqrMagnitude < (owner.NavMeshAgent.stoppingDistance * owner.NavMeshAgent.stoppingDistance))
        {
            _runOnce.Run(() =>
            {
                owner.Status.InvincibleFlg = true;
                _time = 0;
                //�̂̓����蔻�������
                var colls = owner.gameObject.GetComponentsInChildren<Collider>();
                foreach (var item in colls)
                {
                    item.enabled = false;
                }
                owner.SkinnedMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            });

            _time += Time.deltaTime;
            float rate = _time / 3;

            //�{�̂������Ă���
            var Mat = owner.SkinnedMeshRenderer.materials;
            foreach (var mat in Mat)
            {
                if (mat.HasProperty("_Dither"))
                {
                    Debug.Log("�����Ă���");
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
