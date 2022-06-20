using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private bool _ground;
    [SerializeField] float _limitAngle;
    public bool IsGround()
    {
        return _ground;
    }

    private void OnTriggerEnter(Collider other)
    {
        _ground = true;

    }
    private void OnTriggerStay(Collider other)
    {
        _ground = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _ground = false;
    }

}
