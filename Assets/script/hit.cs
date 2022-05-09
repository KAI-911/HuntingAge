using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

public class hit : MonoBehaviour
{
    public AttackInfo info;
    bool active = true;

    public void SetActive(bool _active)
    {
        active = _active;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!active) return;
        info.transform = transform;
        ExecuteEvents.Execute<IAttackDamage>(
        target: other.gameObject,
        eventData: null,
        functor: (reciever, eventData) => reciever.OnDamaged(info));
    }
}



public interface IAttackDamage : IEventSystemHandler
{
    public void OnDamaged(AttackInfo _attackInfo);
}
public class AttackInfo
{
    [SerializeField] public Transform transform;
    [SerializeField] public int damage;
    [SerializeField] public string name;
}
