using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : PlayerStateBase
{
    float collectionTime;
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        collectionTime = owner.CollectionTime;
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Collection);
        owner.Animator.SetTrigger("Change");
    }
    public override void OnUpdate(Player owner)
    {
        collectionTime -= Time.deltaTime;
        if (collectionTime < 0)
        {
            //�|�[�`�ɒǉ�
            int r = GameManager.Instance.UIPoachList.AddPoach(owner.CollectionScript.GetItemID(), owner.CollectionScript.GetNumber());
            if (r == -1) Debug.LogError("�L�[���Ȃ�");
            owner.ChangeState<LocomotionState>();
        }
    }
    private void AddItem()
    {
        List<ItemData> itemDatas = new List<ItemData>();
        var data = GameManager.Instance.UIPoachList;


    }
}

