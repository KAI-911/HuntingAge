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
        ItemIconList[(int)IconType.Select].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        //↓要調整!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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
                foreach (var item in GameManager.Instance.ItemDataList.Dictionary)
                {
                    if (item.Value.CreatableLevel < GameManager.Instance.VillageData.KitchenLevel)
                    {
                        for (int i = 0; i < item.Value.NeedMaterialLst.Count; i++)
                        {
                            if (!GameManager.Instance.MaterialDataList.Dictionary.ContainsKey(item.Value.NeedMaterialLst[i].materialID)) continue;
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
                        if (owner.GetComponent<kichen_varSato>()._count.Check(owner)) owner.ChangeState<CountMode>();
                    });
                }
            }

        }
        public override void OnUpdate(UIBase owner)
        {
            var current = owner.ItemIconList[(int)IconType.Select].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            //カレントナンバーが更新されたら_cleateItemIDも更新する
            if (_memoryNum != current)
            {
                _memoryNum = current;
                var name = owner.ItemIconList[(int)IconType.Select].Buttons[current].GetComponentInChildren<Text>();
                foreach (var item in GameManager.Instance.ItemDataList.Dictionary.Values)
                {
                    if (item.baseData.Name != name.text) continue;
                    owner.GetComponent<kichen_varSato>()._cleateItemID = item.baseData.ID;
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
            var _itemDataDic = GameManager.Instance.ItemDataList.Dictionary;

            list.SetTable(new Vector2(_itemDataDic[id].NeedMaterialLst.Count, 1));
            list.CreateButton();
            for (int i = 0; i < list.Buttons.Count; i++)
            {
                string materialID = _itemDataDic[id].NeedMaterialLst[i].materialID;
                string name = GameManager.Instance.MaterialDataList.Dictionary[materialID].Name;
                //数値の始まりを同じにするために_namespace分に文字数をそろえる
                for (int j = name.Length; j < _namespace; j++) name += "　";

                string needNum = string.Format("{0,3:d}", _itemDataDic[id].NeedMaterialLst[i].requiredCount);
                string holdNum = string.Format("{0,4:d}", GameManager.Instance.MaterialDataList.Dictionary[materialID].PoachHoldNumber + GameManager.Instance.MaterialDataList.Dictionary[materialID].BoxHoldNumber);

                list.SetButtonText(i,"　"+ name + Data.Convert.HanToZenConvert(needNum + "/" + holdNum));
            }
        }
    }
    public class CountMode : UIStateBase
    {

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            owner.GetComponent<kichen_varSato>()._count.Init(owner);
        }
        public override void OnUpdate(UIBase owner)
        {
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
            owner.ChangeState<CleateMode>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<SelectMode>();
        }
        private void UpdateNeedMatrialText(UIBase owner, int _namespace = 4)
        {
            //必要素材のリストを作成ーーーーーーーーーーーーー
            var list = owner.ItemIconList[(int)IconType.Needmaterial];
            //制作するアイテムに必要な素材を調べる
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _itemDataDic = GameManager.Instance.ItemDataList.Dictionary;

            for (int i = 0; i < list.Buttons.Count; i++)
            {
                string materialID = _itemDataDic[id].NeedMaterialLst[i].materialID;
                string name = GameManager.Instance.MaterialDataList.Dictionary[materialID].Name;
                //数値の始まりを同じにするために_namespace分に文字数をそろえる
                for (int j = name.Length; j < _namespace; j++) name += "　";

                string needNum = string.Format("{0,3:d}", _itemDataDic[id].NeedMaterialLst[i].requiredCount * owner.GetComponent<kichen_varSato>()._count.Now);
                string holdNum = string.Format("{0,4:d}", GameManager.Instance.MaterialDataList.Dictionary[materialID].PoachHoldNumber + GameManager.Instance.MaterialDataList.Dictionary[materialID].BoxHoldNumber);

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
                    var list = GameManager.Instance.ItemDataList.Dictionary[createItemID].NeedMaterialLst;
                    var materialValues = GameManager.Instance.MaterialDataList.Values;
                    var materialkeys = GameManager.Instance.MaterialDataList.Keys;
                    foreach (var item in list)
                    {
                        int cost = item.requiredCount * number;
                        int index = materialkeys.IndexOf(item.materialID);
                        var data = materialValues[index];
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
                        materialValues[index] = data;
                        GameManager.Instance.MaterialDataList.DesrializeDictionary();
                    }
                    resultStr = GameManager.Instance.ItemDataList.Dictionary[createItemID].baseData.Name + "を"
                                + Data.Convert.HanToZenConvert(number.ToString()) + "個生産しました";


                }
                else
                {
                    int number = owner.GetComponent<kichen_varSato>()._count.Now;
                    GameManager.Instance.ItemDataList.GetToBox(createItemID, number, 0/*仮*/);
                    var list = GameManager.Instance.ItemDataList.Dictionary[createItemID].NeedMaterialLst;
                    var materialValues = GameManager.Instance.MaterialDataList.Values;
                    var materialkeys = GameManager.Instance.MaterialDataList.Keys;
                    foreach (var item in list)
                    {
                        int cost = item.requiredCount * number;
                        int index = materialkeys.IndexOf(item.materialID);
                        var data = materialValues[index];
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
                        materialValues[index] = data;
                        GameManager.Instance.MaterialDataList.DesrializeDictionary();
                    }
                    resultStr = GameManager.Instance.ItemDataList.Dictionary[createItemID].baseData.Name + "を"
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

        /// <summary>
        /// カウント用のUIを作成から最大値の設定も行う
        /// </summary>
        /// <param name="owner"></param>
        public void Init(UIBase owner)
        {
            if (!Check(owner))
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
            var _ItemDic = GameManager.Instance.ItemDataList.Dictionary;
            var _materialDataDic = GameManager.Instance.MaterialDataList.Dictionary;
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
            countObject = Instantiate(Resources.Load("UI/Count"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            owner.ItemIconList[(int)IconType.Select].AdjustmentImage(countObject.GetComponent<RectTransform>());
        }
        /// <summary>
        /// カウント用のUIを削除
        /// </summary>
        public void Delete()
        {
            Destroy(countObject);
        }
        /// <summary>
        /// カウントアップダウン
        /// </summary>
        /// <param name="vec"></param>
        public void Select(Vector2 vec)
        {
            if (vec.sqrMagnitude > 0)
            {
                if (lockflg == false)
                {
                    if (vec.y > 0) now++;
                    else now--;
                    now = Mathf.Clamp(now, min, max);
                    lockflg = true;

                    countObject.GetComponentInChildren<Text>().text = now.ToString();
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
        /// <returns></returns>
        public bool Check(UIBase owner)
        {
            //最大値の設定
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _ItemDic = GameManager.Instance.ItemDataList.Dictionary;
            var _materialDataDic = GameManager.Instance.MaterialDataList.Dictionary;
            foreach (var item in _ItemDic[id].NeedMaterialLst)
            {
                var materialNum = _materialDataDic[item.materialID].BoxHoldNumber + _materialDataDic[item.materialID].PoachHoldNumber;
                if (item.requiredCount <= 0) Debug.LogError("必要数が0以下です。");
                //制作出来ない
                if ((materialNum / item.requiredCount) <= 0) return false;
            }
            return true;
        }
    }

    //アイコンや名前、効果（フレーバーテキスト）を管理するクラス
    public class ItemExplanation
    {
        private GameObject Icon;
        private GameObject name;
        private GameObject effect;

        public void Set(ItemData itemData, UIBase owner)
        {
            //アイコン
            if (Icon == null) Icon = Instantiate(Resources.Load("UI/Image3"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //要位置調整!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            Icon.GetComponent<Image>().sprite = Resources.Load<Sprite>(itemData.baseData.IconName);

            //名前
            if (name == null) name = Instantiate(Resources.Load("UI/Image3"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //要位置調整!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            name.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, 50);
            //name.GetComponent<Image>().sprite = Resources.Load<Sprite>("背景を変えたければここにパスを書く");
            name.GetComponentInChildren<Text>().text = itemData.baseData.Name;

            //説明文
            if (effect == null) effect = Instantiate(Resources.Load("UI/Image3"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //要位置調整!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            effect.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, 100);
            //name.GetComponent<Image>().sprite = Resources.Load<Sprite>("背景を変えたければここにパスを書く");
            effect.GetComponentInChildren<Text>().text = itemData.FlavorText;

        }

        public void Delete()
        {
            //アイコン
            if (Icon != null) Destroy(Icon);
            //名前
            if (name != null) Destroy(name);
            //説明文
            if (effect != null) Destroy(effect);
        }
    }

}

