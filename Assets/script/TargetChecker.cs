using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChecker : MonoBehaviour
{
    bool _triggerHit;
    public bool TriggerHit { get => _triggerHit; }
    private Collider _collider;
    [SerializeField]@private string MaskTag;

    [SerializeField] private TargetCheckerType _targetCheckerType;
    public TargetCheckerType TargetCheckerType { get => _targetCheckerType; }


    private void Start()
    {
        _triggerHit = false;
        _collider = GetComponent<Collider>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == MaskTag)
        {
            _triggerHit = true;
            _collider = other;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == MaskTag)
        {
            _triggerHit = true;
            _collider = other;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == MaskTag)
        {
            _triggerHit = false;
            _collider = other;
        }

    }
}
