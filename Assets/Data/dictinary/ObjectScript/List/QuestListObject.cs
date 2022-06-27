using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestDataStorage", menuName = "QuestDataObject")]
public class QuestListObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<QuestData> values = new List<QuestData>();
    [SerializeField] Dictionary<string, QuestData> dictionary = new Dictionary<string, QuestData>();


    public List<string> Keys { get => keys; set => keys = value; }
    public List<QuestData> Values { get => values; set => values = value; }
}

