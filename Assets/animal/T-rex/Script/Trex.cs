using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trex : Enemy
{
    public override void Start()
    {
        ChangeState<Idle_Trex>();
    }

}
