using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartChecker : MonoBehaviour
{
    [SerializeField] private PartType _partType;
    public PartType PartType { get => _partType; }
}
