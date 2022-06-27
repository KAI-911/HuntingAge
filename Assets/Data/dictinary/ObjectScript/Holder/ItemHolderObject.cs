using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemHolderStorage", menuName = "ItemHolderObject")]
public class ItemHolderObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<int> values = new List<int>();
    [SerializeField] Dictionary<string, int> dictionary = new Dictionary<string, int>();


    public List<string> Keys { get => keys; set => keys = value; }
    public List<int> Values { get => values; set => values = value; }
}
