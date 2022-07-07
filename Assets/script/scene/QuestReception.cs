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
    //void Start()
    //{
    //    _currentQuestLevelNumber = 0;
    //    _canvas.enabled = false;
    //    foreach (var item in GameManager.Instance.QuestHolder.Dictionary)
    //    {
    //        QuestDataReception tmp = new QuestDataReception();
    //        tmp.key = item.Key;
    //        tmp.questDatas = new List<QuestData>();
    //        foreach (var quests in item.Value.Quests)
    //        {
    //            tmp.questDatas.Add(GameManager.Instance.QuestDataList.Dictionary[quests]);
    //        }
    //        _questList.Add(tmp);
    //    }
    //}


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
        //_questTitle.text = data.Name;
        //_questContents.text = str;

    }

    public void GoToQuest_Rec()
    {
        GameManager.Instance.Quest.GoToQuset();
    }




    //private void Back(InputAction.CallbackContext obj)
    //{
    //    Debug.Log("Back");
    //    _currentState.OnBack(this);
    //}
    //private void Proceed(InputAction.CallbackContext obj)
    //{
    //    Debug.Log("Proceed");
    //    _currentState.OnProceed(this);
    //}
    //private void Unlock(InputAction.CallbackContext obj)
    //{
    //    Debug.Log("Unlock");
    //    _buttonRunOnce.Flg = false;
    //}
    //public void ChangeState<T>() where T : UIState, new()
    //{
    //    var nextState = new T();
    //    _currentState.OnExit(this, nextState);
    //    nextState.OnEnter(this, _currentState);
    //    _currentState = nextState;
    //}

    //public abstract class UIState
    //{
    //    public virtual void OnEnter(QuestReception owner, UIState prevState)
    //    {

    //    }
    //    public virtual void OnUpdate(QuestReception owner)
    //    {

    //    }
    //    public virtual void OnExit(QuestReception owner, UIState nextState)
    //    {

    //    }
    //    public virtual void OnProceed(QuestReception owner)
    //    {

    //    }
    //    public virtual void OnBack(QuestReception owner)
    //    {

    //    }
    //}
    //[Serializable]
    //public class CanvasClose : UIState
    //{
    //    public override void OnEnter(QuestReception owner, UIState prevState)
    //    {
    //        owner.ButtonDelete();
    //        owner._canvas.enabled = false;
    //        owner._player.IsAction = true;
    //    }
    //    public override void OnUpdate(QuestReception owner)
    //    {
    //    }
    //    public override void OnExit(QuestReception owner, UIState nextState)
    //    {
    //    }
    //    public override void OnProceed(QuestReception owner)
    //    {
    //        //�߂��ɗ��Ă���
    //        if (owner._questbordChecker.TriggerHit)
    //        {
    //            owner.ChangeState<LevelSerect>();
    //        }
    //    }
    //    public override void OnBack(QuestReception owner)
    //    {

    //    }
    //}
    //public class LevelSerect : UIState
    //{
    //    public override void OnEnter(QuestReception owner, UIState prevState)
    //    {
    //        owner._player.IsAction = false;

    //        owner._canvas.enabled = true;
    //        //���x���I�����
    //        owner.ButtonDelete();
    //        owner._currentButtonNumber = owner._currentQuestLevelNumber;
    //        if (owner._currentButtonNumber < 0) owner._currentButtonNumber = 0;
    //        owner._questName.text = "";
    //        owner._questContents.text = "";
    //        owner._canvas.enabled = true;
    //        //�{�^���̒ǉ�
    //        var villageData = GameManager.Instance.VillageData;
    //        for (int i = 0; i < Math.Min(villageData.VillageLevel, GameManager.Instance.QuestHolder.Dictionary.Count); i++)
    //        {
    //            //�{�^���̈ʒu�ݒ�
    //            Vector3 pos = owner.firstButtonPos;
    //            pos.y -= i * owner.buttonBetween;
    //            //�C���X�^���X��
    //            var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
    //            //�e�L�X�g�̐ݒ�
    //            var text = obj.GetComponentInChildren<Text>();
    //            text.text = "level" + (i + 1);
    //            //�e�̐ݒ�
    //            obj.transform.parent = owner._buttonParent.transform;
    //            //�{�^���������ꂽ�Ƃ��̐ݒ�
    //            var button = obj.GetComponent<Button>();
    //            int num = i + 1;
    //            string questHolder = "QuestHolder";
    //            questHolder += String.Format("{0:D3}", num);
    //            //�{�^���������ꂽ�Ƃ��̏���
    //            //_questHolder�ɏ�������QuestSerect�Ɉړ�
    //            int level = i;
    //            button.onClick.RemoveAllListeners();
    //            button.onClick.AddListener(() =>
    //            {
    //                owner._currentQuestLevelNumber = level;
    //            });
    //            owner._buttons.Add(obj);
    //        }
    //        owner._buttons[owner._currentButtonNumber].GetComponent<Button>().Select();

    //    }
    //    public override void OnUpdate(QuestReception owner)
    //    {
    //        //�R���g���[���[�őI���ł���悤�ɂ���
    //        owner.Serect();

    //    }
    //    public override void OnExit(QuestReception owner, UIState nextState)
    //    {
    //        owner.ButtonDelete();
    //    }
    //    public override void OnProceed(QuestReception owner)
    //    {
    //        owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
    //        owner.ChangeState<QuestSerect>();
    //    }
    //    public override void OnBack(QuestReception owner)
    //    {
    //        owner.ChangeState<CanvasClose>();
    //    }
    //}
    //public class QuestSerect : UIState
    //{
    //    public override void OnEnter(QuestReception owner, UIState prevState)
    //    {
    //        owner._player.IsAction = false;

    //        owner.ButtonDelete();
    //        for (int i = 0; i < owner._questList[owner._currentQuestLevelNumber].questDatas.Count; i++)
    //        {
    //            //�{�^���̈ʒu�ݒ�
    //            Vector3 pos = owner.firstButtonPos;
    //            pos.y -= i * owner.buttonBetween;
    //            //�C���X�^���X��
    //            QuestData questData = owner._questList[owner._currentQuestLevelNumber].questDatas[i];
    //            var obj = Instantiate(Resources.Load("UI/Button"), pos, Quaternion.identity) as GameObject;
    //            //�e�̐ݒ�
    //            obj.transform.parent = owner._buttonParent.transform;
    //            //�e�L�X�g�̐ݒ�
    //            var text = obj.GetComponentInChildren<Text>();
    //            text.text = questData.Name;
    //            //�{�^���������ꂽ�Ƃ��̐ݒ�
    //            var button = obj.GetComponent<Button>();
    //            button.onClick.RemoveAllListeners();
    //            button.onClick.AddListener(() =>
    //            {
    //                owner._confirmation.gameObject.SetActive(true);
    //            });
    //            owner._buttons.Add(obj);
    //        }


    //        //�ŏ��̃N�G�X�g�̓��e��\�������邽��
    //        if (owner._buttons.Count >= 1)
    //        {
    //            owner._currentButtonNumber = 0;
    //            var data = owner._questList[owner._currentQuestLevelNumber].questDatas[owner._currentButtonNumber];
    //            owner.SelectQuest_Rec(data.ID);
    //            owner._buttons[0].GetComponent<Button>().Select();
    //        }

    //        //�m�F�p�̃{�^���̊֐��̐ݒ�
    //        owner._confirmation.SetProceedButton(() =>
    //        {
    //            GameManager.Instance.Quest.AcceptingQuest = true;
    //            owner.ChangeState<QuestDecision>();
    //            owner._confirmation.gameObject.SetActive(false);
    //        });
    //        owner._confirmation.SetBackButton(() =>
    //        {
    //            GameManager.Instance.Quest.AcceptingQuest = false;
    //        });
    //        owner._confirmation.SetText("���̃N�G�X�g���󂯂܂���");
    //    }
    //    public override void OnUpdate(QuestReception owner)
    //    {
    //        if (owner._confirmation.gameObject.activeSelf)
    //        {
    //            //�R���g���[���[�őI���ł���悤�ɂ���
    //            owner._confirmation.Selecte();

    //            //�N�G�X�g�̏���������
    //            var data = owner._questList[owner._currentQuestLevelNumber].questDatas[owner._currentButtonNumber];
    //            owner.SelectQuest_Rec(data.ID);
    //        }
    //        else
    //        {
    //            //�R���g���[���[�őI���ł���悤�ɂ���
    //            owner.Serect();

    //            //�N�G�X�g�̏���������
    //            var data = owner._questList[owner._currentQuestLevelNumber].questDatas[owner._currentButtonNumber];
    //            owner.SelectQuest_Rec(data.ID);
    //        }

    //    }
    //    public override void OnExit(QuestReception owner, UIState nextState)
    //    {
    //        owner.ButtonDelete();
    //    }

    //    public override void OnProceed(QuestReception owner)
    //    {
    //        if (owner._confirmation.gameObject.activeSelf == false)
    //        {
    //            owner._buttons[owner._currentButtonNumber].GetComponent<Button>().onClick.Invoke();
    //            owner._confirmation.gameObject.SetActive(true);
    //        }
    //    }

    //    public override void OnBack(QuestReception owner)
    //    {
    //        Debug.Log(owner._currentState.GetType());
    //        if (owner._confirmation.gameObject.activeSelf == false)
    //        {
    //            owner.ChangeState<LevelSerect>();
    //        }
    //    }
    //}


    //public class QuestDecision : UIState
    //{
    //    private Button _currntButton;

    //    public override void OnEnter(QuestReception owner, UIState prevState)
    //    {
    //        owner._player.IsAction = true;

    //        owner.ButtonDelete();
    //        owner._canvas.enabled = false;
    //        owner._confirmation.gameObject.SetActive(false);

    //        owner._confirmation.SetProceedButton(() =>
    //        {
    //            if (owner._questbordChecker.TriggerHit)
    //            {
    //                owner.ChangeState<CanvasClose>();
    //                owner._confirmation.gameObject.SetActive(false);
    //                GameManager.Instance.Quest.AcceptingQuest = false;
    //                owner._player.IsAction = true;

    //            }
    //            else if (owner._gateChecker.TriggerHit)
    //            {
    //                owner.GoToQuest_Rec();
    //                owner._player.IsAction = true;
    //            }
    //        });
    //        owner._confirmation.SetBackButton(() =>
    //        {
    //            if (owner._questbordChecker.TriggerHit)
    //            {
    //                owner._confirmation.gameObject.SetActive(false);
    //                owner._player.IsAction = true;
    //            }
    //            else if (owner._gateChecker.TriggerHit)
    //            {
    //                owner._confirmation.gameObject.SetActive(false);
    //                owner._player.IsAction = true;

    //            }
    //        });

    //    }
    //    public override void OnUpdate(QuestReception owner)
    //    {
    //        if (owner._confirmation.gameObject.activeSelf)
    //        {
    //            owner._confirmation.Selecte();
    //        }
    //    }
    //    public override void OnExit(QuestReception owner, UIState nextState)
    //    {
    //        owner._confirmation.gameObject.SetActive(false);
    //    }
    //    public override void OnProceed(QuestReception owner)
    //    {
    //        if (owner._questbordChecker.TriggerHit)
    //        {
    //            owner._player.IsAction = false;

    //            owner._confirmation.SetText("���̃N�G�X�g��j�����܂���");
    //            owner._confirmation.gameObject.SetActive(true);
    //        }


    //        if (owner._gateChecker.TriggerHit)
    //        {
    //            owner._player.IsAction = false;

    //            owner._confirmation.SetText("�N�G�X�g���o�����܂���");
    //            owner._confirmation.gameObject.SetActive(true);
    //        }
    //    }
    //    public override void OnBack(QuestReception owner)
    //    {

    //    }

    //}

    enum IconType
    {
        LevelSelect,
        QuestSelect,
        Confirmation
    }

}
