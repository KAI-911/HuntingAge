using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPoach : UIBase
{
    private string _addItemID;
    private int _addNumber;
    private void Start()
    {
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IP_TypeSelect"]);
        ItemIconList[(int)IconType.ItemSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["IP_ItemSelect"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        ItemIconList[(int)IconType.Setting].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Setting"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);

        var data = ItemIconList[(int)IconType.Confirmation].IconData;
        data._tableSize = new Vector2(1, 2);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(data);
    }
    private class Close : UIStateBase
    {

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UISoundManager.Instance._player.IsAction = true;
        }
        public override void OnMenu(UIBase owner)
        {
            if (!UISoundManager.Instance._player.IsAction) return;
            UISoundManager.Instance._player.IsAction = false;
            UISoundManager.Instance.PlayDecisionSE();
            owner.ChangeState<FirstSlect>();
        }
    }
    /// <summary>
    /// アイテムの整理か武器の整理か
    /// </summary>
    private class FirstSlect : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();
            Debug.Log(owner.ItemIconList[(int)IconType.TypeSelect].TableSize);

            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(() => owner.ChangeState<ItemSlect>());
            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "アイテム選択";

            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() =>
            {
                if (GameManager.Instance.Quest.QuestData.ID != "")
                {
                    owner.ChangeState<QuestView>();
                }
            });
            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "クエスト確認";

            var button2 = list[2].GetComponent<Button>();
            button2.onClick.AddListener(() => owner.ChangeState<GameEnd>());
            var button2Text = list[2].GetComponentInChildren<Text>();
            if (GameManager.Instance.Quest.IsQuest)
            {
                button2Text.text = "クエスト終了";
            }
            else
            {
                button2Text.text = "ゲーム終了";
            }

            var button3 = list[3].GetComponent<Button>();
            button3.onClick.AddListener(() => owner.ChangeState<Setting>());
            var button3Text = list[3].GetComponentInChildren<Text>();
            button3Text.text = "設定";

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.TypeSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            Debug.Log("UIStateBase_FirstSlect");
        }
    }
    private class ItemSlect : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(UIChange)) return;

            owner.ItemIconList[(int)IconType.ItemSelect].CreateButton();

            owner.GetComponent<UIPoach>().UISet();
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            if (nextState.GetType() == typeof(UIChange)) return;

            owner.ItemIconList[(int)IconType.ItemSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ItemSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            Debug.Log("ItemSlect_SecondSelect");
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ChangeState<UIChange>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<FirstSlect>();
        }
    }
    private class UIChange : UIStateBase
    {
        private int _selectionNumber;
        private ItemIcon _itemIcon;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("UI変更ステートになた");
            _itemIcon = owner.GetComponent<UIPoach>().ItemIconList[(int)IconType.ItemSelect];
            _selectionNumber = _itemIcon.CurrentNunber;

        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("UI変更ステート");
            _itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());

        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<ItemSlect>();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            //お互いのUI座標を入れ替える
            var selectButton = _itemIcon.Buttons[_selectionNumber].GetComponent<ItemButton>();
            var currentButton = _itemIcon.Buttons[_itemIcon.CurrentNunber].GetComponent<ItemButton>();
            var MaterialDataList = GameManager.Instance.MaterialDataList;
            var ItemDataList = GameManager.Instance.ItemDataList;
            MaterialData data = new MaterialData();
            if (selectButton.ID != "")
            {
                if (MaterialDataList.Keys.Contains(selectButton.ID))
                {
                    int index = MaterialDataList.Keys.IndexOf(selectButton.ID);
                    data = MaterialDataList.Values[index];
                    data.PoachUINumber = _itemIcon.CurrentNunber;
                    MaterialDataList.Values[index] = data;
                }
                else if (ItemDataList.Dictionary.ContainsKey(selectButton.ID))
                {
                    int index = ItemDataList.Keys.IndexOf(selectButton.ID);
                    data = ItemDataList.Values[index].baseData;
                    data.PoachUINumber = _itemIcon.CurrentNunber;
                    var tmp = ItemDataList.Values[index];
                    tmp.baseData = data;
                    ItemDataList.Values[index] = tmp;
                }
            }
            if (currentButton.ID != "")
            {
                if (MaterialDataList.Keys.Contains(currentButton.ID))
                {
                    int index = MaterialDataList.Keys.IndexOf(currentButton.ID);
                    data = MaterialDataList.Values[index];
                    data.PoachUINumber = _selectionNumber;
                    MaterialDataList.Values[index] = data;
                }
                else if (ItemDataList.Dictionary.ContainsKey(currentButton.ID))
                {
                    int index = ItemDataList.Keys.IndexOf(currentButton.ID);
                    data = ItemDataList.Values[index].baseData;
                    data.PoachUINumber = _selectionNumber;
                    var tmp = ItemDataList.Values[index];
                    tmp.baseData = data;
                    ItemDataList.Values[index] = tmp;
                }
            }

            MaterialDataList.DesrializeDictionary();
            ItemDataList.DesrializeDictionary();
            owner.GetComponent<UIPoach>().UISet();
            owner.ChangeState<ItemSlect>();

        }
    }
    private class QuestView : UIStateBase
    {
        GameObject questBord;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            questBord = Instantiate(Resources.Load("UI/QuestMenu"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;

            QuestData data = GameManager.Instance.Quest.QuestData;

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
            var texts = questBord.GetComponentsInChildren<Text>();
            texts[0].text = data.Name;
            texts[1].text = str;

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(questBord);
        }
        public override void OnUpdate(UIBase owner)
        {
        }
        public override void OnProceed(UIBase owner)
        {
        }
        public override void OnBack(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ChangeState<FirstSlect>();
        }

    }
    private class GameEnd : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var data = owner.ItemIconList[(int)IconType.Confirmation].IconData;
            data._tableSize = new Vector2(1, 2);
            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(data);

            var buttons = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
            Debug.Log(buttons.Count);

            if (GameManager.Instance.Quest.IsQuest)
            {
                owner.ItemIconList[(int)IconType.Confirmation].SetText("クエストをリタイアしますか");
                buttons[0].GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameManager.Instance.Quest.QuestRetire();
                    owner.ChangeState<Close>();
                });
            }
            else
            {
                owner.ItemIconList[(int)IconType.Confirmation].SetText("ゲームを終了しますか");
                buttons[0].GetComponent<Button>().onClick.AddListener(() =>
                {
                    var data = UISoundManager.Instance._player.StatusData;
                    data.Wepon = UISoundManager.Instance._player.WeaponID;
                    data.DesrializeDictionary();
                    owner.ChangeState<Close>();
#if UNITY_EDITOR
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                    Application.Quit();//ゲームプレイ終了
#endif

                });
            }

            buttons[1].GetComponent<Button>().onClick.AddListener(() =>
            {
                owner.ChangeState<FirstSlect>();
            });

            buttons[0].GetComponentInChildren<Text>().text = "はい";
            buttons[1].GetComponentInChildren<Text>().text = "いいえ";

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Confirmation].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.Confirmation].CurrentButtonInvoke();

        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<FirstSlect>();
        }

    }
    private class Setting : UIStateBase
    {
        GameObject _bgmObj;
        GameObject _seObj;
        GameObject _uiObj;
        GameObject _cameraObj;
        Slider slider_bgm;
        Slider slider_se;
        Slider slider_ui;
        Slider slider_camera;
        SettingDataList dataList;
        enum now
        {
            select,
            bgm,
            se,
            ui,
            camera
        }
        now nowState;
        bool ret;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            ret = false;
            nowState = now.select;
            dataList = GameManager.Instance.SettingDataList;
            var buttons = owner.ItemIconList[(int)IconType.Setting].CreateButton();
            //BGMボタンの設定
            buttons[0].GetComponent<Button>().onClick.AddListener(() =>
            {
                nowState = now.bgm;
                slider_bgm.interactable = true;
            });
            buttons[0].GetComponentInChildren<Text>().text = "BGM音量";
            //BGMスライダーの設定
            _bgmObj = Instantiate(Resources.Load("UI/Slider"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            slider_bgm = _bgmObj.GetComponent<Slider>();
            owner.ItemIconList[(int)IconType.Setting].AdjustmentImage(_bgmObj.GetComponent<RectTransform>(), 0);

            //SEボタンの設定
            buttons[1].GetComponent<Button>().onClick.AddListener(() =>
            {
                nowState = now.se;
                slider_se.interactable = true;
            });
            buttons[1].GetComponentInChildren<Text>().text = "SE音量";
            //SEスライダーの設定
            _seObj = Instantiate(Resources.Load("UI/Slider"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            slider_se = _seObj.GetComponent<Slider>();
            owner.ItemIconList[(int)IconType.Setting].AdjustmentImage(_seObj.GetComponent<RectTransform>(), 1);

            //UIボタンの設定
            buttons[2].GetComponent<Button>().onClick.AddListener(() =>
            {
                nowState = now.ui;
                slider_ui.interactable = true;
            });
            buttons[2].GetComponentInChildren<Text>().text = "UI音量";
            //UIスライダーの設定
            _uiObj = Instantiate(Resources.Load("UI/Slider"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            slider_ui = _uiObj.GetComponent<Slider>();
            owner.ItemIconList[(int)IconType.Setting].AdjustmentImage(_uiObj.GetComponent<RectTransform>(), 2);

            //カメラ感度ボタンの設定
            buttons[3].GetComponent<Button>().onClick.AddListener(() =>
            {
                nowState = now.camera;
                slider_camera.interactable = true;
            });
            buttons[3].GetComponentInChildren<Text>().text = "カメラ感度";
            //カメラ感度スライダーの設定
            _cameraObj = Instantiate(Resources.Load("UI/Slider"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            slider_camera = _cameraObj.GetComponent<Slider>();
            owner.ItemIconList[(int)IconType.Setting].AdjustmentImage(_cameraObj.GetComponent<RectTransform>(), 3);
            //背景画像とテキストの調整
            var backrect = owner.ItemIconList[(int)IconType.Setting].ButtonBackObj.GetComponent<RectTransform>();
            var sliderRect = _bgmObj.GetComponent<RectTransform>().sizeDelta;
            backrect.sizeDelta = backrect.sizeDelta + new Vector2(sliderRect.x + owner.ItemIconList[(int)IconType.Setting].IconData._padding, 0);
            //var textRect = owner.ItemIconList[(int)IconType.Setting].TextObj.GetComponent<RectTransform>();
            //textRect.sizeDelta = new Vector2(backrect.sizeDelta.x, textRect.sizeDelta.y);



            slider_bgm.value = dataList.BGMVolume;
            slider_se.value = dataList.SEVolume;
            slider_ui.value = dataList.UIVolume;
            slider_camera.minValue = dataList.CameraMinVolume;
            slider_camera.maxValue = dataList.CameraMaxVolume;
            slider_camera.value = dataList.CameraVolume;
            GameManager.Instance.LookAtCamera.SetSensitivity(slider_camera.value);
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.Setting].DeleteButton();
            Destroy(_bgmObj);
            Destroy(_seObj);
            Destroy(_uiObj);
            Destroy(_cameraObj);
        }
        public override void OnUpdate(UIBase owner)
        {
            if (ret)
            {
                nowState = now.select;
                slider_bgm.interactable = false;
                slider_se.interactable = false;
                slider_ui.interactable = false;
                slider_camera.interactable = false;
                ret = false;
            }
            switch (nowState)
            {
                case now.select:
                    Debug.Log("select");
                    owner.ItemIconList[(int)IconType.Setting].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
                    break;
                case now.bgm:
                    Debug.Log("bgm");
                    slider_bgm.value += Mathf.Clamp(UISoundManager.Instance.InputSelection.ReadValue<Vector2>().x, -0.01f, 0.01f); ;
                    break;
                case now.se:
                    Debug.Log("se");
                    slider_se.value += Mathf.Clamp(UISoundManager.Instance.InputSelection.ReadValue<Vector2>().x, -0.01f, 0.01f); ;
                    break;
                case now.ui:
                    Debug.Log("ui");
                    slider_ui.value += Mathf.Clamp(UISoundManager.Instance.InputSelection.ReadValue<Vector2>().x, -0.01f, 0.01f); ;
                    break;
                case now.camera:
                    Debug.Log("camera");
                    slider_camera.value += Mathf.Clamp(UISoundManager.Instance.InputSelection.ReadValue<Vector2>().x, -5.0f, 5.0f); ;
                    break;
                default:
                    break;
            }
            dataList.BGMVolume = slider_bgm.value;
            dataList.SEVolume = slider_se.value;
            dataList.UIVolume = slider_ui.value;
            dataList.CameraVolume = slider_camera.value;
            dataList.DesrializeDictionary();
            GameManager.Instance.LookAtCamera.SetSensitivity(slider_camera.value);



        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log("OnProceedIn");
            switch (nowState)
            {
                case now.select:
                    UISoundManager.Instance.PlayDecisionSE();
                    owner.ItemIconList[(int)IconType.Setting].CurrentButtonInvoke();
                    break;
                case now.bgm:
                    break;
                case now.se:
                    break;
                case now.ui:
                    break;
                default:
                    break;
            }
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("OnBackIn");
            switch (nowState)
            {
                case now.select:
                    Debug.Log("now.select");
                    owner.ChangeState<FirstSlect>();
                    break;
                case now.bgm:
                case now.se:
                case now.ui:
                case now.camera:
                default:
                    Debug.Log("default");
                    nowState = now.select;
                    ret = true;
                    slider_bgm.interactable = false;
                    slider_se.interactable = false;
                    slider_ui.interactable = false;
                    slider_camera.interactable = false;
                    break;
            }

        }


    }
    private class AddItem : UIStateBase
    {
        ItemIcon itemIcon;
        int addNum;
        string addID;
        float time;
        RunOnce run = new RunOnce();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon = owner.ItemIconList[(int)IconType.Confirmation];
            addNum = owner.GetComponent<UIPoach>()._addNumber;
            addID = owner.GetComponent<UIPoach>()._addItemID;
            Debug.Log(owner.GetComponent<UIPoach>()._addItemID + "    _addItemID");
            if (addID == null) return;
            time = 1;
            var data = itemIcon.IconData;
            Debug.Log(addNum);
            if (addNum == -1)
            {
                Debug.LogError("キーが見つからない");
                owner.ChangeState<Close>();
                return;
            }
            else if (addNum == -2)
            {
                data._tableSize = new Vector2(1, 2);
                data._textData.text = "手持ちがいっぱいです 入れ替えますか";
            }
            else if (addNum == -3)
            {
                data._textData.text = "これ以上持てません";
                data._tableSize = new Vector2(0, 0);
                _ = run.WaitForAsync(time, () => owner.ChangeState<Close>());
            }
            else
            {
                var itemdata = GameManager.Instance.MaterialDataList.Dictionary[addID];
                data._textData.text = itemdata.Name + "を" + addNum + "個入手しました";
                data._tableSize = new Vector2(0, 0);
                _ = run.WaitForAsync(time, () => owner.ChangeState<Close>());
            }

            itemIcon.SetIcondata(data);
            itemIcon.CreateButton();
            //ボタンがない場合
            if ((data._tableSize.x * data._tableSize.y) == 0)
            {
                var backImage = itemIcon.ButtonBackObj.GetComponent<RectTransform>();
                backImage.sizeDelta = new Vector2(200, 50);
                backImage.anchoredPosition = new Vector2(-backImage.sizeDelta.x / 2, backImage.sizeDelta.y);
                var text = itemIcon.TextObj.GetComponent<RectTransform>();
                text.anchoredPosition = backImage.anchoredPosition;
                text.sizeDelta = backImage.sizeDelta;
            }
            else
            {
                var button0 = itemIcon.Buttons[0].GetComponent<Button>();
                button0.onClick.AddListener(() => owner.ChangeState<ItemChange>());
                var button0Text = itemIcon.Buttons[0].GetComponentInChildren<Text>();
                button0Text.text = "はい";
                var button1 = itemIcon.Buttons[1].GetComponent<Button>();
                button1.onClick.AddListener(() => owner.ChangeState<Close>());
                var button1Text = itemIcon.Buttons[1].GetComponentInChildren<Text>();
                button1Text.text = "いいえ";
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            if (itemIcon.Buttons.Count > 0)
            {
                itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            }
        }
        public override void OnProceed(UIBase owner)
        {
            if (itemIcon.Buttons.Count > 0)
            {
                UISoundManager.Instance.PlayDecisionSE();
                itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
            }
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
    }

    private class ItemChange : UIStateBase
    {
        ItemIcon itemIcon;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            itemIcon = owner.ItemIconList[(int)IconType.ItemSelect];
            UISoundManager.Instance._player.IsAction = false;
            var list = itemIcon.CreateButton();
            foreach (var item in GameManager.Instance.MaterialDataList.Dictionary)
            {
                if (item.Value.PoachHoldNumber == 0) continue;
                var ibutton = list[item.Value.PoachUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.ID, ItemBoxOrPoach.poach);
            }
            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.baseData.PoachHoldNumber == 0) continue;
                var ibutton = list[item.Value.baseData.PoachUINumber].GetComponent<ItemButton>();
                ibutton.SetID(item.Value.baseData.ID, ItemBoxOrPoach.poach);
            }

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            UISoundManager.Instance._player.IsAction = true;
            itemIcon.DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            itemIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            var OWNER = owner.GetComponent<UIPoach>();
            var icon = itemIcon.Buttons[itemIcon.CurrentNunber].GetComponent<ItemButton>();
            MaterialDataList materialList = GameManager.Instance.MaterialDataList;
            ItemDataList ItemList = GameManager.Instance.ItemDataList;

            //マテリアルとマテリアルの交換
            if (materialList.Keys.Contains(icon.ID) && materialList.Keys.Contains(OWNER._addItemID))
            {

                int eraseindex = materialList.Keys.IndexOf(icon.ID);
                int addindex = materialList.Keys.IndexOf(OWNER._addItemID);
                var eraseItemData = materialList.Values[eraseindex];
                var addItemData = materialList.Values[addindex];
                eraseItemData.PoachHoldNumber = 0;
                addItemData.PoachHoldNumber = 1;
                addItemData.PoachUINumber = eraseItemData.PoachUINumber;
                materialList.Values[eraseindex] = eraseItemData;
                materialList.Values[addindex] = addItemData;
                materialList.DesrializeDictionary();
            }
            //アイテムとアイテムの交換
            else if (ItemList.Keys.Contains(icon.ID) && ItemList.Keys.Contains(OWNER._addItemID))
            {
                int eraseindex = ItemList.Keys.IndexOf(icon.ID);
                int addindex = ItemList.Keys.IndexOf(owner.GetComponent<UIPoach>()._addItemID);
                var eraseItemData = ItemList.Values[eraseindex];
                var addItemData = ItemList.Values[addindex];
                eraseItemData.baseData.PoachHoldNumber = 0;
                addItemData.baseData.PoachHoldNumber = 1;
                addItemData.baseData.PoachUINumber = eraseItemData.baseData.PoachUINumber;
                ItemList.Values[eraseindex] = eraseItemData;
                ItemList.Values[addindex] = addItemData;
                ItemList.DesrializeDictionary();
            }
            //アイテムとマテリアルの交換
            else if (ItemList.Keys.Contains(icon.ID) && materialList.Keys.Contains(OWNER._addItemID))
            {
                int eraseindex = ItemList.Keys.IndexOf(icon.ID);
                int addindex = materialList.Keys.IndexOf(owner.GetComponent<UIPoach>()._addItemID);
                var eraseItemData = ItemList.Values[eraseindex];
                var addItemData = materialList.Values[addindex];
                eraseItemData.baseData.PoachHoldNumber = 0;
                addItemData.PoachHoldNumber = 1;
                addItemData.PoachUINumber = eraseItemData.baseData.PoachUINumber;
                ItemList.Values[eraseindex] = eraseItemData;
                materialList.Values[addindex] = addItemData;
                ItemList.DesrializeDictionary();
                materialList.DesrializeDictionary();
            }
            //マテリアルとアイテムの交換
            else if (materialList.Keys.Contains(icon.ID) && ItemList.Keys.Contains(OWNER._addItemID))
            {
                int eraseindex = materialList.Keys.IndexOf(icon.ID);
                int addindex = ItemList.Keys.IndexOf(owner.GetComponent<UIPoach>()._addItemID);
                var eraseItemData = materialList.Values[eraseindex];
                var addItemData = ItemList.Values[addindex];
                eraseItemData.PoachHoldNumber = 0;
                addItemData.baseData.PoachHoldNumber = 1;
                addItemData.baseData.PoachUINumber = eraseItemData.PoachUINumber;
                materialList.Values[eraseindex] = eraseItemData;
                ItemList.Values[addindex] = addItemData;
                ItemList.DesrializeDictionary();
                materialList.DesrializeDictionary();
            }
            owner.ChangeState<Close>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<AddItem>();
        }

    }
    public void UISet()
    {
        var list = ItemIconList[(int)IconType.ItemSelect].Buttons;
        //UIセット
        foreach (var item in list)
        {
            var ibutton = item.GetComponent<ItemButton>();
            ibutton.clear();
        }

        foreach (var item in GameManager.Instance.MaterialDataList.Dictionary)
        {
            if (item.Value.PoachHoldNumber == 0) continue;
            var ibutton = list[item.Value.PoachUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
        }
        foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
        {
            if (item.Value.baseData.PoachHoldNumber == 0) continue;
            var ibutton = list[item.Value.baseData.PoachUINumber].GetComponent<ItemButton>();
            ibutton.SetID(item.Key, ItemBoxOrPoach.poach);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="move"></param>
    /// <returns> 
    /// -1 キーが見つからない
    /// -2 UIの枠がない
    /// -3 上限まで持っている
    /// </returns>
    public int AddPoach(string ID, int move)
    {
        int returnValue = move;
        var materialdata = GameManager.Instance.MaterialDataList;
        var itemdata = GameManager.Instance.ItemDataList;

        //アイテムポーチにアイテムがある
        if (materialdata.Keys.Contains(ID))
        {
            if (materialdata.Dictionary[ID].PoachHoldNumber > 0)
            {
                //上限まで持っている 
                if (materialdata.Dictionary[ID].PoachHoldNumber == materialdata.Dictionary[ID].PoachStackNumber)
                {
                    returnValue = -3;
                    _addNumber = returnValue;
                    Debug.Log(_addNumber + "   " + ID);
                    _addItemID = ID;
                    ChangeState<AddItem>();
                    return returnValue;
                }
                returnValue = materialdata.GetToPoach(ID, move, materialdata.Dictionary[ID].PoachUINumber);
                materialdata.DesrializeDictionary();
                _addNumber = returnValue;
                Debug.Log(_addNumber);
                ChangeState<AddItem>();
                return returnValue;
            }
        }
        else if (itemdata.Keys.Contains(ID))
        {
            if (itemdata.Dictionary[ID].baseData.PoachHoldNumber > 0)
            {
                //上限まで持っている
                if (itemdata.Dictionary[ID].baseData.PoachHoldNumber == itemdata.Dictionary[ID].baseData.PoachStackNumber)
                {
                    returnValue = -3;
                    _addNumber = returnValue;
                    Debug.Log(_addNumber);
                    ChangeState<AddItem>();
                    return returnValue;
                }
                returnValue = itemdata.GetToPoach(ID, move, itemdata.Dictionary[ID].baseData.PoachUINumber);
                itemdata.DesrializeDictionary();
                _addNumber = returnValue;
                Debug.Log(_addNumber);
                ChangeState<AddItem>();
                return returnValue;
            }
        }
        else
        {
            return -1;
        }


        _addItemID = ID;
        //アイテムポーチにない場合UIの位置を設定して追加
        var data = ItemIconList[(int)IconType.ItemSelect].IconData;
        int num = (int)data._tableSize.x * (int)data._tableSize.y;
        List<int> vs = new List<int>();
        for (int i = 0; i < num; i++) vs.Add(i);
        foreach (var item in materialdata.Values)
        {
            if (vs.Count == 0)
            {
                break;
            }
            if (item.PoachHoldNumber <= 0) continue;
            vs.Remove(item.PoachUINumber);
        }
        foreach (var item in itemdata.Values)
        {
            if (vs.Count == 0)
            {
                break;
            }
            if (item.baseData.PoachHoldNumber <= 0) continue;
            vs.Remove(item.baseData.PoachUINumber);
        }
        if (vs.Count == 0)
        {
            returnValue = -2;
            _addNumber = returnValue;
            Debug.Log(_addNumber);
            ChangeState<AddItem>();
            return returnValue;
        }

        if (materialdata.Keys.Contains(ID))
        {
            materialdata.GetToPoach(ID, move, vs[0]);
            materialdata.DesrializeDictionary();
        }
        else if (itemdata.Keys.Contains(ID))
        {
            itemdata.GetToPoach(ID, move, vs[0]);
            itemdata.DesrializeDictionary();
        }
        _addNumber = returnValue;
        ChangeState<AddItem>();
        return returnValue;
    }

    enum IconType
    {
        TypeSelect,
        ItemSelect,
        Confirmation,
        Setting
    }

}
