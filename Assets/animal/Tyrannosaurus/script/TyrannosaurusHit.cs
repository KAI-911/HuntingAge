using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyrannosaurusHit : HitReceiver
{
    //�������Փ˂�����
    public override void OnHit(Part _part, AttackInfo _attackInfo, GameObject _gameObject)
    {
        if (_gameObject.CompareTag("Player"))
        {
            Debug.Log(_attackInfo.name + "��Player�ɓ�������  " + gameObject.tag + "���");
            var player = _gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.OnDamaged(_attackInfo);
            }
        }

    }

    public override void TriggerEnter(Collider _other, AttackInfo _attackInfo)
    {
        Debug.Log("OnTriggerEnter");

    }

    public override void TriggerStay(Collider _other, AttackInfo _attackInfo)
    {
        Debug.Log("OnTriggerStay");

    }

    public override void TriggerExit(Collider _other, AttackInfo _attackInfo)
    {
        Debug.Log("OnTriggerExit");

    }


}
