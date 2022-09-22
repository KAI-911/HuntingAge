using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class kichen : UIBase
{
    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _kichenChecker;
    //強化する武器のID
    [SerializeField] string _cleateItemID;
    //ボックスかポーチどっちに送るか
    [SerializeField] bool _toPouch;
    //ボタンの色設定
    [SerializeField] Color _cantColor;
    private MaterialExplanation _materialExplanation;
    ItemData _createItemData;
    //何個作るか
    int _cleateNum;
    private Count _count;
    private GameObject countObj;
    enum IconType
    {
        ToPouchSelect,
        ItemSelect,
        Confirmation,
        MaterialList
    }
    void Start()
    {
        _count = new Count();
        _materialExplanation = new MaterialExplanation();
        ItemIconList[(int)IconType.ToPouchSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.ItemSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        ItemIconList[(int)IconType.MaterialList].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }
    bool Check(ItemData _ID)
    {
        ItemData data = _ID;
        for (int i = 0; i < data.NeedMaterialLst.Count; i++)
        {
            string needID = data.NeedMaterialLst[i].materialID;
            int needRequiredCount = data.NeedMaterialLst[i].requiredCount;

            var _material = GameManager.Instance.MaterialDataList;
            if (!(_material.Dictionary.ContainsKey(needID))) return false;
            int _num = _material.Dictionary[needID].BoxHoldNumber + _material.Dictionary[needID].PoachHoldNumber;
            if (_num < needRequiredCount) return false;
        }
        return true;
    }
    public class MaterialExplanation
    {
        private GameObject Back;
        private GameObject Icon;
        private GameObject name;
        private GameObject effect;
        private Vector2 _base;
        public void Set(UIBase owner, int _num = 1)
        {
            //素材リスト作成
            var ListUI = owner.GetComponent<kichen>().ItemIconList[(int)IconType.MaterialList];
            var data = owner.GetComponent<kichen>()._createItemData;
            var List = data.NeedMaterialLst;
            ListUI.SetText("必要素材／素材所持数");
            ListUI.SetTable(new Vector2(List.Count, 1));
            ListUI.SetLeftTopPos(new Vector2(100, 200));
            ListUI.CreateButton();
            for (int i = 0; i < List.Count; i++)
            {
                var materialID = List[i].materialID;
                var material = GameManager.Instance.MaterialDataList.Dictionary[materialID];
                string text1 = string.Format("{0,3:d}", ((List[i].requiredCount) * _num).ToString());
                string text2 = string.Format("{0,4:d}", (material.BoxHoldNumber + material.PoachHoldNumber).ToString());
                Debug.Log(text1);
                Debug.Log(text2);
                ListUI.SetButtonText(i, "　　" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
            }



            _base = new Vector2(150, -100);
            if (Back == null) Back = Instantiate(Resources.Load("UI/Image3"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //アイコン
            if (Icon == null) Icon = Instantiate(Resources.Load("UI/Image3"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //名前
            if (name == null) name = Instantiate(Resources.Load("UI/Text"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //説明文
            if (effect == null) effect = Instantiate(Resources.Load("UI/Text"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;

            Vector2 iconsize = new Vector2(100, 100);

            float padding = 10;
            //背景
            var BackRet = Back.GetComponent<RectTransform>();
            BackRet.pivot = new Vector2(0, 1);
            BackRet.sizeDelta = new Vector2(400 + padding, 100 + padding);
            BackRet.anchoredPosition = _base - new Vector2(iconsize.x, -iconsize.y) / 2;
            //アイコン
            var IconRet = Icon.GetComponent<RectTransform>();
            IconRet.pivot = new Vector2(0.5f, 0.5f);
            IconRet.anchoredPosition = _base - new Vector2(-padding / 2, padding / 2);
            Icon.GetComponent<Image>().sprite = Resources.Load<Sprite>(data.baseData.IconName);
            //名前
            var NameRet = name.GetComponent<RectTransform>();
            NameRet.pivot = new Vector2(0, 0);
            NameRet.sizeDelta = new Vector2(300, 50);
            NameRet.anchoredPosition = _base + new Vector2((iconsize.x + padding) / 2, padding / 2);
            name.GetComponent<Text>().text = data.baseData.Name;
            name.GetComponent<Text>().color = new Color(1, 1, 1);
            //説明文
            var EffectRet = effect.GetComponent<RectTransform>();
            EffectRet.pivot = new Vector2(0, 1);
            EffectRet.sizeDelta = new Vector2(300, 50);
            EffectRet.anchoredPosition = _base + new Vector2((iconsize.x + padding) / 2, padding / 2);
            effect.GetComponent<Text>().text = data.FlavorText;
            effect.GetComponent<Text>().color = new Color(1, 1, 1);
        }
        public void Delete(UIBase owner)
        {
            //素材リスト
            owner.GetComponent<kichen>().ItemIconList[(int)IconType.MaterialList].DeleteButton();
            //背景
            if (Back != null) Destroy(Back);
            //アイコン
            if (Icon != null) Destroy(Icon);
            //名前
            if (name != null) Destroy(name);
            //説明文
            if (effect != null) Destroy(effect);
        }
    }
    
    [Serializable]
    public class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("kichen_close_OnEnter");
            UISoundManager.Instance._player.IsAction = true;
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log("kichen_close_OnProceed");
            //近くに来ている && 決定ボタンを押している && キャンバスがactiveでない
            if (owner.GetComponent<kichen>()._kichenChecker.TriggerHit && UISoundManager.Instance._player.IsAction)
            {
                UISoundManager.Instance._player.IsAction = false;
                owner.ChangeState<BoxorPouch>();
            }
        }
    }
    public class BoxorPouch : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var UI = owner.ItemIconList[(int)IconType.ToPouchSelect];
            UI.SetText("アイテムをどこに送りますか？");
            UI.SetTable(new Vector2(2, 1));
            UI.CreateButton();
            Debug.Log("dasitayo");
            UI.SetButtonText(0, "ボックスへ");
            UI.SetButtonOnClick(0, () => { owner.GetComponent<kichen>()._toPouch = false; owner.ChangeState<ItemSelectMode>(); owner.GetComponent<kichen>()._toPouch = false; });

            UI.SetButtonText(1, "ポーチへ");
            UI.SetButtonOnClick(1, () => { owner.GetComponent<kichen>()._toPouch = true; owner.ChangeState<ItemSelectMode>(); owner.GetComponent<kichen>()._toPouch = true; });
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ToPouchSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        //public override void OnExit(UIBase owner, UIStateBase nextState)
        //{
        //}
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ToPouchSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ToPouchSelect].DeleteButton();
            Debug.Log("modoru");
            owner.ChangeState<Close>();
        }
    }
    public class ItemSelectMode : UIStateBase
    {
        List<ItemData> _createItem = new List<ItemData>();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {   //作れるアイテムのリストを生成
            foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
            {
                if (item.Value.CreatableLevel <= GameManager.Instance.VillageData.KitchenLevel)
                {
                    for (int i = 0; i < item.Value.NeedMaterialLst.Count; i++)
                    {
                        if (!GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(item.Value.NeedMaterialLst[i].materialID)) continue;
                    }
                    _createItem.Add(item.Value);
                }
            }
            //ボタンオブジェクトの設定
            var UI = owner.ItemIconList[(int)IconType.ItemSelect];
            UI.SetText("作るアイテム");
            UI.SetLeftTopPos(new Vector2(-550, 140));
            UI.SetTable(new Vector2(_createItem.Count, 1));
            UI.CreateButton();
            //作れるアイテム分のボタンを生成しそのボタンの各種設定を行う
            for (int i = 0; i < _createItem.Count; i++)
            {
                int numi = i;
                UI.SetButtonText(numi, _createItem[numi].baseData.Name);
                //素材が足りるかのIF文
                if (owner.GetComponent<kichen>().Check(_createItem[numi]))
                {
                    UI.SetButtonOnClick(numi, () =>
                    {
                        if (owner.GetComponent<kichen>()._toPouch)
                        {
                            if (_createItem[numi].baseData.PoachStackNumber <= _createItem[numi].baseData.PoachHoldNumber)
                            {
                                owner.ChangeState<Confirmation>();
                            }
                            else
                            {
                                owner.GetComponent<kichen>()._cleateItemID = _createItem[numi].baseData.ID;
                                owner.ChangeState<Count>();
                            }
                        }
                        else
                        {
                            owner.GetComponent<kichen>()._cleateItemID = _createItem[numi].baseData.ID;
                            owner.ChangeState<Count>();
                        }

                    });
                }
                //素材が足りないとき
                else
                {
                    var image = UI.Buttons[numi].GetComponent<Image>();
                    image.color = owner.GetComponent<kichen>()._cantColor;
                    UI.SetButtonOnClick(i, () => { });
                }
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ItemSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            int buttonCount = owner.ItemIconList[(int)IconType.ItemSelect].CurrentNunber;
            owner.GetComponent<kichen>()._createItemData = _createItem[buttonCount];

            owner.GetComponent<kichen>()._materialExplanation.Set(owner);

            //var ListUI = owner.ItemIconList[(int)IconType.MaterialList];
            //var data = _createItem[buttonCount].NeedMaterialLst;
            //ListUI.SetText("必要素材／素材所持数");
            //ListUI.SetTable(new Vector2(data.Count, 1));
            //ListUI.SetLeftTopPos(new Vector2(100, 200));
            //ListUI.CreateButton();
            //for (int i = 0; i < data.Count; i++)
            //{
            //    var materialID = data[i].materialID;
            //    var material = GameManager.Instance.MaterialDataList.Dictionary[materialID];
            //    string text1 = string.Format("{0,3:d}", data[i].requiredCount.ToString());
            //    string text2 = string.Format("{0,4:d}", (material.BoxHoldNumber + material.PoachHoldNumber).ToString());
            //    Debug.Log(text1);
            //    Debug.Log(text2);
            //    ListUI.SetButtonText(i, "　　" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
            //}
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ItemSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ItemSelect].DeleteButton();
            owner.GetComponent<kichen>()._materialExplanation.Delete(owner);
            owner.ChangeState<BoxorPouch>();
        }
    }
    public class Count : UIStateBase
    {
        int min, max, now;
        private bool lockflg;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {   //作るアイテムのカウント用オブジェクトの生成
            owner.GetComponent<kichen>().countObj = Instantiate(Resources.Load("UI/Count"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            owner.ItemIconList[(int)IconType.ItemSelect].AdjustmentImage(owner.GetComponent<kichen>().countObj.GetComponent<RectTransform>());

            now = 1; min = 1; max = 9999;

            var _itemData = GameManager.Instance.ItemDataList.Dictionary[owner.GetComponent<kichen>()._cleateItemID];
            if (owner.GetComponent<kichen>()._toPouch)
            {
                max = _itemData.baseData.PoachStackNumber - _itemData.baseData.PoachHoldNumber;
            }

            var _needMaterialList = _itemData.NeedMaterialLst;
            var _needMaterialData = GameManager.Instance.MaterialDataList.Dictionary;
            for (int i = 0; i < _needMaterialList.Count; i++)
            {
                int needMaterialNum = _needMaterialList[i].requiredCount;
                int possessionNeedMaterialCount = _needMaterialData[_needMaterialList[i].materialID].BoxHoldNumber + _needMaterialData[_needMaterialList[i].materialID].PoachHoldNumber;
                if ((possessionNeedMaterialCount / needMaterialNum) < max) max = possessionNeedMaterialCount / needMaterialNum;
            }
        }

        public override void OnUpdate(UIBase owner)
        {
            var vec = UISoundManager.Instance.InputSelection.ReadValue<Vector2>();
            owner.GetComponent<kichen>()._materialExplanation.Set(owner,now);
            if (vec.sqrMagnitude > 0)
            {
                if (lockflg == false)
                {
                    if (vec.y > 0) now++;
                    else now--;
                    now = Mathf.Clamp(now, min, max);
                    lockflg = true;

                    owner.GetComponent<kichen>().countObj.GetComponentInChildren<Text>().text = now.ToString();
                }
            }
            else
            {
                lockflg = false;
            }
        }
        public override void OnProceed(UIBase owner)
        {
            owner.GetComponent<kichen>()._cleateNum = now;
            owner.ChangeState<Confirmation>();
        }
        public override void OnBack(UIBase owner)
        {
            Destroy(owner.GetComponent<kichen>().countObj);
            owner.ChangeState<ItemSelectMode>();
        }
    }
    public class Confirmation : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var UI = owner.ItemIconList[(int)IconType.Confirmation];
            if (prevState.GetType() != typeof(ItemSelectMode))
            {
                var Item = GameManager.Instance.ItemDataList.Dictionary;
                string ItemName = Item[owner.GetComponent<kichen>()._cleateItemID].baseData.ID;

                UI.SetText("素材を消費してアイテムを生産しますか？");
                UI.SetTable(new Vector2(1, 2));
                UI.SetLeftTopPos(new Vector2(-500, -100));
                UI.CreateButton();
                UI.SetButtonText(0, "はい");
                UI.SetButtonOnClick(0, () =>
                {
                    UI.DeleteButton();
                    //アイテムの生成と素材の消費
                    GameManager.Instance.ItemDataList.ItemsConsumption(owner.GetComponent<kichen>()._cleateItemID, owner.GetComponent<kichen>()._cleateNum, owner.GetComponent<kichen>()._toPouch);

                    var confUI = owner.ItemIconList[(int)IconType.Confirmation];
                    confUI.SetTable(new Vector2(1, 1));

                    confUI.SetText("調理完了");

                    confUI.SetLeftTopPos(new Vector2(-200, -100));
                    confUI.CreateButton();

                    confUI.SetButtonText(0, "OK");
                    confUI.SetButtonOnClick(0, () => { owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); owner.ChangeState<ItemSelectMode>(); });
                });
                UI.SetButtonText(1, "いいえ");
                UI.SetButtonOnClick(1, () => { owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); owner.ChangeState<ItemSelectMode>(); });
            }
            else
            {
                UI.SetText("すでにポーチに最大数所持しています");
                UI.SetTable(new Vector2(1, 1));
                UI.SetLeftTopPos(new Vector2(-200, -100));
                UI.CreateButton();
                UI.SetButtonText(0, "OK");
                UI.SetButtonOnClick(0, () => { owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); owner.ChangeState<ItemSelectMode>(); });
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Confirmation].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(owner.GetComponent<kichen>().countObj);
            owner.GetComponent<kichen>()._materialExplanation.Delete(owner);
            owner.ItemIconList[(int)IconType.ItemSelect].DeleteButton();
            owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.Confirmation].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<ItemSelectMode>();
        }
    }
}

