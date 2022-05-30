using System;
using UnityEngine;
using System.Threading.Tasks;

public class Hit : MonoBehaviour
{
    private HitReceiver _hitReceiver;

    [SerializeField]
    private PartType _part;
    public PartType Part { get => _part; set => _part = value; }
    
    [SerializeField]
    private string _maskTag;
    public string MaskTag { get => _maskTag; set => _maskTag = value; }

    private Vector3 _collisionPos;
    public Vector3 CollisionPos { get => _collisionPos; set => _collisionPos = value; }

    private void Start()
    {
        _hitReceiver = transform.root.gameObject.GetComponent<HitReceiver>();
        this.gameObject.SetActive(false);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (!_attackFlg && collision.gameObject.tag != _maskTag) return;
    //    //何かに当たったら、最上位の親にHitReceiverがある場合HitReceiverのOnHitを呼び出す
    //    var receiver = transform.root.gameObject.GetComponent<HitReceiver>();
    //    if (receiver != null)
    //    {
    //        foreach (ContactPoint point in collision.contacts)
    //        {
    //            _collisionPos = point.point;
    //        }
    //        receiver.OnHit(collision.gameObject, this);
    //        Debug.Log("maskTag " + _maskTag);
    //        Debug.Log("hitTag  " + collision.gameObject.tag);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != _maskTag) return;
        //何かに当たったら、最上位の親にHitReceiverがある場合HitReceiverのOnHitを呼び出す
        if (_hitReceiver != null)
        {
            _collisionPos = other.ClosestPointOnBounds(this.transform.position);
            _hitReceiver.OnHit(other.gameObject, this);
        }
    }

}


