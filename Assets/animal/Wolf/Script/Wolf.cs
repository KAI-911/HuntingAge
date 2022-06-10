using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using System.Threading.Tasks;

public class Wolf : Enemy
{
    public override void Start()
    {
        ChangeState<Idle_Wolf>();
    }
    public override void Update()
    {
        base.Update();

        if (Status.DownFlg && CurrentState.GetType() != typeof(Down_Wolf))
        {
            ChangeState<Down_Wolf>();
        }
        if (Status.HP <= 0 && CurrentState.GetType() != typeof(Death_Wolf))
        {
            ChangeState<Death_Wolf>();
        }

    }
}
