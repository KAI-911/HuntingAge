using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartChecker : MonoBehaviour
{
    [SerializeField] private PartType _partType;
    public PartType PartType { get => _partType; }

    [SerializeField] PartCheckerReceiver _receiver;
    private void Start()
    {
        _receiver = transform.root.gameObject.GetComponent<PartCheckerReceiver>();
    }
}
