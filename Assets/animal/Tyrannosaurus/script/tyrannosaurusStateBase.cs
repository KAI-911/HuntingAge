using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class tyrannosaurusStateBase
{
    /// <summary>
    /// �X�e�[�g�J�n���ɌĂ΂��
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnEnter(tyrannosaurus owner, tyrannosaurusStateBase prevState) { }

    /// <summary>
    /// ���t���[���Ă΂��
    /// </summary>
    /// <param name="owner"></param>
    public virtual void OnUpdate(tyrannosaurus owner) { }

    /// <summary>
    /// �X�e�[�g�I�����ɌĂ΂��
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="prevState"></param>
    public virtual void OnExit(tyrannosaurus owner, tyrannosaurusStateBase prevState) { }

    /// <summary>
    /// �A�j���[�V�����C�x���g�������ɂ�΂��
    /// </summary>
    /// <param name="_num"></param>
    public virtual void OnAnimetionFunction(tyrannosaurus owner, int _num) { }
    public virtual void OnAnimetionStart(tyrannosaurus owner, int _num) { }
    public virtual void OnAnimetionEnd(tyrannosaurus owner, int _num) { }

}

