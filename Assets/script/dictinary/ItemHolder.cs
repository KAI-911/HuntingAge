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
    /// 所持できる種類の最大量(枠数)
    /// </summary>
    public int MaxHolding;

    /// <summary>
    /// ボックスかポーチのどっちで保存するか
    /// </summary>
    public ItemStack ItemStack;

    public Dictionary<string, int> Dictionary { get => dictionary;}

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
        if (MaxHolding <= Dictionary.Count) return -1;
        //AddIDがセーブデータから見つからない
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
            //スタック上限の場合
            if (stack <= Dictionary[AddID]) return -3;

            if (stack <= Dictionary[AddID] + AddNumber)
            {
                AddNumber = (Dictionary[AddID] + AddNumber) - stack;
            }

            //既に一つ以上持っている && スタック上限未満
            Dictionary[AddID] += AddNumber;
        }
        else
        {
            //まだ一つももっていない && 枠が空いている
            Dictionary.Add(AddID, 1);
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
        if (!SaveData.ContainsKey(EraseID)) return -1;//エラー:データがない
        var data = SaveData.GetClass(EraseID, new MaterialData());
        if (Dictionary.ContainsKey(EraseID))
        {
            int num = Dictionary[EraseID];
            if (num >= EraseNumber)
            {
                //数値を減らす
                Dictionary[EraseID] -= EraseNumber;
            }
            else
            {
                //減らす数より保持している数が少なかった場合
                //減らす数を保持している数にし、保持数を0にする
                EraseNumber = num;
                Dictionary[EraseID] = 0;
            }
        }
        else
        {
            return -2;//エラー:リストにない
        }
        //保持数を減らした結果0になっていたらリストから削除
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
