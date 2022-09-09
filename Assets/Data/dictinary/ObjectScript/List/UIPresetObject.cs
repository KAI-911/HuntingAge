using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ItemIconStorage", menuName = "ItemIconObject")]

public class UIPresetObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<ItemIconData> values = new List<ItemIconData>();
    [SerializeField] Dictionary<string, ItemIconData> dictionary = new Dictionary<string, ItemIconData>();
    public List<string> Keys { get => keys; set => keys = value; }
    public List<ItemIconData> Values { get => values; set => values = value; }
}
