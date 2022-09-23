using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "VillageDataStorage", menuName = "VillageDataObject")]
[Serializable]
public class VillageListObject : ScriptableObject
{
    public int VillageLevel;
    public int BlacksmithLevel;
    public int KitchenLevel;
}
