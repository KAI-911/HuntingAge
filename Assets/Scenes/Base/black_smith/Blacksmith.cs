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
    //武器データリスト
    [SerializeField] WeaponDataList _WeaponDataList;
    //強化する武器のID
    string _EnhancementWeaponID;

    enum IconType
    {
        TypeSelect,
        Confirmation
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
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UIManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }

    [Serializable]
    public class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_Close_OnEnter");
            UIManager.Instance._player.IsAction = true;
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log("Blacksmith_Close_OnProceed");
            //近くに来ている && 決定ボタンを押している && キャンバスがactiveでない
            if (owner.GetComponent<Blacksmith>()._blacksmithChecker.TriggerHit && UIManager.Instance._player.IsAction)
            {
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

            var button0Text = list[0].GetComponentInChildren<Text>(); button0Text.text = "製造";
            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());

            var button1Text = list[1].GetComponentInChildren<Text>(); button1Text.text = "強化";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => owner.ChangeState<EnhancementSelectMode>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log(owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber);
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<Close>();
        }
    }

    public class ProductionSelectMode : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var iconText = owner.ItemIconList[(int)IconType.TypeSelect];
            var icon = iconText.IconData;
            icon._textData.text = "武器製造";
            iconText.SetIcondata(icon);
            //ボタンの追加
            var table = owner.ItemIconList[(int)IconType.TypeSelect];
            var iconData = table.IconData;
            iconData._tableSize = new Vector2(3, 1);
            table.SetIcondata(iconData);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();
            for (int i = 0; i < 3/*斧・槍・弓*/; i++)
            {
                Button button = list[i].GetComponent<Button>();
                Text buttonText = list[i].GetComponentInChildren<Text>();
                switch (i)
                {
                    case 0:
                        button.onClick.AddListener(() =>
                        {
                            owner.GetComponent<Blacksmith>().productionWeaponType = 0;
                            owner.ChangeState<ProductionWeaponMode>();
                        });
                        buttonText.text = "製造：斧";
                        break;
                    case 1:
                        button.onClick.AddListener(() =>
                        {
                            owner.GetComponent<Blacksmith>().productionWeaponType = 1;
                            owner.ChangeState<ProductionWeaponMode>();
                        });
                        buttonText.text = "製造：槍";
                        break;
                    case 2:
                        button.onClick.AddListener(() =>
                        {
                            owner.GetComponent<Blacksmith>().productionWeaponType = 2;
                            owner.ChangeState<ProductionWeaponMode>();
                        });
                        buttonText.text = "製造：弓";
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
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
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
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_ProductionWeaponMode_OnEnter");
            ConfirmationSelect = false;
            var iconText = owner.ItemIconList[(int)IconType.TypeSelect];
            var icon = iconText.IconData;
            icon._textData.text = "武器製造";
            iconText.SetIcondata(icon);

            //ボタンの追加
            var villageData = GameManager.Instance.VillageData;
            var Weapon = owner.GetComponent<Blacksmith>()._WeaponDataList.Dictionary;
            List<WeaponData> _CreatableWeapon = new List<WeaponData>();
            foreach (var item in Weapon)
            {
                if ((item.Value.CreatableLevel <= villageData.BlacksmithLevel)
                    && ((int)item.Value.WeaponType == owner.GetComponent<Blacksmith>().productionWeaponType))
                {
                    _CreatableWeapon.Add(item.Value);
                }
            }

            var table = owner.ItemIconList[(int)IconType.TypeSelect];
            var iconData = table.IconData;
            iconData._tableSize = new Vector2(_CreatableWeapon.Count, 1);
            table.SetIcondata(iconData);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();
            for (int i = 0; i < _CreatableWeapon.Count; i++)
            {
                var buttonText = list[i].GetComponentInChildren<Text>();
                buttonText.text = _CreatableWeapon[i].Name;
                var button = list[i].GetComponent<Button>();
                int num = i;
                button.onClick.AddListener(() =>
                {
                    Debug.Log("osaretayo " + _CreatableWeapon[num].ID);
                    int count = GameManager.Instance.WeaponDataList.Production(_CreatableWeapon[num].ID);
                    ConfirmationSelect = true;

                    var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                    icon._tableSize = new Vector2(1, 1);
                    icon._textData.hight = 60;

                    switch (count)
                    {
                        case 0:
                            icon._textData.text = "すでに所持しています";
                            break;
                        case 1:
                            icon._textData.text = "製造完了";
                            break;
                        case 2:
                            icon._textData.text = "素材が足りません";
                            break;
                        default:
                            break;
                    }

                    owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                    var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                    var buttonText = list[0].GetComponentInChildren<Text>();
                    var button = list[0].GetComponent<Button>();

                    switch (count)
                    {
                        case 0:
                            {
                                Debug.Log("すでに所持しています");
                                buttonText.text = "OK";
                                button.onClick.AddListener(() => { owner.ChangeState<ProductionSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                            }
                            break;
                        case 1:
                            {
                                Debug.Log("製造完了");
                                buttonText.text = "OK";
                                button.onClick.AddListener(() => { owner.ChangeState<ProductionSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                            }
                            break;
                        case 2:
                            {
                                Debug.Log("素材が足りません");
                                buttonText.text = "OK";
                                button.onClick.AddListener(() => { owner.ChangeState<ProductionSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                            }
                            break;
                        default:
                            break;
                    }
                });
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            if (ConfirmationSelect)
            {
                owner.ItemIconList[(int)IconType.Confirmation].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            }
            else
            {
                owner.ItemIconList[(int)IconType.TypeSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
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
            var iconText = owner.ItemIconList[(int)IconType.TypeSelect];
            var icon = iconText.IconData;
            icon._textData.text = "武器強化先";
            iconText.SetIcondata(icon);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();

            var Weapon = owner.GetComponent<Blacksmith>()._WeaponDataList.Dictionary;
            List<WeaponData> _TmpWeapon = new List<WeaponData>();
            foreach (var item in Weapon)
            {
                if (item.Value.BoxPossession
                    && item.Value.EnhancementID != "empty") _TmpWeapon.Add(item.Value);
            }

            var table = owner.ItemIconList[(int)IconType.TypeSelect];
            var iconData = table.IconData;
            iconData._tableSize = new Vector2(_TmpWeapon.Count, 1);
            table.SetIcondata(iconData);

            for (int i = 0; i < _TmpWeapon.Count; i++)
            {
                int num = i;
                string enhancementID = _TmpWeapon[num].EnhancementID;
                var buttonText = list[num].GetComponentInChildren<Text>();
                var name = GameManager.Instance.WeaponDataList;int index = name.Keys.IndexOf(enhancementID);
                buttonText.text = name.Values[index].Name;
                var button = list[num].GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    owner.GetComponent<Blacksmith>()._EnhancementWeaponID = enhancementID;
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
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
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
            var Weapon = owner.GetComponent<Blacksmith>()._WeaponDataList.Dictionary;
            string WeaponName = Weapon[owner.GetComponent<Blacksmith>()._EnhancementWeaponID].ID;

            ConfirmationSelect = true;
            var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
            icon._textData.text = "素材を消費して武器を強化しますか？";
            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
            var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "はい";
            var button0 = list[0].GetComponent<Button>();

            button0.onClick.AddListener(() =>
            {

                owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
                int count = GameManager.Instance.WeaponDataList.Enhancement(WeaponName);
                ConfirmationSelect = true;

                var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                icon._tableSize = new Vector2(1, 1);
                icon._textData.hight = 60;

                switch (count)
                {
                    case 0:
                Debug.Log("syozidayo");
                        icon._textData.text = "すでに所持しています";
                        break;
                    case 1:
                        Debug.Log("kyoukadayo");
                        icon._textData.text = "強化完了";
                        break;
                    case 2:
                        Debug.Log("tarinaiyo");
                        icon._textData.text = "素材が足りません";
                        break;
                    default:
                        break;
                }

                owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                var buttonText = list[0].GetComponentInChildren<Text>();
                var button = list[0].GetComponent<Button>();

                switch (count)
                {
                    case 0:
                        {
                            buttonText.text = "OK";
                            button.onClick.AddListener(() => { owner.ChangeState<EnhancementSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                        }
                        break;
                    case 1:
                        {
                            buttonText.text = "OK";
                            button.onClick.AddListener(() => { owner.ChangeState<EnhancementSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                        }
                        break;
                    case 2:
                        {
                            buttonText.text = "OK";
                            button.onClick.AddListener(() => { owner.ChangeState<EnhancementSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                        }
                        break;
                    default:
                        break;
                }
            });

            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "いいえ";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => { owner.ChangeState<EnhancementSelectMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
        }
        public override void OnUpdate(UIBase owner)
        {
                owner.ItemIconList[(int)IconType.Confirmation].Select(UIManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Confirmation].Buttons[owner.ItemIconList[(int)IconType.Confirmation].CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<EnhancementSelectMode>();
        }
    }
}

