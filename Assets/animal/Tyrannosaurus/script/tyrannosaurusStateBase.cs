using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TyrannosaurusStateBase
{
    /// <summary>
    /// ステート開始時に呼ばれる
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState) { }

    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    /// <param name="owner"></param>
    public virtual void OnUpdate(Tyrannosaurus owner) { }

    /// <summary>
    /// ステート終了時に呼ばれる
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnExit(Tyrannosaurus owner, TyrannosaurusStateBase prevState) { }

    /// <summary>
    /// アニメーションイベント発生時によばれる
    /// </summary>
    /// <param name="_num"></param>
    public virtual void OnAnimetionFunction(Tyrannosaurus owner, int _num) { }
    public virtual void OnAnimetionStart(Tyrannosaurus owner, int _num) { }
    public virtual void OnAnimetionEnd(Tyrannosaurus owner, int _num) { }

}

