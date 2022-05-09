using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TyrannosaurusStateBase
{
    /// <summary>
    /// �X�e�[�g�J�n���ɌĂ΂��
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnEnter(Tyrannosaurus owner, TyrannosaurusStateBase prevState) { }

    /// <summary>
    /// ���t���[���Ă΂��
    /// </summary>
    /// <param name="owner"></param>
    public virtual void OnUpdate(Tyrannosaurus owner) { }

    /// <summary>
    /// �X�e�[�g�I�����ɌĂ΂��
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnExit(Tyrannosaurus owner, TyrannosaurusStateBase prevState) { }

    /// <summary>
    /// �A�j���[�V�����C�x���g�������ɂ�΂��
    /// </summary>
    /// <param name="_num"></param>
    public virtual void OnAnimetionFunction(Tyrannosaurus owner, int _num) { }
    public virtual void OnAnimetionStart(Tyrannosaurus owner, int _num) { }
    public virtual void OnAnimetionEnd(Tyrannosaurus owner, int _num) { }

}

