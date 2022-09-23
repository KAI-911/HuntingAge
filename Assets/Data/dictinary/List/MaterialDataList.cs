using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class MaterialDataList : MonoBehaviour
{
    public MaterialSaveData _materialSaveData;

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        foreach (var item in _materialSaveData.dictionary)
        {
            Debug.Log("Key: " + item.Key + " Value: " + item.Value);
        }
    }

    /// <summary>
    /// �{�b�N�X����|�[�`�Ɏw�萔�̃A�C�e���𑗂�
    /// �{�b�N�X�Ɏw�萔�Ȃ��ꍇ�̓{�b�N�X�ɂ��邾���n��
    /// �|�[�`�Ɏ󂯎��e�ʂ�����Ȃ��ꍇ�͎󂯎��邾���n��
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�{�b�N�X���炢���ړ������邩</param>
    /// <param name="uinum">�|�[�`�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    public void BoxToPoach(string ID, int move, int uinum)
    {
        if (!_materialSaveData.dictionary.ContainsKey(ID)) return;
        var data = _materialSaveData.dictionary[ID];

        int eraseNumber = move;
        //�{�b�N�X�ɃA�C�e��������Ȃ�
        if (move > data.BoxHoldNumber) eraseNumber = data.BoxHoldNumber;
        //�|�[�`�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.PoachStackNumber - data.PoachHoldNumber) eraseNumber = data.PoachStackNumber - data.PoachHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.PoachHoldNumber <= 0) data.PoachUINumber = uinum;

        data.BoxHoldNumber -= eraseNumber;
        data.PoachHoldNumber += eraseNumber;
        _materialSaveData.dictionary[ID] = data;
    }

    /// <summary>
    /// �|�[�`����{�b�N�X�Ɏw�萔�̃A�C�e���𑗂�
    /// �|�[�`�Ɏw�萔�Ȃ��ꍇ�̓|�[�`�ɂ��邾���n��
    /// �{�b�N�X�Ɏ󂯎��e�ʂ�����Ȃ��ꍇ�͎󂯎��邾���n��
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�{�b�N�X���炢���ړ������邩</param>
    /// <param name="uinum">�{�b�N�X�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    public void PoachToBox(string ID, int move, int uinum)
    {
        if (!_materialSaveData.dictionary.ContainsKey(ID)) return;
        var data = _materialSaveData.dictionary[ID];

        int eraseNumber = move;
        //�|�[�`�ɃA�C�e��������Ȃ�
        if (move > data.PoachHoldNumber) eraseNumber = data.PoachHoldNumber;
        //�{�b�N�X�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.BoxStackNumber - data.BoxHoldNumber) eraseNumber = data.BoxStackNumber - data.BoxHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.BoxHoldNumber <= 0) data.BoxUINumber = uinum;

        data.PoachHoldNumber -= eraseNumber;
        data.BoxHoldNumber += eraseNumber;
        _materialSaveData.dictionary[ID] = data;
    }

    /// <summary>
    /// �|�[�`�Ɏw�萔�̃A�C�e���𑗂�
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�����ړ������邩</param>
    /// <param name="uinum">�|�[�`�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    /// <returns>
    /// ��{�I�ɐ��̒l��Ԃ�
    /// -1 �G���[:�L�[��������Ȃ�����
    /// </returns>
    public int GetToPoach(string ID, int move, int uinum)
    {
        if (!_materialSaveData.dictionary.ContainsKey(ID)) return -1;
        var data = _materialSaveData.dictionary[ID];

        int addNumber = move;
        //�|�[�`�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.PoachStackNumber - data.PoachHoldNumber) addNumber = data.PoachStackNumber - data.PoachHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.PoachHoldNumber <= 0) data.PoachUINumber = uinum;

        data.PoachHoldNumber += addNumber;
        _materialSaveData.dictionary[ID] = data;
        return addNumber;
    }

    /// <summary>
    /// �{�b�N�X�Ɏw�萔�̃A�C�e���𑗂� 
    /// ��{�I�ɐ��̒l��Ԃ�
    /// -1 �G���[:�L�[��������Ȃ�����
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�����ړ������邩</param>
    /// <param name="uinum">�{�b�N�X�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    /// <returns></returns>
    public int GetToBox(string ID, int move, int uinum)
    {
        if (!_materialSaveData.dictionary.ContainsKey(ID)) return -1;
        var data = _materialSaveData.dictionary[ID];

        int addNumber = move;
        //�|�[�`�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.BoxStackNumber - data.BoxHoldNumber) addNumber = data.BoxStackNumber - data.BoxHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.BoxHoldNumber <= 0) data.BoxUINumber = uinum;

        data.BoxHoldNumber += addNumber;
        _materialSaveData.dictionary[ID] = data;
        return addNumber;
    }

}

[System.Serializable]
public struct MaterialData
{
    /// <summary>
    /// ID��Item000����A��
    /// ID��Material000����A��
    /// </summary>
    public string ID;

    /// <summary>
    /// �\������閼�O
    /// </summary>
    public string Name;

    /// <summary>
    /// �A�C�R���̃p�X
    /// </summary>
    public string IconName;

    /// <summary>
    /// �A�C�e���{�b�N�X�ň�g�ŕۑ��ł���ő��
    /// </summary>
    public int BoxStackNumber;

    /// <summary>
    /// �A�C�e���{�b�N�X�łǂ̘g�ɕۑ�����Ă��邩
    /// </summary>
    public int BoxUINumber;

    /// <summary>
    /// �A�C�e���{�b�N�X�łǂꂾ�������Ă��邩
    /// </summary>
    public int BoxHoldNumber;

    /// <summary>
    /// �A�C�e���|�[�`�ň�g�ŕۑ��ł���ő��
    /// </summary>
    public int PoachStackNumber;

    /// <summary>
    /// �A�C�e���|�[�`�łǂ̘g�ɕۑ�����Ă��邩
    /// </summary>
    public int PoachUINumber;

    /// <summary>
    /// �A�C�e���|�[�`�łǂꂾ�������Ă��邩
    /// </summary>
    public int PoachHoldNumber;
}
