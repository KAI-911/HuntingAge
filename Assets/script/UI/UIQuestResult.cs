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
                        texts[1].text = Minutes.ToString() + " �� " + Seconds.ToString() + " �b ";
                        break;
                    case 2:
                        texts[2].text = GameManager.Instance.Quest.DeathCount.ToString() + " �_�E�� ";
                        break;
                    default:
                        break;
                }
            }
            owner.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);

            _itemIcon = owner.ItemIconList[(int)current.item];
            _decisionIcon = owner.ItemIconList[(int)current.decision];

            _itemIcon.CreateButton();
            //�����ŃN�G�X�g�̕�V�̃A�C�e����ݒ肷��
            //�N�G�X�g�f�[�^�ɃN���A��V��ǉ�����̂��ǂ�����
            var gm = GameManager.Instance;
            foreach (var reward in gm.Quest.QuestData.QuestRewardDatas)
            {
                MaterialData data;
                if(gm.MaterialDataList.Keys.Contains(reward.name))
                {
                    data = gm.MaterialDataList.Dictionary[reward.name];
                }
                else if(gm.ItemDataList.Keys.Contains(reward.name))
                {
                    data = gm.ItemDataList.Dictionary[reward.name].baseData;
                }
                else
                {
                    continue;
                }
                var index = _itemIcon.FirstNotSetNumber();
                if (index == -1) break;
                var ibutton = _itemIcon.Buttons[index].GetComponent<ItemButton>();
                ibutton.SetData(reward.name, reward.number.ToString(), Resources.Load<Sprite>(data.IconName));
                _itemIcon.SetButtonOnClick(index, () =>
                {
                    owner.GetComponent<UIQuestResult>().ItemToBox(reward.name, reward.number, owner);
                    ibutton.clear();
                    _itemIcon.Buttons[index].GetComponent<Button>().onClick.RemoveAllListeners();
                });
            }

            //�S�Ď󂯎��
            _decisionIcon.CreateButton();
            _decisionIcon.SetButtonText(0, "�S�Ď󂯎���ďI��");
            _decisionIcon.SetButtonOnClick(0, () => {
                owner.GetComponent<UIQuestResult>().AllItemToBox(owner);
                GameManager.Instance.SceneChange(GameManager.Instance.VillageScene);
                owner.ChangeState<UIStateBase>();
            });
            _decisionIcon.SetButtonText(1, "�S�Ĕj�����ďI��");
            _decisionIcon.SetButtonOnClick(1, () => {
                GameManager.Instance.SceneChange(GameManager.Instance.VillageScene);
                owner.ChangeState<UIStateBase>();
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
                    //�w�肵�Ă���ԍ����ŉ��s�̏ꍇ�ŉ��{�^������������
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
                    //�w�肵�Ă���ԍ����ŏ�s�̏ꍇ�ŏ�{�^������������
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
    void AllItemToBox(UIBase owner)
    {
        //��V�̃A�C�e����S�ă{�b�N�X�ɑ���
        for (int i = 0; i < owner.ItemIconList[(int)current.item].Buttons.Count; i++)
        {
            var b = owner.ItemIconList[(int)current.item].Buttons[i].GetComponent<Button>();
            b.onClick.Invoke();
        }
    }
    void ItemToBox(string _ID,int _num, UIBase owner)
    {
        var gm = GameManager.Instance;
        int index = 0;
        if (gm.MaterialDataList.Keys.Contains(_ID))
        {
            //UI�̔ԍ��̐ݒ�
            if (gm.MaterialDataList.Dictionary[_ID].BoxHoldNumber<=0)
            {        
                int num = owner.ItemIconList[(int)current.item].GetSize;
                List<int> vs = new List<int>();
                for (int i = 0; i < num; i++) vs.Add(i);
                foreach (var item in gm.MaterialDataList.Values)
                {
                    if (vs.Count == 0)
                    {
                        break;
                    }
                    if (item.BoxHoldNumber <= 0) continue;
                    vs.Remove(item.BoxUINumber);
                }
                foreach (var item in gm.ItemDataList.Values)
                {
                    if (vs.Count == 0)
                    {
                        break;
                    }
                    if (item.baseData.BoxHoldNumber <= 0) continue;
                    vs.Remove(item.baseData.BoxUINumber);
                }

            }
            gm.MaterialDataList.GetToBox(_ID, _num, index);
        }
        else if (gm.ItemDataList.Keys.Contains(_ID))
        {
            //UI�̔ԍ��̐ݒ�
            if (gm.ItemDataList.Dictionary[_ID].baseData.BoxHoldNumber <= 0)
            {
                int num = owner.ItemIconList[(int)current.item].GetSize;
                List<int> vs = new List<int>();
                for (int i = 0; i < num; i++) vs.Add(i);
                foreach (var item in gm.MaterialDataList.Values)
                {
                    if (vs.Count == 0)
                    {
                        break;
                    }
                    if (item.BoxHoldNumber <= 0) continue;
                    vs.Remove(item.BoxUINumber);
                }
                foreach (var item in gm.ItemDataList.Values)
                {
                    if (vs.Count == 0)
                    {
                        break;
                    }
                    if (item.baseData.BoxHoldNumber <= 0) continue;
                    vs.Remove(item.baseData.BoxUINumber);
                }

            }
            gm.ItemDataList.GetToBox(_ID, _num, index);
        }
    }

}
