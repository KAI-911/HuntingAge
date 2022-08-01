using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestResult : UIBase
{
    private current currentNum;

    private void Start()
    {
        _currentState = new Select();
        _currentState.OnEnter(this, null);
        currentNum = current.item;
    }
    private class Wait: UIStateBase
    {

    }
    private class Select : UIStateBase
    {
        private bool lockFlg;
        ItemIcon _decisionIcon;
        ItemIcon _itemIcon;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UIManager.Instance._player.IsAction = false;
            lockFlg = false;
            var texts = owner.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        texts[0].text = GameManager.Instance.Quest.QuestData.Name;
                        break;
                    case 1:
                        int Minutes = (int)GameManager.Instance.Quest.QuestTime / 60;
                        int Seconds = (int)GameManager.Instance.Quest.QuestTime % 60;
                        texts[1].text = Minutes.ToString() + " 分 " + Seconds.ToString() + " 秒 ";
                        break;
                    case 2:
                        texts[2].text = GameManager.Instance.Quest.DeathCount.ToString() + " ダウン ";
                        break;
                    default:
                        break;
                }
            }
            owner.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);

            _itemIcon = owner.ItemIconList[(int)current.item];
            _decisionIcon = owner.ItemIconList[(int)current.decision];

            _itemIcon.CreateButton();
            //ここでクエストの報酬のアイテムを設定する
            //クエストデータにクリア報酬を追加するのが良さそう

            //全て受け取る
            _decisionIcon.CreateButton();
            _decisionIcon.SetButtonText(0, "全て受け取って終了");
            _decisionIcon.SetButtonOnClick(0, () => {
                owner.GetComponent<UIQuestResult>().ItemToBox();
                GameManager.Instance.SceneChange(GameManager.Instance.VillageScene);
                owner.ChangeState<Wait>();
            });
            _decisionIcon.SetButtonText(1, "全て破棄して終了");
            _decisionIcon.SetButtonOnClick(1, () => {
                GameManager.Instance.SceneChange(GameManager.Instance.VillageScene);
                owner.ChangeState<Wait>();
            });
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            _itemIcon.DeleteButton();
            _decisionIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            var OWNER = owner.GetComponent<UIQuestResult>();
            var vec = UIManager.Instance.InputSelection.ReadValue<Vector2>();

            if (OWNER.currentNum == current.item)
            {
                owner.ItemIconList[(int)current.item].Select(vec);
                if (!lockFlg)
                {
                    var table = owner.ItemIconList[(int)current.item].TableSize;
                    //指定している番号が最下行の場合で下ボタンを押したら
                    if (owner.ItemIconList[(int)current.item].CurrentNunber >= table.x * table.y - table.y && vec.y < 0)
                    {
                        OWNER.currentNum = current.decision;
                        if (owner.ItemIconList[(int)current.item].CurrentNunber % table.x < table.y / 2)
                        {
                            owner.ItemIconList[(int)current.decision].CurrentNunber = 0;
                        }
                        else
                        {
                            owner.ItemIconList[(int)current.decision].CurrentNunber = 1;
                        }
                    }
                }
            }
            else
            {
                owner.ItemIconList[(int)current.decision].Select(vec);
                if (!lockFlg)
                {
                    var table = owner.ItemIconList[(int)current.decision].TableSize;
                    //指定している番号が最上行の場合で上ボタンを押したら
                    if (owner.ItemIconList[(int)current.decision].CurrentNunber < table.y && vec.y > 0)
                    {
                        OWNER.currentNum = current.item;
                    }
                }
            }
            if (vec.sqrMagnitude > 0) lockFlg = true;
            else lockFlg = false;
        }
        public override void OnProceed(UIBase owner)
        {
            var OWNER = owner.GetComponent<UIQuestResult>();

            if (OWNER.currentNum == current.item)
            {
                _itemIcon.CurrentButtonInvoke();
            }
            else
            {
                _decisionIcon.CurrentButtonInvoke();
            }
        }
    }
    enum current
    {
        item,
        decision
    }
    void ItemToBox()
    {
        //報酬のアイテムを全てボックスに送る
    }

}
