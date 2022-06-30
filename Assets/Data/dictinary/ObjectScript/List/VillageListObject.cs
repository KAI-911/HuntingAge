using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VillageDataStorage", menuName = "VillageDataObject")]

public class VillageListObject : ScriptableObject
{
    public int VillageLevel;
    public int BlacksmithLevel;
    public int KitchenLevel;
}
