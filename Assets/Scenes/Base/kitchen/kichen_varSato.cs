using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class kichen_varSato : UIBase
{
    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _kichenChecker;
    //制作するアイテムのID
    [SerializeField] string _cleateItemID;
    [SerializeField] GameObject _materialIDandNumber;


    [SerializeField] UIPresetDataList _uIPresetDataList;
    [SerializeField] Color _cantColor;

    //ボックスかポーチどっちに送るか
    [SerializeField] bool _toPouch;
    //何個作るか
    int cleateNum;

    private Count _count;
    private ItemExplanation _itemExplanation;

    enum IconType
    {
        Select,
        Confirmation,
        Needmaterial
    }


    void Start()
    {
        _count = new Count();
        _itemExplanation = new ItemExplanation();
        ItemIconList[(int)IconType.Select].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Select].SetLeftTopPos(new Vector2(-600, 200));
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        ItemIconList[(int)IconType.Needmaterial].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Needmaterial].SetLeftTopPos(new Vector2(100, 200));

        _currentState = new Close();
        _currentState.OnEnter(this, null);
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
            if (owner.GetComponent<kichen_varSato>()._kichenChecker.TriggerHit && UISoundManager.Instance._player.IsAction)
            {
                owner.ChangeState<BoxorPouch>();
            }
        }
    }

    public class BoxorPouch : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            UISoundManager.Instance._player.IsAction = false;
            owner.GetComponent<kichen_varSato>()._itemExplanation.Delete();
            var UI = owner.ItemIconList[(int)IconType.Select];
            UI.SetText("アイテムをどこに送りますか？");
            UI.SetTable(new Vector2(2, 1));
            UI.CreateButton();
            UI.SetButtonText(0, "ボックスへ");
            UI.SetButtonOnClick(0, () => { owner.GetComponent<kichen_varSato>()._toPouch = false; owner.ChangeState<SelectMode>(); });

            UI.SetButtonText(1, "ポーチへ");
            UI.SetButtonOnClick(1, () => { owner.GetComponent<kichen_varSato>()._toPouch = true; owner.ChangeState<SelectMode>(); });
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Select].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.Select].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.Select].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
    }

    public class SelectMode : UIStateBase
    {
        int _memoryNum;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(BoxorPouch))
            {
                _memoryNum = -1;
                //製造するアイテムのリストを作成ーーーーーーーーーーーーー
                var UI = owner.ItemIconList[(int)IconType.Select];
                UI.SetText("作るアイテム");
                List<ItemData> _createItem = new List<ItemData>();
                foreach (var item in GameManager.Instance.ItemDataList._itemSaveData.Dictionary)
                {
                    if (item.Value.CreatableLevel <= GameManager.Instance.VillageData.KitchenLevel)
                    {
                        for (int i = 0; i < item.Value.NeedMaterialLst.Count; i++)
                        {
                            if (!GameManager.Instance.MaterialDataList._materialSaveData.dictionary.ContainsKey(item.Value.NeedMaterialLst[i].materialID)) continue;
                        }
                        _createItem.Add(item.Value);
                    }
                }
                UI.SetTable(new Vector2(_createItem.Count, 1));
                UI.CreateButton();


                //ボタンが押されたら何個作るかのカウント用のオブジェクトを作成
                for (int i = 0; i < _createItem.Count; i++)
                {
                    int numi = i;
                    UI.SetButtonText(numi, _createItem[numi].baseData.Name);
                    UI.SetButtonOnClick(numi, () =>
                    {
                        //もし一つ以上制作できるなら個数選択に移行
                        if (owner.GetComponent<kichen_varSato>()._count.Check(owner, owner.GetComponent<kichen_varSato>()._toPouch))
                        {
                            owner.ChangeState<CountMode>();
                        }
                    });
                    if (!owner.GetComponent<kichen_varSato>()._count.Check(owner, owner.GetComponent<kichen_varSato>()._toPouch, _createItem[numi].baseData.ID))
                    {
                        var image = UI.Buttons[numi].GetComponent<Image>();
                        image.color = owner.GetComponent<kichen_varSato>()._cantColor;
                    }
                }
            }
            else if (prevState.GetType() == typeof(CleateMode))
            {
                var UI = owner.ItemIconList[(int)IconType.Select];
                foreach (var item in UI.Buttons)
                {
                    var text = item.GetComponentInChildren<Text>();
                    foreach (var data in GameManager.Instance.ItemDataList._itemSaveData.Dictionary.Values)
                    {
                        if (!owner.GetComponent<kichen_varSato>()._count.Check(owner, owner.GetComponent<kichen_varSato>()._toPouch, data.baseData.ID))
                        {
                            var image = item.GetComponent<Image>();
                            image.color = owner.GetComponent<kichen_varSato>()._cantColor;
                        }
                    }
                }
            }
            else if (prevState.GetType() == typeof(CountMode))
            {
                owner.GetComponent<kichen_varSato>()._count.Delete();
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("アイテム選択モード");

            var current = owner.ItemIconList[(int)IconType.Select].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            //カレントナンバーが更新されたら_cleateItemIDも更新する
            if (_memoryNum != current)
            {
                _memoryNum = current;
                var name = owner.ItemIconList[(int)IconType.Select].Buttons[current].GetComponentInChildren<Text>();
                foreach (var item in GameManager.Instance.ItemDataList._itemSaveData.Dictionary.Values)
                {
                    if (item.baseData.Name != name.text) continue;
                    owner.GetComponent<kichen_varSato>()._cleateItemID = item.baseData.ID;
                    owner.GetComponent<kichen_varSato>()._itemExplanation.Set(item, owner);

                }
                ChangeNeedMatrialButtons(owner);
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            //ボックスかポーチかの選択に戻るならボタンを消す
            if (nextState.GetType() == typeof(BoxorPouch))
            {
                owner.ItemIconList[(int)IconType.Select].DeleteButton();
                owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
                owner.ItemIconList[(int)IconType.Needmaterial].DeleteButton();
            }
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.Select].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<BoxorPouch>();
        }

        /// <summary>
        /// 必要素材などを表示
        /// ボタンを再生成するため頻繁には呼ばないように
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="_namespace"></param>
        private void ChangeNeedMatrialButtons(UIBase owner, int _namespace = 3)
        {
            //必要素材のリストを作成ーーーーーーーーーーーーー
            var list = owner.ItemIconList[(int)IconType.Needmaterial];
            list.DeleteButton();
            list.SetText("素材名　　必要素材／素材所持数");
            //制作するアイテムに必要な素材を調べる
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _itemDataDic = GameManager.Instance.ItemDataList._itemSaveData.Dictionary;

            list.SetTable(new Vector2(_itemDataDic[id].NeedMaterialLst.Count, 1));
            list.CreateButton();
            for (int i = 0; i < list.Buttons.Count; i++)
            {
                string materialID = _itemDataDic[id].NeedMaterialLst[i].materialID;
                string name = GameManager.Instance.MaterialDataList._materialSaveData.dictionary[materialID].Name;
                //数値の始まりを同じにするために_namespace分に文字数をそろえる
                for (int j = name.Length; j < _namespace; j++) name += "　";

                string needNum = string.Format("{0,2:d}", _itemDataDic[id].NeedMaterialLst[i].requiredCount);
                string holdNum = string.Format("{0,4:d}", GameManager.Instance.MaterialDataList._materialSaveData.dictionary[materialID].PoachHoldNumber + GameManager.Instance.MaterialDataList._materialSaveData.dictionary[materialID].BoxHoldNumber);

                list.SetButtonText(i, "　" + name + Data.Convert.HanToZenConvert(needNum + "/" + holdNum));
            }
        }
    }
    public class CountMode : UIStateBase
    {

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            owner.GetComponent<kichen_varSato>()._count.Init(owner, owner.GetComponent<kichen_varSato>()._toPouch);
        }
        public override void OnUpdate(UIBase owner)
        {
            Debug.Log("個数選択モード");
            if (owner.GetComponent<kichen_varSato>()._count.CountObject == null)
            {
                owner.ChangeState<SelectMode>();
            }
            owner.GetComponent<kichen_varSato>()._count.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            //必要素材の個数の更新
            UpdateNeedMatrialText(owner);
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.GetComponent<kichen_varSato>()._count.Delete();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ChangeState<CleateMode>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<SelectMode>();
        }
        private void UpdateNeedMatrialText(UIBase owner, int _namespace = 3)
        {
            //必要素材のリストを作成ーーーーーーーーーーーーー
            var list = owner.ItemIconList[(int)IconType.Needmaterial];
            //制作するアイテムに必要な素材を調べる
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _itemDataDic = GameManager.Instance.ItemDataList._itemSaveData.Dictionary;

            for (int i = 0; i < list.Buttons.Count; i++)
            {
                string materialID = _itemDataDic[id].NeedMaterialLst[i].materialID;
                string name = GameManager.Instance.MaterialDataList._materialSaveData.dictionary[materialID].Name;
                //数値の始まりを同じにするために_namespace分に文字数をそろえる
                for (int j = name.Length; j < _namespace; j++) name += "　";

                string needNum = string.Format("{0,2:d}", _itemDataDic[id].NeedMaterialLst[i].requiredCount * owner.GetComponent<kichen_varSato>()._count.Now);
                string holdNum = string.Format("{0,4:d}", GameManager.Instance.MaterialDataList._materialSaveData.dictionary[materialID].PoachHoldNumber + GameManager.Instance.MaterialDataList._materialSaveData.dictionary[materialID].BoxHoldNumber);

                list.SetButtonText(i, "　" + name + Data.Convert.HanToZenConvert(needNum + "/" + holdNum));
            }
        }

    }

    public class CleateMode : UIStateBase
    {
        private ItemIcon confimationIcon;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            confimationIcon = owner.ItemIconList[(int)IconType.Confirmation];
            confimationIcon.SetText("素材を消費してアイテムを作りますか？");
            confimationIcon.SetTable(new Vector2(1, 2));
            confimationIcon.CreateButton();

            confimationIcon.SetButtonText(0, "はい");
            confimationIcon.SetButtonOnClick(0, () =>
            {
                string resultStr = "";
                //生産するアイテムのID
                string createItemID = owner.GetComponent<kichen_varSato>()._cleateItemID;
                //アイテム生産
                if (owner.GetComponent<kichen_varSato>()._toPouch)
                {
                    int number = owner.GetComponent<kichen_varSato>()._count.Now;
                    GameManager.Instance.ItemDataList.GetToPoach(createItemID, number, 0/*仮*/);
                    var list = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[createItemID].NeedMaterialLst;
                    var material = GameManager.Instance.MaterialDataList._materialSaveData.dictionary;
                    foreach (var item in list)
                    {
                        int cost = item.requiredCount * number;
                        var data = material[item.materialID];
                        //先にポーチから必要数を消費する
                        if (data.PoachHoldNumber > 0)
                        {
                            if (data.PoachHoldNumber < cost)
                            {
                                cost = cost - data.PoachHoldNumber;
                                data.PoachHoldNumber = 0;
                            }
                            else
                            {
                                data.PoachHoldNumber = data.PoachHoldNumber - cost;
                                cost = 0;
                            }
                        }
                        //ボックスから消費する
                        data.BoxHoldNumber -= cost;
                        material[item.materialID] = data;
                    }
                    resultStr = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[createItemID].baseData.Name + "を"
                                + Data.Convert.HanToZenConvert(number.ToString()) + "個生産しました";


                }
                else
                {
                    int number = owner.GetComponent<kichen_varSato>()._count.Now;
                    GameManager.Instance.ItemDataList.GetToBox(createItemID, number, 0/*仮*/);
                    var list = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[createItemID].NeedMaterialLst;
                    var material = GameManager.Instance.MaterialDataList._materialSaveData.dictionary;
                    foreach (var item in list)
                    {
                        int cost = item.requiredCount * number;
                        var data = material[item.materialID];
                        //先にポーチから必要数を消費する
                        if (data.PoachHoldNumber > 0)
                        {
                            if (data.PoachHoldNumber < cost)
                            {
                                cost = cost - data.PoachHoldNumber;
                                data.PoachHoldNumber = 0;
                            }
                            else
                            {
                                data.PoachHoldNumber = data.PoachHoldNumber - cost;
                                cost = 0;
                            }
                        }
                        //ボックスから消費する
                        data.BoxHoldNumber -= cost;
                        material[item.materialID] = data;
                    }
                    resultStr = GameManager.Instance.ItemDataList._itemSaveData.Dictionary[createItemID].baseData.Name + "を"
                                + Data.Convert.HanToZenConvert(number.ToString()) + "個生産しました";

                }

                confimationIcon.DeleteButton();
                confimationIcon.SetText(resultStr);
                confimationIcon.SetTable(new Vector2(1, 1));
                confimationIcon.CreateButton();

                confimationIcon.SetButtonText(0, "OK");
                confimationIcon.SetButtonOnClick(0, () => owner.ChangeState<SelectMode>());
            });

            confimationIcon.SetButtonText(1, "いいえ");
            confimationIcon.SetButtonOnClick(1, () => owner.ChangeState<CountMode>());
        }
        public override void OnUpdate(UIBase owner)
        {
            confimationIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            confimationIcon.DeleteButton();
            if (nextState.GetType() == typeof(SelectMode))
            {
                owner.GetComponent<kichen_varSato>()._count.Delete();
            }
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            confimationIcon.CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<CountMode>();
        }
    }

    //カウントするためのクラス
    public class Count
    {
        private GameObject countObject;
        private int min, max, now;
        private bool lockflg;

        public int Min { get => min; }
        public int Max { get => max; }
        public int Now { get => now; }
        public GameObject CountObject { get => countObject; }

        /// <summary>
        /// カウント用のUIを作成から最大値の設定も行う
        /// </summary>
        /// <param name="owner"></param>
        public void Init(UIBase owner, bool toPoach)
        {
            if (!Check(owner, toPoach))
            {
                Debug.LogError("必要個数が足りない");
                return;
            }

            //最大値最小値の初期化
            now = 1;
            min = 1;
            max = 9999;
            //最大値の設定
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _ItemDic = GameManager.Instance.ItemDataList._itemSaveData.Dictionary;
            var _materialDataDic = GameManager.Instance.MaterialDataList._materialSaveData.dictionary;
            if (toPoach)
            {
                max = _ItemDic[id].baseData.PoachStackNumber - _ItemDic[id].baseData.PoachHoldNumber;
            }
            else
            {
                max = _ItemDic[id].baseData.BoxStackNumber - _ItemDic[id].baseData.BoxHoldNumber;
            }
            foreach (var item in _ItemDic[id].NeedMaterialLst)
            {
                var materialNum = _materialDataDic[item.materialID].BoxHoldNumber + _materialDataDic[item.materialID].PoachHoldNumber;
                if (item.requiredCount <= 0) Debug.LogError("必要数が0以下です。");
                if ((materialNum / item.requiredCount) < max)
                {
                    max = materialNum / item.requiredCount;
                }
            }

            //カウント用のUIを制作、位置設定
            if (countObject == null)
            {
                countObject = Instantiate(Resources.Load("UI/Count"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
                owner.ItemIconList[(int)IconType.Select].AdjustmentImage(countObject.GetComponent<RectTransform>());
            }
        }
        /// <summary>
        /// カウント用のUIを削除
        /// </summary>
        public void Delete()
        {
            if (countObject == null) return;
            Destroy(countObject);
        }
        /// <summary>
        /// カウントアップダウン
        /// </summary>
        /// <param name="vec"></param>
        public void Select(Vector2 vec)
        {
            if (countObject == null) return;

            if (vec.sqrMagnitude > 0)
            {
                if (lockflg == false)
                {
                    if (vec.y > 0) now++;
                    else now--;
                    now = Mathf.Clamp(now, min, max);
                    lockflg = true;

                    countObject.GetComponentInChildren<Text>().text = now.ToString();
                    UISoundManager.Instance.PlayCursorSE();
                }
            }
            else
            {
                lockflg = false;
            }

        }

        /// <summary>
        /// 制作可能ならtureを返す
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="toPoach"></param>
        /// <param name="id">特に指定しない場合は現在選択しているボタンのアイテムのIDを使う</param>
        /// <returns></returns>
        public bool Check(UIBase owner, bool toPoach, string id = " ")
        {
            //最大値の設定
            if (id == " ")
            {
                id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            }
            var _ItemDic = GameManager.Instance.ItemDataList._itemSaveData.Dictionary;
            var _materialDataDic = GameManager.Instance.MaterialDataList._materialSaveData.dictionary;
            //必要素材が足りているか確認
            foreach (var item in _ItemDic[id].NeedMaterialLst)
            {
                var materialNum = _materialDataDic[item.materialID].BoxHoldNumber + _materialDataDic[item.materialID].PoachHoldNumber;
                if (item.requiredCount <= 0) Debug.LogError("必要数が0以下です。");
                //制作出来ない
                if ((materialNum / item.requiredCount) <= 0) return false;
            }
            //制作したアイテムがもてるかの確認
            if (toPoach)
            {
                if (_ItemDic[id].baseData.PoachHoldNumber < 0)
                {
                    //未使用の枠があるか確認----------------------------------------------
                    List<int> vs = new List<int>();
                    var table = owner.GetComponent<kichen_varSato>()._uIPresetDataList.Dictionary["IP_ItemSelect"]._tableSize;
                    //全ての枠を確認
                    for (int i = 0; i < table.x * table.y; i++) vs.Add(i);
                    //使用している枠を削除していく
                    foreach (var item in GameManager.Instance.ItemDataList._itemSaveData.Dictionary.Values)
                    {
                        if (vs.Contains(item.baseData.PoachUINumber)) vs.Remove(item.baseData.PoachUINumber);
                    }
                    foreach (var item in GameManager.Instance.MaterialDataList._materialSaveData.dictionary.Values)
                    {
                        if (vs.Contains(item.PoachUINumber)) vs.Remove(item.PoachUINumber);
                    }
                    if (vs.Count <= 0) return false;
                }
                else if (_ItemDic[id].baseData.PoachHoldNumber == _ItemDic[id].baseData.PoachStackNumber)
                {
                    //最大まで持っているので生産できない
                    return false;
                }

            }
            else
            {
                if (_ItemDic[id].baseData.BoxHoldNumber <= 0)
                {
                    //未使用の枠があるか確認----------------------------------------------
                    List<int> vs = new List<int>();
                    var table = owner.GetComponent<kichen_varSato>()._uIPresetDataList.Dictionary["IB_ItemSelect"]._tableSize;
                    //全ての枠を確認
                    for (int i = 0; i < table.x * table.y; i++) vs.Add(i);
                    //使用している枠を削除していく
                    foreach (var item in GameManager.Instance.ItemDataList._itemSaveData.Dictionary.Values)
                    {
                        if (vs.Contains(item.baseData.PoachUINumber)) vs.Remove(item.baseData.PoachUINumber);
                    }
                    foreach (var item in GameManager.Instance.MaterialDataList._materialSaveData.dictionary.Values)
                    {
                        if (vs.Contains(item.PoachUINumber)) vs.Remove(item.PoachUINumber);
                    }
                    if (vs.Count <= 0) return false;
                }
                else if (_ItemDic[id].baseData.BoxHoldNumber == _ItemDic[id].baseData.BoxStackNumber)
                {
                    //最大まで持っているので生産できない
                    return false;
                }

            }
            return true;
        }
    }

    //アイコンや名前、効果（フレーバーテキスト）を管理するクラス
    public class ItemExplanation
    {
        private GameObject Back;
        private GameObject Icon;
        private GameObject name;
        private GameObject effect;
        private Vector2 _base;
        public void Set(ItemData itemData, UIBase owner)
        {
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
            Icon.GetComponent<Image>().sprite = Resources.Load<Sprite>(itemData.baseData.IconName);

            //名前
            var NameRet = name.GetComponent<RectTransform>();
            NameRet.pivot = new Vector2(0, 0);
            NameRet.sizeDelta = new Vector2(300, 50);
            NameRet.anchoredPosition = _base + new Vector2((iconsize.x + padding) / 2, padding / 2);
            name.GetComponent<Text>().text = itemData.baseData.Name;
            name.GetComponent<Text>().color = new Color(1, 1, 1);

            //説明文
            var EffectRet = effect.GetComponent<RectTransform>();
            EffectRet.pivot = new Vector2(0, 1);
            EffectRet.sizeDelta = new Vector2(300, 50);
            EffectRet.anchoredPosition = _base + new Vector2((iconsize.x+padding) / 2, padding / 2);
            effect.GetComponent<Text>().text = itemData.FlavorText;
            effect.GetComponent<Text>().color = new Color(1, 1, 1);

        }

        public void Delete()
        {
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

}

