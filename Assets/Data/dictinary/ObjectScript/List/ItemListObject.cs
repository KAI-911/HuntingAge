using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDataStorage", menuName = "ItemDataObject")]

public class ItemListObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<ItemData> values = new List<ItemData>();
    [SerializeField] Dictionary<string, ItemData> dictionary = new Dictionary<string, ItemData>();


    public List<string> Keys { get => keys; set => keys = value; }
    public List<ItemData> Values { get => values; set => values = value; }
}
