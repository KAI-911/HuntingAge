using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private bool _ground;
    [SerializeField] float _limitAngle;
    [SerializeField] float _groundAngle;
    public bool IsGround()
    {
        return _ground;
    }
    private void Start()
    {
        _groundAngle = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            _groundAngle = Vector3.Angle(Vector3.up, hit.normal);
        }
        else
        {
            _groundAngle = 0;
        }

        if(_limitAngle>_groundAngle)
        {
            _ground = true;
        }
        else
        {
            _ground = false;
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
            _groundAngle = Vector3.Angle(Vector3.up, hit.normal);
        }
        else
        {
            _groundAngle = 0;
        }

        if (_limitAngle > _groundAngle)
        {
            _ground = true;
        }
        else
        {
            _ground = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _ground = false;
    }

}
