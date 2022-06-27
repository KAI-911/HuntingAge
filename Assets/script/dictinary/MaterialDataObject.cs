using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialDataStorage", menuName = "MaterialDataObject")]
public class MaterialDataObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<MaterialData> values = new List<MaterialData>();
    [SerializeField] Dictionary<string, MaterialData> dictionary = new Dictionary<string, MaterialData>();


    public List<string> Keys { get => keys; set => keys = value; }
    public List<MaterialData> Values { get => values; set => values = value; }
}
