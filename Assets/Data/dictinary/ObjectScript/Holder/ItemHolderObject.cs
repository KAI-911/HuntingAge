using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemHolderStorage", menuName = "ItemHolderObject")]
public class ItemHolderObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<CollectionDataLsit> values = new List<CollectionDataLsit>();
    [SerializeField] Dictionary<string, CollectionDataLsit> dictionary = new Dictionary<string, CollectionDataLsit>();


    public List<string> Keys { get => keys; set => keys = value; }
    public List<CollectionDataLsit> Values { get => values; set => values = value; }

}


[System.Serializable]
public struct CollectionDataLsit
{
    public GameObject _imagePrefab;
    public string ID;
    public int GetNum;
    public Sprite Icon;
}
