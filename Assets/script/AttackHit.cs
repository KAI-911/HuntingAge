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
    //âπ
    [SerializeField] private GameObject _se;
    public GameObject SE { get => _se; set => _se = value; }

    private void Start()
    {
        _hitReceiver = transform.root.gameObject.GetComponent<HitReceiver>();
        this.gameObject.SetActive(false);
        _hitReceiver.AddHitObject(this);
    }
    private void OnDestroy()
    {
        int num = _hitReceiver.Hits.IndexOf(this);
        _hitReceiver.Hits.RemoveAt(num);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != _mask) return;
        //âΩÇ©Ç…ìñÇΩÇ¡ÇΩÇÁÅAç≈è„à ÇÃêeÇÃHitReceiverÇÃOnHitÇåƒÇ—èoÇ∑
        if (_hitReceiver != null)
        {
            PartType partType = other.gameObject.GetComponent<PartChecker>().PartType;
            _hitPart = partType;
            _collisionPos = other.ClosestPointOnBounds(this.transform.position);
            _hitReceiver.SE = _se;
            _hitReceiver.OnHit(other.gameObject, this);
        }
    }

}


