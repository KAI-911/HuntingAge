using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class Blacksmith : UIBase
{
    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _blacksmithChecker;
    //ボタンの色設定
    [SerializeField] Color _cantColor;
    //強化する武器のID
    string _createWeaponID;
    //武器を製造するか強化するか
    enum Mode { create, Enhancement }
    enum IconType
    {
        ModeSelect,
        TypeSelect,
        ProductSelect,
        Confirmation,
        MaterialList
    }

    private int productionWeaponType;
    private enum WeaponType
    {
        Axe = 1,
        Spear,
        Bow
    }
    void Start()
    {
        ItemIconList[(int)IconType.ModeSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.ProductSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        ItemIconList[(int)IconType.MaterialList].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }

    bool Check(WeaponData _ID, bool _modeIsProduction)
    {
        WeaponData data = _ID;

        if (_modeIsProduction)
        {
            for (int i = 0; i < data.ProductionNeedMaterialLst.Count; i++)
            {
                string needID = data.ProductionNeedMaterialLst[i].materialID;
                int needRequiredCount = data.ProductionNeedMaterialLst[i].requiredCount;

                var _material = GameManager.Instance.MaterialDataList;
                if (!(_material.Dictionary.ContainsKey(needID))) return false;
                int _num = _material.Dictionary[needID].BoxHoldNumber + _material.Dictionary[needID].PoachHoldNumber;
                if (_num < needRequiredCount) return false;
            }
        }
        else
        {
            for (int i = 0; i < data.EnhancementNeedMaterialLst.Count; i++)
            {
                string needID = data.EnhancementNeedMaterialLst[i].materialID;
                int needRequiredCount = data.EnhancementNeedMaterialLst[i].requiredCount;

                var _material = GameManager.Instance.MaterialDataList;
                if (!(_material.Dictionary.ContainsKey(needID))) return false;
                int _num = _material.Dictionary[needID].BoxHoldNumber + _material.Dictionary[needID].PoachHoldNumber;
                if (_num < needRequiredCount) return false;
            }
        }

        return true;
    }

    [Serializable]
    public class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_Close_OnEnter");
            UISoundManager.Instance._player.IsAction = true;
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log("Blacksmith_Close_OnProceed");
            //近くに来ている && 決定ボタンを押している && キャンバスがactiveでない
            if (owner.GetComponent<Blacksmith>()._blacksmithChecker.TriggerHit && UISoundManager.Instance._player.IsAction)
            {
                UISoundManager.Instance._player.IsAction = false;
                owner.ChangeState<ChoiceMode>();
            }
        }
    }

    public class ChoiceMode : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            owner.ItemIconList[(int)IconType.ProductSelect].DeleteButton();
            owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
            owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();

            Debug.Log("Blacksmith_TypeSelectMode_OnEnter");
            var UI = owner.ItemIconList[(int)IconType.ModeSelect];
            UI.SetText("鍛冶場：武器");
            UI.SetLeftTopPos(new Vector2(-600, 200));
            UI.SetTable(new Vector2(2, 1));
            UI.CreateButton();

            UI.SetButtonText(0, "製造");
            UI.SetButtonOnClick(0, () => owner.ChangeState<ProductionSelect>());

            UI.SetButtonText(1, "強化");
            UI.SetButtonOnClick(1, () => owner.ChangeState<EnhancementSelect>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ModeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.ModeSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ModeSelect].DeleteButton();
            UISoundManager.Instance._player.IsAction = true;
            owner.ChangeState<Close>();
        }
    }

    public class ProductionSelect : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("武器種選択");
            UI.SetLeftTopPos(new Vector2(-550,140));
            UI.SetTable(new Vector2(3, 1));
            var list = UI.CreateButton();
            for (int i = 0; i < 3/*斧・槍・弓*/; i++)
            {
                switch (i)
                {
                    case 0:
                        UI.SetButtonText(i, "斧系統");
                        UI.SetButtonOnClick(i, () =>
                             {
                                 owner.GetComponent<Blacksmith>().productionWeaponType = 0;
                                 owner.ChangeState<ProductionWeapon>();
                             });
                        break;
                    case 1:
                        UI.SetButtonText(i, "槍系統");
                        UI.SetButtonOnClick(i, () =>
                            {
                                owner.GetComponent<Blacksmith>().productionWeaponType = 1;
                                owner.ChangeState<ProductionWeapon>();
                            });
                        break;
                    case 2:
                        UI.SetButtonText(i, "弓系統");
                        UI.SetButtonOnClick(i, () =>
                            {
                                owner.GetComponent<Blacksmith>().productionWeaponType = 2;
                                owner.ChangeState<ProductionWeapon>();
                            });
                        break;
                }
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.TypeSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            owner.ChangeState<ChoiceMode>();
        }
    }

    public class ProductionWeapon : UIStateBase
    {
        List<WeaponData> _CreatableWeapon = new List<WeaponData>();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_ProductionWeaponMode_OnEnter");

            var ButtonUI = owner.ItemIconList[(int)IconType.ProductSelect];

            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
            foreach (var item in Weapon)
            {
                if ((item.Value.CreatableLevel <= GameManager.Instance.VillageData.BlacksmithLevel)
                    && ((int)item.Value.WeaponType == owner.GetComponent<Blacksmith>().productionWeaponType))
                {
                    _CreatableWeapon.Add(item.Value);
                }
            }

            ButtonUI.SetText("武器製造");
            ButtonUI.SetLeftTopPos(new Vector2(-500, 80));
            ButtonUI.SetTable(new Vector2(_CreatableWeapon.Count, 1));
            ButtonUI.CreateButton();

            for (int i = 0; i < _CreatableWeapon.Count; i++)
            {
                int num = i;
                ButtonUI.SetButtonText(i, _CreatableWeapon[i].Name);
                if (!owner.GetComponent<Blacksmith>().Check(_CreatableWeapon[i], true))
                {
                    var image = ButtonUI.Buttons[num].GetComponent<Image>();
                    image.color = owner.GetComponent<Blacksmith>()._cantColor;
                    ButtonUI.SetButtonOnClick(i, () => { });
                }
                else
                {
                    ButtonUI.SetButtonOnClick(i, () =>
                    {
                        owner.GetComponent<Blacksmith>()._createWeaponID = _CreatableWeapon[num].ID;
                        owner.ChangeState<Confirmation>();
                    });
                }

            }
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.ProductSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            int buttonCount = owner.ItemIconList[(int)IconType.ProductSelect].CurrentNunber;

            var ListUI = owner.ItemIconList[(int)IconType.MaterialList];
            var data = _CreatableWeapon[buttonCount].ProductionNeedMaterialLst;
            ListUI.SetText("必要素材／素材所持数");
            ListUI.SetTable(new Vector2(data.Count, 1));
            ListUI.SetLeftTopPos(new Vector2(100, 200));
            ListUI.CreateButton();
            for (int i = 0; i < data.Count; i++)
            {
                var materialID = data[i].materialID;
                var material = GameManager.Instance.MaterialDataList.Dictionary[materialID];
                string text1 = string.Format("{0,3:d}", data[i].requiredCount.ToString());
                string text2 = string.Format("{0,4:d}", (material.BoxHoldNumber + material.PoachHoldNumber).ToString());
                Debug.Log(text1);
                Debug.Log(text2);
                ListUI.SetButtonText(i, "　　" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
            }

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.ProductSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();
            owner.ItemIconList[(int)IconType.ProductSelect].DeleteButton();
            Debug.Log("modoru");
            owner.ChangeState<ProductionSelect>();
        }
    }
    public class EnhancementSelect : UIStateBase
    {
        List<WeaponData> _TmpWeapon = new List<WeaponData>();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            //ボタンの追加
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("武器強化先");

            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
            foreach (var item in Weapon)
            {
                if (item.Value.BoxPossession
                    && item.Value.EnhancementID != "empty") _TmpWeapon.Add(item.Value);
            }

            UI.SetTable(new Vector2(_TmpWeapon.Count, 1));
            UI.SetLeftTopPos(new Vector2(-550, 140));
            UI.CreateButton();

            if (_TmpWeapon.Count <= 0)
            {
                var confUI = owner.ItemIconList[(int)IconType.Confirmation];
                confUI.SetText("強化できる武器がありません");
                confUI.SetTable(new Vector2(1, 1));
                confUI.SetLeftTopPos(new Vector2(-200, -100));
                confUI.CreateButton();
                confUI.SetButtonText(0, "OK");
                confUI.SetButtonOnClick(0, () =>
                {
                    owner.ChangeState<ChoiceMode>();
                });
            }
            else
            {
                for (int i = 0; i < _TmpWeapon.Count; i++)
                {
                    int num = i;
                    string enhancementID = _TmpWeapon[num].EnhancementID;
                    var name = GameManager.Instance.WeaponDataList; int index = name.Keys.FindIndex(n => n.StartsWith(enhancementID));
                    Debug.Log(name.Values[index].Name);
                    UI.SetButtonText(i, name.Values[index].Name);
                    if (!owner.GetComponent<Blacksmith>().Check(name.Values[index], false))
                    {
                        var image = UI.Buttons[num].GetComponent<Image>();
                        image.color = owner.GetComponent<Blacksmith>()._cantColor;
                        UI.SetButtonOnClick(i, () => { });
                    }
                    UI.SetButtonOnClick(i, () =>
                    {
                        owner.GetComponent<Blacksmith>()._createWeaponID = _TmpWeapon[num].ID;
                        owner.ChangeState<Confirmation>();
                    });
                }
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            if (_TmpWeapon.Count > 0)
            {
                owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
                int buttonCount = owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber;
                var ListUI = owner.ItemIconList[(int)IconType.MaterialList];
                var data = _TmpWeapon[buttonCount].EnhancementNeedMaterialLst;
                ListUI.SetText("必要素材／素材所持数");
                ListUI.SetTable(new Vector2(data.Count, 1));
                ListUI.SetLeftTopPos(new Vector2(100, 200));
                ListUI.CreateButton();
                for (int i = 0; i < data.Count; i++)
                {
                    var materialID = data[i].materialID;
                    var material = GameManager.Instance.MaterialDataList.Dictionary[materialID];
                    string text1 = string.Format("{0,3:d}", data[i].requiredCount.ToString());
                    string text2 = string.Format("{0,4:d}", (material.BoxHoldNumber + material.PoachHoldNumber).ToString());
                    Debug.Log(text1);
                    Debug.Log(text2);
                    ListUI.SetButtonText(i, "　　" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
                }
            }
            else
            {
                owner.ItemIconList[(int)IconType.Confirmation].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            if (_TmpWeapon.Count > 0) owner.ItemIconList[(int)IconType.TypeSelect].CurrentButtonInvoke();
            else owner.ItemIconList[(int)IconType.Confirmation].CurrentButtonInvoke();
            UISoundManager.Instance.PlayDecisionSE();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            Debug.Log("modoru");
            owner.ChangeState<ChoiceMode>();
        }
    }

    public class Confirmation : UIStateBase
    {
        private UIStateBase _prevState;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log(1111);
            //モード選択画面
            _prevState = prevState;
            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
            string WeaponName = Weapon[owner.GetComponent<Blacksmith>()._createWeaponID].ID;
            var UI = owner.ItemIconList[(int)IconType.Confirmation];

            Debug.Log(11111);
            if (prevState.GetType() == typeof(EnhancementSelect)) UI.SetText("素材を消費して武器を強化しますか？");
            else if (prevState.GetType() == typeof(ProductionWeapon)) UI.SetText("素材を消費して武器を製造しますか？");

            Debug.Log(111111);
            UI.SetTable(new Vector2(1, 2));
            UI.CreateButton();
            UI.SetButtonText(0, "はい");
            UI.SetButtonOnClick(0, () =>
             {
                 UI.DeleteButton();
                 bool count = false;
                 if (prevState.GetType() == typeof(EnhancementSelect)) count = GameManager.Instance.WeaponDataList.Enhancement(WeaponName, true);
                 else if (prevState.GetType() == typeof(ProductionWeapon)) count = GameManager.Instance.WeaponDataList.Production(WeaponName, true);

                 var confUI = owner.ItemIconList[(int)IconType.Confirmation];
                 confUI.SetTable(new Vector2(1, 1));

                 if (!count) confUI.SetText("すでに所持しています");
                 else
                 {
                     if (prevState.GetType() == typeof(EnhancementSelect)) confUI.SetText("強化完了");
                     else if (prevState.GetType() == typeof(ProductionWeapon)) confUI.SetText("製造完了");
                 }

                 confUI.SetLeftTopPos(new Vector2(-200, -100));
                 confUI.CreateButton();

                 confUI.SetButtonText(0, "OK");
                 confUI.SetButtonOnClick(0, () => { owner.ChangeState<ChoiceMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
             });
            UI.SetButtonText(1, "いいえ");
            UI.SetButtonOnClick(1, () => { owner.ChangeState<ChoiceMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Confirmation].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            owner.ItemIconList[(int)IconType.ProductSelect].DeleteButton();
            owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
            owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.Confirmation].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<ChoiceMode>();
        }
    }
}