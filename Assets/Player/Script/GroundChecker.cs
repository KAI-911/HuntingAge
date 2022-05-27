using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private bool ground;
    public bool IsGround()
    {
        return ground;
    }
    private void OnTriggerEnter(Collider other)
    {
        ground = true;
    }
    private void OnTriggerStay(Collider other)
    {
        ground = true;
    }
    private void Update()
    {
    }
    private void OnTriggerExit(Collider other)
    {
        ground = false;
    }

}
