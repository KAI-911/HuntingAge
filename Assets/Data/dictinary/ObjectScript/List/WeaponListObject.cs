using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaonDataStorage", menuName = "WeaponDataObject")]

public class WeaponListObject : ScriptableObject
{
    [SerializeField] List<string> keys = new List<string>();
    [SerializeField] List<WeaponData> values = new List<WeaponData>();
    [SerializeField] Dictionary<string, WeaponData> dictionary = new Dictionary<string, WeaponData>();

    public List<string> Keys { get => keys; set => keys = value; }
    public List<WeaponData> Values { get => values; set => values = value; }
}

