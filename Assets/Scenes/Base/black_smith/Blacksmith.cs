using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class Blacksmith : UIBase
{
    ///表示するテキスト
    [SerializeField] Text _blacksmithMode;
    //プレイヤーが近くまで来たか判断
    [SerializeField] TargetChecker _blacksmithChecker;
    //確認用
    [SerializeField] Confirmation _confirmation;
    //武器データリスト
    [SerializeField] WeaponDataList _WeaponDataList;
    //強化する武器のID
    [SerializeField] string _EnhancementWeaponID;

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
            Debug.Log("Blacksmith_close_OnEnter");
            UIManager.Instance._player.IsAction = true;
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log("Blacksmith_close_OnProceed");
            //近くに来ている && 決定ボタンを押している && キャンバスがactiveでない
            if (owner.GetComponent<Blacksmith>()._blacksmithChecker.TriggerHit && UIManager.Instance._player.IsAction)
            {
                Debug.Log("o");
                owner.ChangeState<TypeSelectMode>();
            }
        }
    }

    public class TypeSelectMode : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "製造";
            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());

            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "強化";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => owner.ChangeState<EnhancementSelectMode>());
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
            owner.ChangeState<Close>();
        }
    }

    public class ProductionSelectMode : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("ProductionSelect");
            //モード選択画面
            owner.GetComponent<Blacksmith>()._blacksmithMode.text = "何を作る？";
            //ボタンの追加
            var lists = owner.ItemIconList[(int)IconType.TypeSelect];
            lists.TableSize = new Vector2(3, 1);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();
            for (int i = 0; i < 3/*斧・槍・弓*/; i++)
            {
                Button button = list[i].GetComponent<Button>();
                Text buttonText = list[i].GetComponentInChildren<Text>();
                switch (i)
                {
                    case 0:
                        button.onClick.AddListener(() => owner.ChangeState<ProductionWeaponMode>());
                        buttonText.text = "製造：斧";
                        break;
                    case 1:
                        button.onClick.AddListener(() => owner.ChangeState<ProductionWeaponMode>());
                        buttonText.text = "製造：槍";
                        break;
                    case 2:
                        button.onClick.AddListener(() => owner.ChangeState<ProductionWeaponMode>());
                        buttonText.text = "製造：弓";
                        break;
                }
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
            owner.ChangeState<TypeSelectMode>();
        }
    }

    public class ProductionWeaponMode : UIStateBase
    {
        private bool ConfirmationSelect;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            ConfirmationSelect = false;

            switch (owner.GetComponent<Blacksmith>().productionWeaponType)
            {
                case 1:
                    owner.GetComponent<Blacksmith>()._blacksmithMode.text = "製造：斧";
                    break;
                case 2:
                    owner.GetComponent<Blacksmith>()._blacksmithMode.text = "製造：槍";
                    break;
                case 3:
                    owner.GetComponent<Blacksmith>()._blacksmithMode.text = "製造：弓";
                    break;
                default:
                    break;
            }


            //ボタンの追加
            var villageData = GameManager.Instance.VillageData;
            var Weapon = owner.GetComponent<Blacksmith>()._WeaponDataList.Dictionary;
            List<WeaponData> _CreatableWeapon = new List<WeaponData>();
            foreach (var item in Weapon)
            {
                if (item.Value.CreatableLevel <= villageData.BlacksmithLevel
                    && (int)item.Value.WeaponType == owner.GetComponent<Blacksmith>().productionWeaponType)
                {
                    _CreatableWeapon.Add(item.Value);
                }
            }

            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();
            for (int i = 0; i < _CreatableWeapon.Count; i++)
            {
                var buttonText = list[i].GetComponentInChildren<Text>();
                buttonText.text = _CreatableWeapon[i].Name;
                var button = list[i].GetComponent<Button>();
                switch (GameManager.Instance.WeaponDataList.Production(_CreatableWeapon[i].ID))
                {
                    case 0:
                        button.onClick.AddListener(() =>
                        {
                            ConfirmationSelect = true;
                            var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                            icon._textData.text = "すでに所持しています";
                            icon._tableSize = new Vector2(1, 1);
                            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                            var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                            var buttonText = list[0].GetComponentInChildren<Text>();
                            buttonText.text = "OK";
                            var button = list[0].GetComponent<Button>();
                            button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        });
                        break;
                    case 1:
                        button.onClick.AddListener(() =>
                        {
                            ConfirmationSelect = true;
                            var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                            icon._textData.text = "製造完了";
                            icon._tableSize = new Vector2(1, 1);
                            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                            var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                            var buttonText = list[0].GetComponentInChildren<Text>();
                            buttonText.text = "OK";
                            var button = list[0].GetComponent<Button>();
                            button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        });
                        break;
                    case 2:
                        button.onClick.AddListener(() =>
                        {
                            ConfirmationSelect = true;
                            var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                            icon._textData.text = "素材が足りません";
                            icon._tableSize = new Vector2(1, 1);
                            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                            var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                            var buttonText = list[0].GetComponentInChildren<Text>();
                            buttonText.text = "OK";
                            var button = list[0].GetComponent<Button>();
                            button.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());
                        });
                        break;
                    default:
                        break;
                }
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
            //モード選択画面
            owner.GetComponent<Blacksmith>()._blacksmithMode.text = "何を強化する？";
            //ボタンの追加
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();

            var Weapon = owner.GetComponent<Blacksmith>()._WeaponDataList.Dictionary;
            List<WeaponData> _EnhancementedWeapon = new List<WeaponData>();
            foreach (var item in Weapon)
            {
                if (item.Value.BoxPossession) _EnhancementedWeapon.Add(item.Value);
            }

            for (int i = 0; i < _EnhancementedWeapon.Count; i++)
            {
                var buttonText = list[i].GetComponentInChildren<Text>();
                buttonText.text = _EnhancementedWeapon[i].Name;
                var button = list[i].GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    owner.GetComponent<Blacksmith>()._EnhancementWeaponID = _EnhancementedWeapon[i].ID;
                    owner.ChangeState<EnhancementWeaponMode>();
                });
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
            owner.ChangeState<TypeSelectMode>();
        }
    }

    public class EnhancementWeaponMode : UIStateBase
    {
        private bool ConfirmationSelect;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            //モード選択画面
            var Weapon = owner.GetComponent<Blacksmith>()._WeaponDataList.Dictionary;
            string WeaponName = Weapon[owner.GetComponent<Blacksmith>()._EnhancementWeaponID].ID;
            owner.GetComponent<Blacksmith>()._blacksmithMode.text = WeaponName + "の強化";

            ConfirmationSelect = true;
            var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
            icon._textData.text = "素材を消費して武器を強化しますか？";
            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
            var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "はい";
            var button0 = list[0].GetComponent<Button>();
            switch (GameManager.Instance.WeaponDataList.Enhancement(owner.GetComponent<Blacksmith>()._EnhancementWeaponID))

            {
                case 0:
                    button0.onClick.AddListener(() =>
                    {
                        ConfirmationSelect = true;
                        var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                        icon._textData.text = "すでに所持しています";
                        icon._tableSize = new Vector2(1, 1);
                        owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                        var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                        var buttonText = list[0].GetComponentInChildren<Text>();
                        buttonText.text = "OK";
                        var button = list[0].GetComponent<Button>();
                        button.onClick.AddListener(() => owner.ChangeState<EnhancementSelectMode>());
                    });
                    break;
                case 1:
                    button0.onClick.AddListener(() =>
                    {
                        ConfirmationSelect = true;
                        var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                        icon._textData.text = "製造完了";
                        icon._tableSize = new Vector2(1, 1);
                        owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                        var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                        var buttonText = list[0].GetComponentInChildren<Text>();
                        buttonText.text = "OK";
                        var button = list[0].GetComponent<Button>();
                        button.onClick.AddListener(() => owner.ChangeState<EnhancementSelectMode>());
                    });
                    break;
                case 2:
                    button0.onClick.AddListener(() =>
                    {
                        ConfirmationSelect = true;
                        var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                        icon._textData.text = "素材が足りません";
                        icon._tableSize = new Vector2(1, 1);
                        owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
                        var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();
                        var buttonText = list[0].GetComponentInChildren<Text>();
                        buttonText.text = "OK";
                        var button = list[0].GetComponent<Button>();
                        button.onClick.AddListener(() => owner.ChangeState<EnhancementSelectMode>());
                    });
                    break;
                default:
                    break;
            }

            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "いいえ";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => owner.ChangeState<EnhancementSelectMode>());
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
            owner.ChangeState<TypeSelectMode>();
        }
    }
}

