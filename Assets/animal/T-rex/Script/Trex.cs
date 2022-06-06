using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trex : Enemy
{
    public override void Start()
    {
        ChangeState<Idle_Trex>();
    }
    public override void Update()
    {
        base.Update();

        if (Status.DownFlg && CurrentState.GetType() != typeof(Down_Trex))
        {
            ChangeState<Down_Trex>();
        }
        if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Trex))
        {
            ChangeState<Death_Trex>();
        }

    }
}
