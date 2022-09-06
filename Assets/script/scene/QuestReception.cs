using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class QuestReception : UIBase
{
    ///表示するテキスト
    private GameObject _questMenu;
    [SerializeField] GameObject _questMenuPrefab;

    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _questbordChecker;
    [SerializeField] TargetChecker _gateChecker;

    [SerializeField] QuestHolder _questHolder;
    [SerializeField] QuestDataList _questDataList;

    [SerializeField] QuestHolderData _questHolderData;


    private void Start()
    {
        ItemIconList[(int)IconType.LevelSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.QuestSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);

        //仮でボタンの大きさをIP_TypeSelectに合わせている
        ItemIconData itemIconData = ItemIconList[(int)IconType.Confirmation].IconData;
        itemIconData._buttonPrefab = ItemIconList[(int)IconType.LevelSelect].IconData._buttonPrefab;
        ItemIconList[(int)IconType.Confirmation].SetIcondata(itemIconData);

        _currentState = new Close();
        _currentState.OnEnter(this, null);
        GameManager.Instance.Quest.QuestReset();
    }

    private class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UISoundManager.Instance._player.IsAction = true;
        }

        public override void OnProceed(UIBase owner)
        {
            //動けない時(==他のUIを開いている)ときは早期リターン
            if (!UISoundManager.Instance._player.IsAction) return;
            if (owner.gameObject.GetComponent<QuestReception>()._questbordChecker.TriggerHit)
            {
                UISoundManager.Instance._player.IsAction = false;
                UISoundManager.Instance.PlayDecisionSE();
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
                t.text = "レベル" + (i + 1);
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

            itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
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
            //ボタンの数を調整
            ItemIconData itemIconData = itemIcon.IconData;
            itemIconData._tableSize = new Vector2(owner.GetComponent<QuestReception>()._questHolderData.Quests.Count, 1);
            itemIconData._textFlg = false;
            itemIcon.SetIcondata(itemIconData);

            var objList = itemIcon.CreateButton();
            for (int i = 0; i < objList.Count; i++)
            {
                var data = owner.GetComponent<QuestReception>()._questDataList.Dictionary[owner.GetComponent<QuestReception>()._questHolderData.Quests[i]];
                itemIcon.SetButtonText(i, data.Name);
                itemIcon.SetButtonOnClick(i, () =>
                {
                    //GameManager.Instance.Quest.QuestData = data;
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
            itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            var id = owner.GetComponent<QuestReception>()._questDataList.Dictionary[owner.GetComponent<QuestReception>()._questHolderData.Quests[itemIcon.CurrentNunber]].ID;
            owner.GetComponent<QuestReception>().SelectQuest_Rec(id);
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
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

            //ボタンの調整
            ItemIconData itemIconData = itemIcon.IconData;
            itemIconData._textData.text = "このクエストを受けますか";
            itemIcon.SetIcondata(itemIconData);

            var objList = itemIcon.CreateButton();
            for (int i = 0; i < objList.Count; i++)
            {
                var data = owner.GetComponent<QuestReception>()._questDataList.Dictionary[owner.GetComponent<QuestReception>()._questHolderData.Quests[i]];
                var t = objList[i].GetComponentInChildren<Text>();
                var b = objList[i].GetComponentInChildren<Button>();
                if (i == 0)
                {
                    t.text = "はい";
                    b.onClick.AddListener(() =>
                    {
                        UISoundManager.Instance.PlayDecisionSE();
                        Debug.Log("GoQueest");
                        owner.ChangeState<GoQuest>();
                    });
                }
                else
                {
                    t.text = "いいえ";
                    b.onClick.AddListener(() =>
                    {
                        UISoundManager.Instance.PlayDecisionSE();
                        Debug.Log("QuestSelect");
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
            itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
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
        GameObject backimage;
        GameObject text;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UISoundManager.Instance._player.IsAction = true;
            itemIcon = owner.ItemIconList[(int)IconType.Confirmation];

            backimage = Instantiate(Resources.Load("UI/Image")) as GameObject;
            text = Instantiate(Resources.Load("UI/Text")) as GameObject;

            backimage.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            text.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
            var imageRect = backimage.GetComponent<RectTransform>();
            var textRect = text.GetComponent<RectTransform>();
            imageRect.sizeDelta = new Vector2(300, 100);
            textRect.sizeDelta = new Vector2(300, 100);
            var textText = text.GetComponent<Text>();
            textText.text = GameManager.Instance.Quest.QuestData.Name;
            textText.alignment = TextAnchor.MiddleLeft;
            textText.resizeTextForBestFit = true;
            imageRect.anchoredPosition = new Vector2(-Data.SCR.Width / 2 + Data.SCR.Padding, Data.SCR.Height / 2 - Data.SCR.Padding);
            textRect.anchoredPosition = new Vector2(-Data.SCR.Width / 2 + Data.SCR.Padding, Data.SCR.Height / 2 - Data.SCR.Padding);
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(backimage);
            Destroy(text);
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("クエスト受注中");
            if (!UISoundManager.Instance._player.IsAction && itemIcon.Buttons.Count != 0)
            {
                itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            }
        }
        public override void OnProceed(UIBase owner)
        {
            if (!UISoundManager.Instance._player.IsAction && itemIcon.Buttons.Count != 0)
            {
                UISoundManager.Instance.PlayDecisionSE();
                itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
                return;
            }

            if (owner.GetComponent<QuestReception>()._gateChecker.TriggerHit && UISoundManager.Instance._player.IsAction)
            {
                UISoundManager.Instance._player.IsAction = false;
                //ボタンの調整
                ItemIconData itemIconData = itemIcon.IconData;
                itemIconData._textData.text = "このクエストに出発しますか";
                itemIcon.SetIcondata(itemIconData);

                var objList = itemIcon.CreateButton();
                for (int i = 0; i < objList.Count; i++)
                {
                    var t = objList[i].GetComponentInChildren<Text>();
                    var b = objList[i].GetComponentInChildren<Button>();
                    if (i == 0)
                    {
                        t.text = "はい";
                        b.onClick.AddListener(() =>
                        {
                            UISoundManager.Instance.PlayQuestSE();
                            UISoundManager.Instance._player.IsAction = true;
                            itemIcon.DeleteButton();
                            GameManager.Instance.Quest.GoToQuset();
                            owner.ChangeState<Close>();
                        });
                    }
                    else
                    {
                        t.text = "いいえ";
                        b.onClick.AddListener(() =>
                        {
                            UISoundManager.Instance.PlayDecisionSE();
                            UISoundManager.Instance._player.IsAction = true;
                            itemIcon.DeleteButton();
                        });
                    }
                }
            }

            if (owner.GetComponent<QuestReception>()._questbordChecker.TriggerHit && UISoundManager.Instance._player.IsAction)
            {
                UISoundManager.Instance._player.IsAction = false;
                //ボタンの調整
                ItemIconData itemIconData = itemIcon.IconData;
                itemIconData._textData.text = "このクエストを破棄しますか";
                itemIcon.SetIcondata(itemIconData);

                var objList = itemIcon.CreateButton();
                for (int i = 0; i < objList.Count; i++)
                {
                    var t = objList[i].GetComponentInChildren<Text>();
                    var b = objList[i].GetComponentInChildren<Button>();
                    if (i == 0)
                    {
                        t.text = "はい";
                        b.onClick.AddListener(() =>
                        {
                            UISoundManager.Instance.PlayDecisionSE();
                            owner.ChangeState<Close>();
                            UISoundManager.Instance._player.IsAction = true;
                            itemIcon.DeleteButton();
                        });
                    }
                    else
                    {
                        t.text = "いいえ";
                        b.onClick.AddListener(() =>
                        {
                            UISoundManager.Instance.PlayDecisionSE();
                            UISoundManager.Instance._player.IsAction = true;
                            itemIcon.DeleteButton();
                        });
                    }

                }

            }

        }
        public override void OnBack(UIBase owner)
        {
            UISoundManager.Instance._player.IsAction = true;
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


                str += "クリア条件: ";
                foreach (var item in data.TargetName)
                {
                    var tmp = GameManager.Instance.EnemyDataList.Dictionary[item.name];

                    str += tmp.DisplayName + "を" + item.number + "体討伐する\n";
                }
                break;
            case ClearConditions.Gathering:
                str += "クリア条件: ";
                foreach (var item in data.TargetName)
                {
                    var tmp = GameManager.Instance.MaterialDataList.Dictionary[item.name];

                    str += tmp.Name + "を" + item.number + "個採取する\n";
                }
                break;
            default:
                break;
        }
        str += "失敗条件: " + (int)(data.Failure + 1) + "回力尽きる\n";
        switch (data.Field)
        {
            case Scene.Forest:
                str += "狩場: 森林";
                break;
            case Scene.Animal:
                str += "狩場: 実験用";
                break;
            default:
                break;
        }
        if (_questMenu != null)
        {
            var texts = _questMenu.GetComponentsInChildren<Text>();
            texts[0].text = data.Name;
            texts[1].text = str;
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
