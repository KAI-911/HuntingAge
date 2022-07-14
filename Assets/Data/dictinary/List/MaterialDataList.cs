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
    /// ボックスからポーチに指定数のアイテムを送る
    /// ボックスに指定数ない場合はボックスにあるだけ渡す
    /// ポーチに受け取る容量が足りない場合は受け取れるだけ渡す
    /// </summary>
    /// <param name="ID">移動させるアイテムのID</param>
    /// <param name="move">ボックスからいくつ移動させるか</param>
    /// <param name="uinum">ポーチに移動させるアイテムがない場合に使用するUIの場所</param>
    public void BoxToPoach(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int eraseNumber = move;
        //ボックスにアイテムが足りない
        if (move > data.BoxHoldNumber) eraseNumber = data.BoxHoldNumber;
        //ポーチに受け取る容量がない
        if (move > data.PoachStackNumber - data.PoachHoldNumber) eraseNumber = data.PoachStackNumber - data.PoachHoldNumber;

        //UIの位置を設定
        if (data.PoachHoldNumber <= 0) data.PoachUINumber = uinum;

        data.BoxHoldNumber -= eraseNumber;
        data.PoachHoldNumber += eraseNumber;
        values[index] = data;
        DesrializeDictionary();
    }

    /// <summary>
    /// ポーチからボックスに指定数のアイテムを送る
    /// ポーチに指定数ない場合はポーチにあるだけ渡す
    /// ボックスに受け取る容量が足りない場合は受け取れるだけ渡す
    /// </summary>
    /// <param name="ID">移動させるアイテムのID</param>
    /// <param name="move">ボックスからいくつ移動させるか</param>
    /// <param name="uinum">ボックスに移動させるアイテムがない場合に使用するUIの場所</param>
    public void PoachToBox(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int eraseNumber = move;
        //ポーチにアイテムが足りない
        if (move > data.PoachHoldNumber) eraseNumber = data.PoachHoldNumber;
        //ボックスに受け取る容量がない
        if (move > data.BoxStackNumber - data.BoxHoldNumber) eraseNumber = data.BoxStackNumber - data.BoxHoldNumber;

        //UIの位置を設定
        if (data.BoxHoldNumber <= 0) data.BoxUINumber = uinum;

        data.PoachHoldNumber -= eraseNumber;
        data.BoxHoldNumber += eraseNumber;
        values[index] = data;
        DesrializeDictionary();
    }

    /// <summary>
    /// ポーチに指定数のアイテムを送る
    /// </summary>
    /// <param name="ID">移動させるアイテムのID</param>
    /// <param name="move">ポーチからいくつ移動させるか</param>
    /// <param name="uinum">ポーチに移動させるアイテムがない場合に使用するUIの場所</param>
    /// <returns>
    /// 基本的に正の値を返す
    /// -1 エラー:キーが見つからなかった
    /// </returns>
    public int GetToPoach(string ID, int move, int uinum)
    {
        if (!keys.Contains(ID)) return -1;
        int index = keys.IndexOf(ID);
        var data = values[index];

        int addNumber = move;
        //ボックスに受け取る容量がない
        if (move > data.PoachStackNumber - data.PoachHoldNumber) addNumber = data.PoachStackNumber - data.PoachHoldNumber;

        //UIの位置を設定
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
    /// IDはItem000から連番
    /// IDはMaterial000から連番
    /// </summary>
    public string ID;

    /// <summary>
    /// 表示される名前
    /// </summary>
    public string Name;

    /// <summary>
    /// アイコンのパス
    /// </summary>
    public string IconName;

    /// <summary>
    /// アイテムボックスで一枠で保存できる最大量
    /// </summary>
    public int BoxStackNumber;

    /// <summary>
    /// アイテムボックスでどの枠に保存されているか
    /// </summary>
    public int BoxUINumber;

    /// <summary>
    /// アイテムボックスでどれだけ持っているか
    /// </summary>
    public int BoxHoldNumber;

    /// <summary>
    /// アイテムポーチで一枠で保存できる最大量
    /// </summary>
    public int PoachStackNumber;

    /// <summary>
    /// アイテムポーチでどの枠に保存されているか
    /// </summary>
    public int PoachUINumber;

    /// <summary>
    /// アイテムポーチでどれだけ持っているか
    /// </summary>
    public int PoachHoldNumber;
}
