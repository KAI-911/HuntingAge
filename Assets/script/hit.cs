using System;
using UnityEngine;
using System.Threading.Tasks;

public class Hit : MonoBehaviour
{
    [SerializeField] private HitReceiver.Part part = HitReceiver.Part.notSet;
    [SerializeField] private AttackInfo info = new AttackInfo();
    [SerializeField] private int damage;
    [SerializeField] private string partName;
    [SerializeField] private bool trigger;
    [SerializeField] private string targetTag;
    [SerializeField] private GameObject triggerObject = null;
    private void Start()
    {
        info.damage = damage;
        info.name = partName;
    }
    public void SetAttackFlg(bool _attackFlg)
    {
        info.attackFlg = _attackFlg;
    }
    public bool GetTrigger()
    {
        return trigger;
    }
    public GameObject GetTriggerGameObject()
    {
        return triggerObject;
    }
    public HitReceiver.Part GetPart()
    {
        return part;
    }
    private void OnCollisionEnter(Collision collision)
    {
        //�U�����肪������Ζ߂�
        if (info.attackFlg == false) return;
        //�����ɓ���������A�ŏ�ʂ̐e��HitReceiver������ꍇHitReceiver��OnHit���Ăяo��
        var receiver = transform.root.gameObject.GetComponent<HitReceiver>();
        if (receiver != null)
        {
            info.transform = transform;
            receiver.OnHit(part, info, collision.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //�U�����肪������Ζ߂�
        if (info.attackFlg == false) return;
        //�����ɓ���������A�ŏ�ʂ̐e��HitReceiver������ꍇHitReceiver��OnHit���Ăяo��
        var receiver = transform.root.gameObject.GetComponent<HitReceiver>();
        if (receiver != null)
        {
            info.transform = transform;
            receiver.OnHit(part, info, other.gameObject);
        }

        if (other.CompareTag(targetTag))
        {
            trigger = true;
            triggerObject = other.gameObject;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            trigger = true;
            triggerObject = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(targetTag))
        {
            trigger = false;
            triggerObject = null;

        }
    }
}


