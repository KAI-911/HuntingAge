using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : PlayerStateBase
{
    float collectionTime;
    string _getItem;
    int _getNumber;
    public override void OnEnter(Player owner, PlayerStateBase prevState)
    {
        collectionTime = owner.CollectionTime;
        _getItem = owner.CollectionScript.GetItemID();
        _getNumber = owner.CollectionScript.GetNumber();
        owner.CollectionScript.CollectableTimes -= 1;
        owner.Animator.SetInteger("AniState", (int)PlayerAnimationState.Collection);
        owner.Animator.SetTrigger("Change");
    }
    public override void OnUpdate(Player owner)
    {
        collectionTime -= Time.deltaTime;
        if (collectionTime < 0)
        {
            if(!GameManager.Instance.MaterialDataList._materialSaveData.dictionary.ContainsKey(_getItem))
            {
                Debug.Log("取得アイテムが見つからないので何もしない");
                owner.ChangeState<LocomotionState>();
            }
            //ポーチに追加
            int r = GameManager.Instance.UIPoachList.AddPoach(_getItem, _getNumber);
            if (r == -1) Debug.LogError("キーがない");
            if (r > 0) GameManager.Instance.Quest.GatheringCount.Add(_getItem, r);
            owner.ChangeState<LocomotionState>();
        }
    }

}

