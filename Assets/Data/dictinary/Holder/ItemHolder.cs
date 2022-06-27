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
    public Dictionary<string, int> Dictionary { get => dictionary; }

    /// <summary>
    /// �����ł����ނ̍ő��(�g��)
    /// </summary>
    public int MaxHolding;

    /// <summary>
    /// �{�b�N�X���|�[�`�̂ǂ����ŕۑ����邩
    /// </summary>
    public ItemStack ItemStack;


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
        if (MaxHolding <= dictionary.Count) return -1;
        //AddID���Z�[�u�f�[�^���猩����Ȃ�
        if (!GameManager.Instance.ItemDataList.Dictionary.ContainsKey(AddID)) return -1;
        var data = GameManager.Instance.ItemDataList.Dictionary[AddID];

        if (dictionary.ContainsKey(AddID))
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
            if (stack <= dictionary[AddID]) return -3;

            if (stack <= dictionary[AddID] + AddNumber)
            {
                AddNumber = (dictionary[AddID] + AddNumber) - stack;
            }

            //���Ɉ�ȏ㎝���Ă��� && �X�^�b�N�������
            dictionary[AddID] += AddNumber;
        }
        else
        {
            //�܂���������Ă��Ȃ� && �g���󂢂Ă���
            dictionary.Add(AddID, 1);
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
        //EraseID���Z�[�u�f�[�^���猩����Ȃ�
        if (!GameManager.Instance.ItemDataList.Dictionary.ContainsKey(EraseID)) return -1;
        var data = GameManager.Instance.ItemDataList.Dictionary[EraseID];
        if (dictionary.ContainsKey(EraseID))
        {
            int num = dictionary[EraseID];
            if (num >= EraseNumber)
            {
                //���l�����炷
                dictionary[EraseID] -= EraseNumber;
            }
            else
            {
                //���炷�����ێ����Ă��鐔�����Ȃ������ꍇ
                //���炷����ێ����Ă��鐔�ɂ��A�ێ�����0�ɂ���
                EraseNumber = num;
                dictionary[EraseID] = 0;
            }
        }
        else
        {
            return -2;//�G���[:���X�g�ɂȂ�
        }
        //�ێ��������炵������0�ɂȂ��Ă����烊�X�g����폜
        if (dictionary[EraseID] == 0)
        {
            dictionary.Remove(EraseID);
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

    [ContextMenu("Load")]
    private void Load()
    {
        modifyValues = true;
        keys.Clear();
        values.Clear();
        dictionary.Clear();
        Debug.Log("Load");
        switch (ItemStack)
        {
            case ItemStack.Box:
                Debug.Log("Box"+ GameManager.Instance.ItemDataList.Dictionary.Count);
                foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
                {
                    if (item.Value.BoxHoldNumber<= 0) continue;
                    Debug.Log(item);
                    keys.Add(item.Key);
                    values.Add(item.Value.BoxHoldNumber);
                    dictionary.Add(item.Key, item.Value.BoxHoldNumber);

                }
                //for (int i = 0; i < GameManager.Instance.ItemDataList.Dictionary.Count; i++)
                //{
                //    Debug.Log("Box"+i);
                //    if (GameManager.Instance.ItemDataList.Dictionary[DictionaryData.Keys[i]].BoxHoldNumber <= 0) continue;
                //    Debug.Log(DictionaryData.Keys[i]);
                //    keys.Add(DictionaryData.Keys[i]);
                //    values.Add(DictionaryData.Values[i]);
                //    dictionary.Add(DictionaryData.Keys[i], DictionaryData.Values[i]);
                //}
                break;
            case ItemStack.Poach:
                Debug.Log("Poach");
                Debug.Log("Poach" + GameManager.Instance.ItemDataList.Dictionary.Count);
                foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
                {
                    if (item.Value.PoachHoldNumber <= 0) continue;
                    Debug.Log(item);
                    keys.Add(item.Key);
                    values.Add(item.Value.PoachHoldNumber);
                    dictionary.Add(item.Key, item.Value.PoachHoldNumber);

                }
                //for (int i = 0; i < GameManager.Instance.ItemDataList.Dictionary.Count; i++)
                //{
                //    Debug.Log("Poach" + i);
                //    if (GameManager.Instance.ItemDataList.Dictionary[DictionaryData.Keys[i]].PoachHoldNumber <= 0) continue;
                //    Debug.Log(DictionaryData.Keys[i]);
                //    keys.Add(DictionaryData.Keys[i]);
                //    values.Add(DictionaryData.Values[i]);
                //    dictionary.Add(DictionaryData.Keys[i], DictionaryData.Values[i]);
                //}
                break;
            default:
                Debug.Log("default");
                break;
        }
        DesrializeDictionary();
    }
}
