using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class tyrannosaurusStateBase
{
    /// <summary>
    /// ステート開始時に呼ばれる
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnEnter(tyrannosaurus owner, tyrannosaurusStateBase prevState) { }

    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    /// <param name="owner"></param>
    public virtual void OnUpdate(tyrannosaurus owner) { }

    /// <summary>
    /// ステート終了時に呼ばれる
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnExit(tyrannosaurus owner, tyrannosaurusStateBase prevState) { }

    /// <summary>
    /// アニメーションイベント発生時によばれる
    /// </summary>
    /// <param name="_num"></param>
    public virtual void OnAnimetionFunction(tyrannosaurus owner, int _num) { }
    public virtual void OnAnimetionStart(tyrannosaurus owner, int _num) { }
    public virtual void OnAnimetionEnd(tyrannosaurus owner, int _num) { }

}

