using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] ItemHolderObject DictionaryData;
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<int> values = new List<int>();
    [SerializeField] Dictionary<string, int> dictionary = new Dictionary<string, int>();
    public bool modifyValues;

    /// <summary>
    /// �����ł����ނ̍ő��(�g��)
    /// </summary>
    public int MaxHolding;

    /// <summary>
    /// �{�b�N�X���|�[�`�̂ǂ����ŕۑ����邩
    /// </summary>
    public ItemStack ItemStack;

    public Dictionary<string, int> Dictionary { get => dictionary;}

    /// <summary>
    /// -1 = �g���S�Ė��܂��Ă���
    /// -2 = �Z�[�u�f�[�^���Ȃ�
    /// -3 = �X�^�b�N���
    /// </summary>
    /// <param name="AddID"></param>
    /// <param name="AddNumber"></param>
    /// <returns></returns>
    int ItemAdd(string AddID, int AddNumber)
    {
        //�g���S�Ė��܂��Ă���
        if (MaxHolding <= Dictionary.Count) return -1;
        //AddID���Z�[�u�f�[�^���猩����Ȃ�
        if (!SaveData.ContainsKey(AddID)) return -2;
        var data = SaveData.GetClass(AddID, new MaterialData());
        if (Dictionary.ContainsKey(AddID))
        {
            int stack = 0;
            switch (ItemStack)
            {
                case ItemStack.Box:
                    stack = data.BoxStackNumber;
                    break;
                case ItemStack.Poach:
                    stack = data.PoachStackNumber;
                    break;
                default:
                    stack = data.PoachStackNumber;
                    break;
            }
            //�X�^�b�N����̏ꍇ
            if (stack <= Dictionary[AddID]) return -3;

            if (stack <= Dictionary[AddID] + AddNumber)
            {
                AddNumber = (Dictionary[AddID] + AddNumber) - stack;
            }

            //���Ɉ�ȏ㎝���Ă��� && �X�^�b�N�������
            Dictionary[AddID] += AddNumber;
        }
        else
        {
            //�܂���������Ă��Ȃ� && �g���󂢂Ă���
            Dictionary.Add(AddID, 1);
        }
        return AddNumber;
    }

    /// <summary>
    /// -1 = EraseID��������Ȃ�
    /// -2 = ItemList��EraseID���Ȃ�
    /// </summary>
    /// <param name="EraseID"></param>
    /// <param name="EraseNumber"></param>
    /// <returns></returns>
    int ItemErase(string EraseID, int EraseNumber)
    {
        if (!SaveData.ContainsKey(EraseID)) return -1;//�G���[:�f�[�^���Ȃ�
        var data = SaveData.GetClass(EraseID, new MaterialData());
        if (Dictionary.ContainsKey(EraseID))
        {
            int num = Dictionary[EraseID];
            if (num >= EraseNumber)
            {
                //���l�����炷
                Dictionary[EraseID] -= EraseNumber;
            }
            else
            {
                //���炷�����ێ����Ă��鐔�����Ȃ������ꍇ
                //���炷����ێ����Ă��鐔�ɂ��A�ێ�����0�ɂ���
                EraseNumber = num;
                Dictionary[EraseID] = 0;
            }
        }
        else
        {
            return -2;//�G���[:���X�g�ɂȂ�
        }
        //�ێ��������炵������0�ɂȂ��Ă����烊�X�g����폜
        if (Dictionary[EraseID] == 0)
        {
            Dictionary.Remove(EraseID);
        }
        return EraseNumber;
    }


    private void Awake()
    {
        for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count, DictionaryData.Values.Count); i++)
        {
            Dictionary.Add(DictionaryData.Keys[i], DictionaryData.Values[i]);
        }

    }
    public void OnBeforeSerialize()
    {
        if (!modifyValues)
        {
            keys.Clear();
            values.Clear();
            for (int i = 0; i < Mathf.Min(DictionaryData.Keys.Count,DictionaryData.Values.Count); i++)
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
        dictionary = new Dictionary<string, int>();
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
        foreach (var item in Dictionary)
        {
            Debug.Log("Key: " + item.Key + " Value: " + item.Value);
        }
    }
}
