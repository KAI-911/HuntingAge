using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase
{
    /// <summary>
    /// �X�e�[�g�J�n���ɌĂ΂��
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnEnter(Player owner, PlayerStateBase prevState) { }

    /// <summary>
    /// ���t���[���Ă΂��
    /// </summary>
    /// <param name="owner"></param>
    public virtual void OnUpdate(Player owner) { }

    /// <summary>
    /// �X�e�[�g�I�����ɌĂ΂��
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnExit(Player owner, PlayerStateBase prevState) { }

    /// <summary>
    /// �A�j���[�V�����C�x���g�������ɂ�΂��
    /// </summary>
    /// <param name="_num"></param>
    public virtual void�@OnAnimetionFunction(Player owner,int _num) { }
    
    public virtual void�@OnAnimetionStart(Player owner,int _num) { }
    public virtual void�@OnAnimetionEnd(Player owner,int _num) { }

}
