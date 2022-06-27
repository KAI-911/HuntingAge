using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "QuestHolderStorage", menuName = "QuestHolderObject")]

public class QuestHolderObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<QuestHolderData> values = new List<QuestHolderData>();
    [SerializeField] Dictionary<string, QuestHolderData> dictionary = new Dictionary<string, QuestHolderData>();


    public List<string> Keys { get => keys; set => keys = value; }
    public List<QuestHolderData> Values { get => values; set => values = value; }

}
