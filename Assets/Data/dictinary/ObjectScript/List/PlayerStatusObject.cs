using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStatusStorage", menuName = "PlayerStatusObject")]
public class PlayerStatusObject : ScriptableObject
{
    public int MaxHP;
    public int MaxSP;
    public int Attack;
    public int Defense;
}
