using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;
public class QuestReception : UIBase
{
    ///�\������e�L�X�g
    private GameObject _questMenu;
    [SerializeField] GameObject _questMenuPrefab;

    //�v���C���[���߂��܂ŗ��������f
    [SerializeField] TargetChecker _questbordChecker;
    [SerializeField] TargetChecker _gateChecker;

    [SerializeField] QuestHolder _questHolder;
    [SerializeField] QuestDataList _questDataList;

    [SerializeField] QuestHolderData _questHolderData;


    private void Start()
    {
        ItemIconList[(int)IconType.LevelSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.QuestSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["Confirmation"]);

        //���Ń{�^���̑傫����IP_TypeSelect�ɍ��킹�Ă���
        ItemIconData itemIconData = ItemIconList[(int)IconType.Confirmation].IconData;
        itemIconData._buttonPrefab = ItemIconList[(int)IconType.LevelSelect].IconData._buttonPrefab;
        ItemIconList[(int)IconType.Confirmation].SetIcondata(itemIconData);

        _currentState = new Close();
        _currentState.OnEnter(this, null);

    }

    private class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UIManager.Instance._player.IsAction = true;
        }

        public override void OnProceed(UIBase owner)
        {
            //�����Ȃ���(==����UI���J���Ă���)�Ƃ��͑������^�[��
            if (!UIManager.Instance._player.IsAction) return;
            if (owner.gameObject.GetComponent<QuestReception>()._questbordChecker.TriggerHit)
            {
                UIManager.Instance._player.IsAction = false;
                owner.ChangeState<QuestLevelSelect>();
            }
        }
    }
    private class QuestLevelSelect : UIStateBase
    {
        ItemIcon itemIcon;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon = owner.ItemIconList[(int)IconType.LevelSelect];
            ItemIconData itemIconData = itemIcon.IconData;
            itemIconData._tableSize = new Vector2(GameManager.Instance.VillageData.VillageLevel, 1);
            itemIconData._textFlg = false;
            itemIcon.SetIcondata(itemIconData);
            var objList = itemIcon.CreateButton(itemIcon.CurrentNunber);

            for (int i = 0; i < GameManager.Instance.VillageData.VillageLevel; i++)
            {
                var t = objList[i].GetComponentInChildren<Text>();
                t.text = "���x��" + (i + 1);
                var b = objList[i].GetComponent<Button>();
                var key = owner.GetComponent<QuestReception>()._questHolder.Keys[i];

                b.onClick.AddListener(() =>
                {
                    owner.GetComponent<QuestReception>()._questHolderData = owner.GetComponent<QuestReception>()._questHolder.Dictionary[key];
                    foreach (var item in owner.GetComponent<QuestReception>()._questHolderData.Quests)
                    {
                        Debug.Log(item);
                    }
                    owner.ChangeState<QuestSelect>();
                });
            }


        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log(owner.GetType());

            itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
    }

    private class QuestSelect : UIStateBase
    {
        ItemIcon itemIcon;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon = owner.ItemIconList[(int)IconType.QuestSelect];
            if (prevState.GetType() == typeof(QuestConfirmation)) return;
            Debug.Log(prevState.GetType());
            owner.GetComponent<QuestReception>()._questMenu = Instantiate(owner.GetComponent<QuestReception>()._questMenuPrefab, GameManager.Instance.ItemCanvas.Canvas.transform);
            //�{�^���̐��𒲐�
            ItemIconData itemIconData = itemIcon.IconData;
            itemIconData._tableSize = new Vector2(owner.GetComponent<QuestReception>()._questHolderData.Quests.Count, 1);
            itemIconData._textFlg = false;
            itemIcon.SetIcondata(itemIconData);

            var objList = itemIcon.CreateButton();
            Debug.Log("�{�^���쐬����");
            for (int i = 0; i < objList.Count; i++)
            {
                var data = owner.GetComponent<QuestReception>()._questDataList.Dictionary[owner.GetComponent<QuestReception>()._questHolderData.Quests[i]];
                var t = objList[i].GetComponentInChildren<Text>();
                t.text = data.Name;
                var b = objList[i].GetComponentInChildren<Button>();
                b.onClick.AddListener(() =>
                {
                    GameManager.Instance.Quest.QuestData = data;
                    owner.ChangeState<QuestConfirmation>();
                });
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            if (nextState.GetType() != typeof(QuestConfirmation))
            {
                itemIcon.DeleteButton();
                Destroy(owner.GetComponent<QuestReception>()._questMenu);
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log(owner.GetType());
            Debug.Log(itemIcon.Buttons.Count);
            itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            var id = owner.GetComponent<QuestReception>()._questDataList.Dictionary[owner.GetComponent<QuestReception>()._questHolderData.Quests[itemIcon.CurrentNunber]].ID;
            owner.GetComponent<QuestReception>().SelectQuest_Rec(id);
        }
        public override void OnProceed(UIBase owner)
        {
            itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<QuestLevelSelect>();
        }
    }

    private class QuestConfirmation : UIStateBase
    {
        ItemIcon itemIcon;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon = owner.ItemIconList[(int)IconType.Confirmation];

            //�{�^���̒���
            ItemIconData itemIconData = itemIcon.IconData;
            itemIconData._textData.text = "���̃N�G�X�g���󂯂܂���";
            itemIcon.SetIcondata(itemIconData);

            var objList = itemIcon.CreateButton();
            for (int i = 0; i < objList.Count; i++)
            {
                var data = owner.GetComponent<QuestReception>()._questDataList.Dictionary[owner.GetComponent<QuestReception>()._questHolderData.Quests[i]];
                var t = objList[i].GetComponentInChildren<Text>();
                var b = objList[i].GetComponentInChildren<Button>();
                if (i == 0)
                {
                    t.text = "�͂�";
                    b.onClick.AddListener(() =>
                    {
                        GameManager.Instance.Quest.QuestData = data;
                        Debug.Log("GoQueest");
                        owner.ChangeState<GoQuest>();
                    });
                }
                else
                {
                    t.text = "������";
                    b.onClick.AddListener(() =>
                    {
                        GameManager.Instance.Quest.QuestData = data;
                        Debug.Log("GoQueest");
                        owner.ChangeState<QuestSelect>();
                    });
                }

            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            if (nextState.GetType() != typeof(QuestSelect))
            {
                owner.ItemIconList[(int)IconType.QuestSelect].DeleteButton();
                Destroy(owner.GetComponent<QuestReception>()._questMenu);
            }
            itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log(owner.GetType());
            itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<QuestSelect>();
        }
    }
    private class GoQuest : UIStateBase
    {
        ItemIcon itemIcon;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UIManager.Instance._player.IsAction = true;
            itemIcon = owner.ItemIconList[(int)IconType.Confirmation];
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("�N�G�X�g�󒍒�");
            if (UIManager.Instance._player.IsAction == false)
            {
                Debug.Log("�N�G�X�g�����́H�j�������?");
                itemIcon.Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            }

        }
        public override void OnProceed(UIBase owner)
        {
            if (UIManager.Instance._player.IsAction == false)
            {
                itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
                return;
            }

            if (owner.GetComponent<QuestReception>()._gateChecker.TriggerHit && UIManager.Instance._player.IsAction)
            {
                UIManager.Instance._player.IsAction = false;
                //�{�^���̒���
                ItemIconData itemIconData = itemIcon.IconData;
                itemIconData._textData.text = "���̃N�G�X�g�ɏo�����܂���";
                itemIcon.SetIcondata(itemIconData);

                var objList = itemIcon.CreateButton();
                for (int i = 0; i < objList.Count; i++)
                {
                    var t = objList[i].GetComponentInChildren<Text>();
                    var b = objList[i].GetComponentInChildren<Button>();
                    if (i == 0)
                    {
                        t.text = "�͂�";
                        b.onClick.AddListener(() =>
                        {
                            UIManager.Instance._player.IsAction = true;
                            itemIcon.DeleteButton();
                            GameManager.Instance.Quest.GoToQuset();
                            owner.ChangeState<Close>();
                        });
                    }
                    else
                    {
                        t.text = "������";
                        b.onClick.AddListener(() =>
                        {
                            UIManager.Instance._player.IsAction = true;
                            itemIcon.DeleteButton();
                        });
                    }
                }
            }

            if (owner.GetComponent<QuestReception>()._questbordChecker.TriggerHit && UIManager.Instance._player.IsAction)
            {
                UIManager.Instance._player.IsAction = false;
                //�{�^���̒���
                ItemIconData itemIconData = itemIcon.IconData;
                itemIconData._textData.text = "���̃N�G�X�g��j�����܂���";
                itemIcon.SetIcondata(itemIconData);

                var objList = itemIcon.CreateButton();
                for (int i = 0; i < objList.Count; i++)
                {
                    var t = objList[i].GetComponentInChildren<Text>();
                    var b = objList[i].GetComponentInChildren<Button>();
                    if (i == 0)
                    {
                        t.text = "�͂�";
                        b.onClick.AddListener(() =>
                        {
                            owner.ChangeState<Close>();
                            UIManager.Instance._player.IsAction = true;
                            itemIcon.DeleteButton();
                        });
                    }
                    else
                    {
                        t.text = "������";
                        b.onClick.AddListener(() =>
                        {
                            UIManager.Instance._player.IsAction = true;
                            itemIcon.DeleteButton();
                        });
                    }

                }

            }
            
        }
        public override void OnBack(UIBase owner)
        {
            UIManager.Instance._player.IsAction = true;
            itemIcon.DeleteButton();

        }
    }



    public void SelectQuest_Rec(string QuestID)
    {
        QuestData data = _questDataList.Dictionary[QuestID];
        GameManager.Instance.Quest.QuestData = data;
        string str = "";
        switch (data.Clear)
        {
            case ClearConditions.TargetSubjugation:


                str += "�N���A����: ";
                foreach (var item in data.TargetName)
                {
                    var tmp = GameManager.Instance.EnemyDataList.Dictionary[item.name];

                    str += tmp.DisplayName + "��" + item.number + "�̓�������\n";
                }
                break;
            case ClearConditions.Gathering:
                break;
            default:
                break;
        }
        str += "���s����: " + (int)(data.Failure + 1) + "��͐s����\n";
        switch (data.Field)
        {
            case Scene.Forest:
                str += "���: �X��";
                break;
            case Scene.Animal:
                str += "���: �����p";
                break;
            default:
                break;
        }
        if(_questMenu!=null)
        {
            var texts = _questMenu.GetComponentsInChildren<Text>();
            texts[0].text= data.Name;
            texts[1].text= str;

        }
    }

    public void GoToQuest_Rec()
    {
        GameManager.Instance.Quest.GoToQuset();
    }

    enum IconType
    {
        LevelSelect,
        QuestSelect,
        Confirmation
    }

}
