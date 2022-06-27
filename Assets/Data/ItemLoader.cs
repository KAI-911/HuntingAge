using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoader : MonoBehaviour
{
    //[SerializeField] MaterialData MaterialData;
    //[SerializeField] ItemData ItemData;
    //[SerializeField] List<MaterialData> MaterialDatas;
    //[SerializeField] List<ItemData> ItemDatas;
    //[SerializeField] string keyItem;
    //[SerializeField] string keyMaterial;

    //[ContextMenu("SetMaterial")]
    //private void SetMaterial()
    //{
    //    SaveData.SetClass(keyMaterial, MaterialData);
    //    Debug.Log("マテリアルデータをセットしました");
    //}

    //[ContextMenu("LoadMaterial")]
    //private void LoadMaterial()
    //{
    //    MaterialData = SaveData.GetClass(keyMaterial, MaterialData);
    //    Debug.Log("マテリアルデータをロードしました");
    //}


    //[ContextMenu("SetItem")]
    //private void SetItem()
    //{
    //    SaveData.SetClass(keyItem, ItemData);
    //    Debug.Log("アイテムデータをセットしました");
    //}

    //[ContextMenu("LoadItem")]
    //private void LoadItem()
    //{
    //    ItemData = SaveData.GetClass(keyItem, ItemData);
    //    Debug.Log("アイテムデータをロードしました");
    //}


    //[ContextMenu("LoadList")]
    //private void LoadList()
    //{
    //    MaterialDatas.Clear();
    //    ItemDatas.Clear();
    //    foreach (var item in SaveData.Keys())
    //    {
    //        Debug.Log(item);
    //        if (item.StartsWith("Material"))
    //        {
    //            MaterialDatas.Add(SaveData.GetClass(item, new MaterialData()));
    //        }
    //        else if (item.StartsWith("Item"))
    //        {
    //            ItemDatas.Add(SaveData.GetClass(item, new ItemData()));
    //        }
    //    }
    //}
    //[ContextMenu("SetList")]
    //private void Setist()
    //{
    //    foreach (var item in ItemDatas)
    //    {
    //        SaveData.SetClass(item.ID, item);
    //    }
    //    foreach (var item in MaterialDatas)
    //    {
    //        SaveData.SetClass(item.ID, item);
    //    }
    //}
}
