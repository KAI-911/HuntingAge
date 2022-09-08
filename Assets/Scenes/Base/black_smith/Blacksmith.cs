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
    //強化する武器のID
    string _EnhancementWeaponID;

    enum IconType
    {
        TypeSelect,
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
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        ItemIconList[(int)IconType.MaterialList].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
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
                owner.ChangeState<TypeSelectMode>();
            }
        }
    }


    public class TypeSelectMode : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_TypeSelectMode_OnEnter");
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("鍛冶場：武器");
            UI.SetTable(new Vector2(2, 1));
            var list = UI.CreateButton();

            UI.SetButtonText(0, "製造");
            UI.SetButtonOnClick(0, () => owner.ChangeState<ProductionSelectMode>());

            UI.SetButtonText(1, "強化");
            UI.SetButtonOnClick(1, () => owner.ChangeState<EnhancementSelectMode>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log(owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber);
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            UISoundManager.Instance._player.IsAction = true;
            owner.ChangeState<Close>();
        }
    }

    public class ProductionSelectMode : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("武器製造");
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
                                 owner.ChangeState<ProductionWeaponMode>();
                             });
                        break;
                    case 1:
                        UI.SetButtonText(i, "槍系統");
                        UI.SetButtonOnClick(i, () =>
                            {
                                owner.GetComponent<Blacksmith>().productionWeaponType = 1;
                                owner.ChangeState<ProductionWeaponMode>();
                            });
                        break;
                    case 2:
                        UI.SetButtonText(i, "弓系統");
                        UI.SetButtonOnClick(i, () =>
                            {
                                owner.GetComponent<Blacksmith>().productionWeaponType = 2;
                                owner.ChangeState<ProductionWeaponMode>();
                            });
                        break;
                }
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<TypeSelectMode>();
        }
    }

    public class ProductionWeaponMode : UIStateBase
    {
        private bool ConfirmationSelect;
        List<WeaponData> _CreatableWeapon = new List<WeaponData>();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_ProductionWeaponMode_OnEnter");
            ConfirmationSelect = false;
            var ButtonUI = owner.ItemIconList[(int)IconType.TypeSelect];
            var confUI = owner.ItemIconList[(int)IconType.Confirmation];

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
            ButtonUI.SetTable(new Vector2(_CreatableWeapon.Count, 1));
            ButtonUI.CreateButton();

            for (int i = 0; i < _CreatableWeapon.Count; i++)
            {
                ButtonUI.SetButtonText(i, _CreatableWeapon[i].Name);
                int num = i;
                ButtonUI.SetButtonOnClick(i, () =>
                {
                    int count = GameManager.Instance.WeaponDataList.Production(_CreatableWeapon[num].ID);
                    ConfirmationSelect = true;

                    confUI.SetLeftTopPos(new Vector2(-300, -150));
                    confUI.SetTable(new Vector2(1, 1));
                    switch (count)
                    {
                        case 0:
                            confUI.SetText("すでに所持しています");
                            break;
                        case 1:
                            confUI.SetText("製造完了");
                            break;
                        case 2:
                            confUI.SetText("素材が足りません");
                            break;
                        default:
                            break;
                    }
                    confUI.CreateButton();
                    confUI.SetButtonText(0, "OK");

                    confUI.SetButtonOnClick(0, () => owner.ChangeState<ProductionSelectMode>());
                });
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            if (ConfirmationSelect) owner.ItemIconList[(int)IconType.Confirmation].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            else
            {
                owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
                int buttonCount = owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber;
                //if (currentNunber > 0)
                var ListUI = owner.ItemIconList[(int)IconType.MaterialList];
                var data = _CreatableWeapon[buttonCount].ProductionNeedMaterialLst;
                ListUI.SetText("必要素材/素材所持数");
                ListUI.SetTable(new Vector2(data.Count, 1));
                ListUI.SetLeftTopPos(new Vector2(100, 200));
                ListUI.CreateButton();
                for (int i = 0; i < data.Count; i++)
                {
                    var materialID = data[i].materialID;
                    var material = GameManager.Instance.MaterialDataList.Dictionary[materialID];
                    //string text1 = string.Format("{0,4:d}", data[i].requiredCount);
                    string text1;
                    if (data[i].requiredCount >= 10) text1 = data[i].requiredCount.ToString();
                    else text1 = "  " + data[i].requiredCount.ToString();

                    string text2 = (material.BoxHoldNumber + material.PoachHoldNumber).ToString();

                    

                    ListUI.SetButtonText(i,"　　　" + material.Name + text1 + "/" + text2, TextAnchor.MiddleLeft);
                }
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
            owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            if (ConfirmationSelect) owner.ItemIconList[(int)IconType.Confirmation].CurrentButtonInvoke();
            else owner.ItemIconList[(int)IconType.TypeSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<ProductionSelectMode>();
        }
    }

    public class EnhancementSelectMode : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            //ボタンの追加
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("武器強化先");

            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
            List<WeaponData> _TmpWeapon = new List<WeaponData>();
            foreach (var item in Weapon)
            {
                if (item.Value.BoxPossession
                    && item.Value.EnhancementID != "empty") _TmpWeapon.Add(item.Value);
            }
            UI.SetTable(new Vector2(_TmpWeapon.Count, 1));
            UI.CreateButton();

            for (int i = 0; i < _TmpWeapon.Count; i++)
            {
                int num = i;
                string enhancementID = _TmpWeapon[num].EnhancementID;
                var name = GameManager.Instance.WeaponDataList; int index = name.Keys.IndexOf(enhancementID);
                UI.SetButtonText(i, name.Values[index].Name);
                UI.SetButtonOnClick(i, () =>
                {
                    owner.GetComponent<Blacksmith>()._EnhancementWeaponID = _TmpWeapon[num].ID;
                    owner.ChangeState<EnhancementWeaponMode>();
                });
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<TypeSelectMode>();
        }
    }

    public class EnhancementWeaponMode : UIStateBase
    {
        private bool ConfirmationSelect;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("kyoukakakuninndayo");
            //モード選択画面
            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
            string WeaponName = Weapon[owner.GetComponent<Blacksmith>()._EnhancementWeaponID].ID;
            var UI = owner.ItemIconList[(int)IconType.Confirmation];

            ConfirmationSelect = true;
            UI.SetText("素材を消費して武器を強化しますか？");
            UI.SetTable(new Vector2(1, 2));
            UI.CreateButton();
            UI.SetButtonText(0, "はい");
            UI.SetButtonOnClick(0, () =>
             {
                 UI.DeleteButton();
                 int count = GameManager.Instance.WeaponDataList.Enhancement(WeaponName);
                 ConfirmationSelect = true;

                 var confUI = owner.ItemIconList[(int)IconType.Confirmation];
                 confUI.SetTable(new Vector2(1, 1));

                 switch (count)
                 {
                     case 0:
                         Debug.Log("syozidayo");
                         confUI.SetText("すでに所持しています");
                         break;
                     case 1:
                         Debug.Log("kyoukadayo");
                         confUI.SetText("強化完了");
                         break;
                     case 2:
                         Debug.Log("tarinaiyo");
                         confUI.SetText("素材が足りません");
                         break;
                     default:
                         break;
                 }

                 confUI.CreateButton();

                 switch (count)
                 {
                     case 0:
                         {
                             confUI.SetButtonText(0, "OK");
                             confUI.SetButtonOnClick(0, () => { owner.ChangeState<EnhancementSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                         }
                         break;
                     case 1:
                         {
                             confUI.SetButtonText(0, "OK");
                             confUI.SetButtonOnClick(0, () => { owner.ChangeState<EnhancementSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                         }
                         break;
                     case 2:
                         {
                             confUI.SetButtonText(0, "OK");
                             confUI.SetButtonOnClick(0, () => { owner.ChangeState<EnhancementSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                         }
                         break;
                     default:
                         break;
                 }
             });

            UI.SetButtonText(1, "いいえ");
            UI.SetButtonOnClick(1, () => { owner.ChangeState<EnhancementSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Confirmation].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Confirmation].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<EnhancementSelectMode>();
        }
    }
}

