using System;
using UnityEngine;
using System.Threading.Tasks;

public class AttackHit : MonoBehaviour
{
    private HitReceiver _hitReceiver;
    [SerializeField] private PartType _attackPart;
    public PartType AttackPart { get => _attackPart; set => _attackPart = value; }

    private PartType _hitPart;
    public PartType HitPart { get => _hitPart; set => _hitPart = value; }

    [SerializeField] private string _mask;
    public string Mask { get => _mask; set => _mask = value; }

    private Vector3 _collisionPos;
    public Vector3 CollisionPos { get => _collisionPos; set => _collisionPos = value; }


    private void Start()
    {
        _hitReceiver = transform.root.gameObject.GetComponent<HitReceiver>();
        this.gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != _mask) return;
        //‰½‚©‚É“–‚½‚Á‚½‚çAÅãˆÊ‚Ìe‚ÌHitReceiver‚ÌOnHit‚ğŒÄ‚Ño‚·
        if (_hitReceiver != null)
        {
            PartType partType = other.gameObject.GetComponent<PartChecker>().PartType;
            _hitPart = partType;
            _collisionPos = other.ClosestPointOnBounds(this.transform.position);
            _hitReceiver.OnHit(other.gameObject, this);
        }
    }

}


