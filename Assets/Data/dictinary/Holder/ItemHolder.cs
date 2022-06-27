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
    /// 所持できる種類の最大量(枠数)
    /// </summary>
    public int MaxHolding;

    /// <summary>
    /// ボックスかポーチのどっちで保存するか
    /// </summary>
    public ItemStack ItemStack;


    /// <summary>
    /// -1 = 枠が全て埋まっている
    /// -2 = セーブデータがない
    /// -3 = スタック上限
    /// </summary>
    /// <param name="AddID"></param>
    /// <param name="AddNumber"></param>
    /// <returns></returns>
    int ItemAdd(string AddID, int AddNumber)
    {
        //枠が全て埋まっている
        if (MaxHolding <= dictionary.Count) return -1;
        //AddIDがセーブデータから見つからない
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
            //スタック上限の場合
            if (stack <= dictionary[AddID]) return -3;

            if (stack <= dictionary[AddID] + AddNumber)
            {
                AddNumber = (dictionary[AddID] + AddNumber) - stack;
            }

            //既に一つ以上持っている && スタック上限未満
            dictionary[AddID] += AddNumber;
        }
        else
        {
            //まだ一つももっていない && 枠が空いている
            dictionary.Add(AddID, 1);
        }
        return AddNumber;
    }

    /// <summary>
    /// -1 = EraseIDが見つからない
    /// -2 = ItemListにEraseIDがない
    /// </summary>
    /// <param name="EraseID"></param>
    /// <param name="EraseNumber"></param>
    /// <returns></returns>
    int ItemErase(string EraseID, int EraseNumber)
    {
        //EraseIDがセーブデータから見つからない
        if (!GameManager.Instance.ItemDataList.Dictionary.ContainsKey(EraseID)) return -1;
        var data = GameManager.Instance.ItemDataList.Dictionary[EraseID];
        if (dictionary.ContainsKey(EraseID))
        {
            int num = dictionary[EraseID];
            if (num >= EraseNumber)
            {
                //数値を減らす
                dictionary[EraseID] -= EraseNumber;
            }
            else
            {
                //減らす数より保持している数が少なかった場合
                //減らす数を保持している数にし、保持数を0にする
                EraseNumber = num;
                dictionary[EraseID] = 0;
            }
        }
        else
        {
            return -2;//エラー:リストにない
        }
        //保持数を減らした結果0になっていたらリストから削除
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
