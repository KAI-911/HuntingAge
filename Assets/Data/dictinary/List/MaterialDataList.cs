using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDataList : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] MaterialListObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<MaterialData> values = new List<MaterialData>();
    [SerializeField] Dictionary<string, MaterialData> dictionary = new Dictionary<string, MaterialData>();
    public bool modifyValues;
    public Dictionary<string, MaterialData> Dictionary { get => dictionary; }
    public List<string> Keys { get => keys; set => keys = value; }
    public List<MaterialData> Values { get => values; set => values = value; }

    private void Awake()
    {
        keys.Clear();
        values.Clear();
        for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count, DictionaryData.Values.Count); i++)
        {
            keys.Add(DictionaryData.Keys[i]);
            values.Add(DictionaryData.Values[i]);
            Dictionary.Add(DictionaryData.Keys[i], DictionaryData.Values[i]);
        }

    }
    public void OnBeforeSerialize()
    {
        if (!modifyValues)
        {
            keys.Clear();
            values.Clear();
            for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count, DictionaryData.Values.Count); i++)
            {
                keys.Add(DictionaryData.Keys[i]);
                values.Add(DictionaryData.Values[i]);
            }
        }
    }

    public void OnAfterDeserialize()
    {

    }
    public void DesrializeDictionary()
    {
        Debug.Log("DesrializeDictionary");
        dictionary.Clear();
        DictionaryData.Keys.Clear();
        DictionaryData.Values.Clear();
        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            DictionaryData.Keys.Add(keys[i]);
            DictionaryData.Values.Add(values[i]);
            Dictionary.Add(keys[i], values[i]);
        }
        modifyValues = false;
    }

    [ContextMenu("PrintDictionary")]
    public void PrintDictionary()
    {
        Debug.Log("Log");
        foreach (var item in Dictionary)
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
        if (!keys.Contains(ID)) return;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int eraseNumber = move;
        //�{�b�N�X�ɃA�C�e��������Ȃ�
        if (move > data.BoxHoldNumber) eraseNumber = data.BoxHoldNumber;
        //�|�[�`�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.PoachStackNumber - data.PoachHoldNumber) eraseNumber = data.PoachStackNumber - data.PoachHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.PoachHoldNumber <= 0) data.PoachUINumber = uinum;

        data.BoxHoldNumber -= eraseNumber;
        data.PoachHoldNumber += eraseNumber;
        values[index] = data;
        DesrializeDictionary();
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
        if (!keys.Contains(ID)) return;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int eraseNumber = move;
        //�|�[�`�ɃA�C�e��������Ȃ�
        if (move > data.PoachHoldNumber) eraseNumber = data.PoachHoldNumber;
        //�{�b�N�X�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.BoxStackNumber - data.BoxHoldNumber) eraseNumber = data.BoxStackNumber - data.BoxHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.BoxHoldNumber <= 0) data.BoxUINumber = uinum;

        data.PoachHoldNumber -= eraseNumber;
        data.BoxHoldNumber += eraseNumber;
        values[index] = data;
        DesrializeDictionary();
    }

    /// <summary>
    /// �|�[�`�Ɏw�萔�̃A�C�e���𑗂�
    /// </summary>
    /// <param name="ID">�ړ�������A�C�e����ID</param>
    /// <param name="move">�|�[�`���炢���ړ������邩</param>
    /// <param name="uinum">�|�[�`�Ɉړ�������A�C�e�����Ȃ��ꍇ�Ɏg�p����UI�̏ꏊ</param>
    /// <returns>
    /// ��{�I�ɐ��̒l��Ԃ�
    /// -1 �G���[:�L�[��������Ȃ�����
    /// </returns>
    public int GetToPoach(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return -1;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int addNumber = move;
        //�{�b�N�X�Ɏ󂯎��e�ʂ��Ȃ�
        if (move > data.PoachStackNumber - data.PoachHoldNumber) addNumber = data.PoachStackNumber - data.PoachHoldNumber;

        //UI�̈ʒu��ݒ�
        if (data.PoachHoldNumber <= 0) data.PoachUINumber = uinum;

        data.PoachHoldNumber += addNumber;
        values[index] = data;
        DesrializeDictionary();
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
