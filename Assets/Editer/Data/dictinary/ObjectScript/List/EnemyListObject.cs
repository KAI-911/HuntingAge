using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataStorage", menuName = "EnemyDataObject")]
public class EnemyListObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<EnemyData> values = new List<EnemyData>();
    [SerializeField] Dictionary<string, EnemyData> dictionary = new Dictionary<string, EnemyData>();


    public List<string> Keys { get => keys; set => keys = value; }
    public List<EnemyData> Values { get => values; set => values = value; }
}
