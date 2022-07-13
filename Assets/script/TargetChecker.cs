using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetChecker : MonoBehaviour
{
    bool _triggerHit;
    public bool TriggerHit { get => _triggerHit; }
    private Collider _collider;
    public Collider Collider { get => _collider; }
    [SerializeField]@private string _maskTag;

    [SerializeField] private TargetCheckerType _targetCheckerType;
    public TargetCheckerType TargetCheckerType { get => _targetCheckerType; }

    private void Awake()
    {
        _triggerHit = false;
    }
    private void Start()
    {
        
        _collider = GetComponent<Collider>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == _maskTag)
        {
            _triggerHit = true;
            _collider = other;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == _maskTag)
        {
            _triggerHit = true;
            _collider = other;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == _maskTag)
        {
            _triggerHit = false;
            _collider = other;
        }

    }
}
