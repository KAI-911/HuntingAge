using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    [SerializeField] bool _ground;
    [SerializeField] string _checkTag;
    public bool IsGround()
    {
        return _ground;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != _checkTag) return;
        _ground = true;

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag!= _checkTag) return;
        _ground = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != _checkTag) return;
        _ground = false;
    }

}
