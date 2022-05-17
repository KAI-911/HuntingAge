using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase
{
    /// <summary>
    /// ステート開始時に呼ばれる
    /// 基本的にアニメーターにどのanimationをするのかセットする
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnEnter(Player owner, PlayerStateBase prevState) { }

    /// <summary>
    /// 毎フレーム呼ばれる
    /// </summary>
    /// <param name="owner"></param>
    public virtual void OnUpdate(Player owner) { }

    /// <summary>
    /// ステート終了時に呼ばれる
    /// 基本的にtriggerを立てる
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnExit(Player owner, PlayerStateBase prevState) { }

    /// <summary>
    /// アニメーションイベント発生時によばれる
    /// </summary>
    /// <param name="_num"></param>
    public virtual void　OnAnimetionFunction(Player owner, AnimationEvent _animationEvent) { }
    
    public virtual void　OnAnimetionStart(Player owner, AnimationEvent _animationEvent) { }
    public virtual void　OnAnimetionEnd(Player owner, AnimationEvent _animationEvent) { }

}
