using UnityEngine;
using UnityEngine.EventSystems;

public class HitReceiver : MonoBehaviour
{

    [SerializeField] private Hit[] _hits;
    public Hit[] Hits { get => _hits; set => _hits = value; }

    [SerializeField] private string _maskTag;
    public string MaskTag { get => _maskTag; set => _maskTag = value; }

    private void Start()
    {
        foreach (var element in Hits)
        {
            element.MaskTag = MaskTag;
        }
    }

    public void ChangeAttackFlg(PartType _part)
    {
        foreach (var element in _hits)
        {
            if (_part == element.Part) element.gameObject.SetActive(!element.gameObject.activeSelf);
        }
    }
    public void AttackFlgReset()
    {
        foreach (var element in _hits)
        {
            element.gameObject.SetActive(false);
        }
    }

    public virtual void OnHit(GameObject _hitObject, Hit _hit)
    {
        AttackInfo info = new AttackInfo();
        //HitÇ©ÇÁéÊìæÇ∑ÇÈ
        info.PartType = _hit.Part;
        info.CllisionPos = _hit.CollisionPos;
        var hitstatus = _hitObject.transform.root.gameObject.GetComponent<Status>();
        var mystatus = transform.root.gameObject.GetComponent<Status>();

        if (hitstatus == null || mystatus == null) return;
        info.Attack = mystatus.Attack;
        info.HitReaction = HitReaction.nonReaction;
        hitstatus.OnDamaged(info);
    }


}


public interface IAttackDamage : IEventSystemHandler
{
    public void OnDamaged(AttackInfo _attackInfo);
}
public enum PartType
{
    notSet,
    head,
    body,
    rightLeg,
    leftLeg,
    tail,
    axe
}
public enum HitReaction
{
    nonReaction,    //îΩâûÇµÇ»Ç¢
    lowReaction,    //åyÇ≠ÇÃÇØÇºÇÈ
    middleReaction, //ÇµÇËÇ‡ÇøÇÇ¬Ç≠
    highReaction    //êÅÇ¡îÚÇ‘
}
