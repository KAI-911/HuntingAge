using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class Blacksmith : UIBase
{
    ///�\������e�L�X�g
    [SerializeField] Text _blacksmithMode;
    //�v���C���[���߂��܂ŗ��������f
    [SerializeField] TargetChecker _blacksmithChecker;
    //�m�F�p
    [SerializeField] Confirmation _confirmation;
    //����f�[�^���X�g
    [SerializeField] WeaponDataList _WeaponDataList;
    //�������镐���ID
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
            //�߂��ɗ��Ă��� && ����{�^���������Ă��� && �L�����o�X��active�łȂ�
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
            button0Text.text = "����";
            var button0 = list[0].GetComponent<Button>();
            button0.onClick.AddListener(() => owner.ChangeState<ProductionSelectMode>());

            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "����";
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
            //���[�h�I�����
            owner.GetComponent<Blacksmith>()._blacksmithMode.text = "�������H";
            //�{�^���̒ǉ�
            var lists = owner.ItemIconList[(int)IconType.TypeSelect];
            lists.TableSize = new Vector2(3, 1);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();
            for (int i = 0; i < 3/*���E���E�|*/; i++)
            {
                Button button = list[i].GetComponent<Button>();
                Text buttonText = list[i].GetComponentInChildren<Text>();
                switch (i)
                {
                    case 0:
                        button.onClick.AddListener(() => owner.ChangeState<ProductionWeaponMode>());
                        buttonText.text = "�����F��";
                        break;
                    case 1:
                        button.onClick.AddListener(() => owner.ChangeState<ProductionWeaponMode>());
                        buttonText.text = "�����F��";
                        break;
                    case 2:
                        button.onClick.AddListener(() => owner.ChangeState<ProductionWeaponMode>());
                        buttonText.text = "�����F�|";
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
                    owner.GetComponent<Blacksmith>()._blacksmithMode.text = "�����F��";
                    break;
                case 2:
                    owner.GetComponent<Blacksmith>()._blacksmithMode.text = "�����F��";
                    break;
                case 3:
                    owner.GetComponent<Blacksmith>()._blacksmithMode.text = "�����F�|";
                    break;
                default:
                    break;
            }


            //�{�^���̒ǉ�
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
                            icon._textData.text = "���łɏ������Ă��܂�";
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
                            icon._textData.text = "��������";
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
                            icon._textData.text = "�f�ނ�����܂���";
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
            //���[�h�I�����
            owner.GetComponent<Blacksmith>()._blacksmithMode.text = "������������H";
            //�{�^���̒ǉ�
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
            //���[�h�I�����
            var Weapon = owner.GetComponent<Blacksmith>()._WeaponDataList.Dictionary;
            string WeaponName = Weapon[owner.GetComponent<Blacksmith>()._EnhancementWeaponID].ID;
            owner.GetComponent<Blacksmith>()._blacksmithMode.text = WeaponName + "�̋���";

            ConfirmationSelect = true;
            var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
            icon._textData.text = "�f�ނ�����ĕ�����������܂����H";
            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
            var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "�͂�";
            var button0 = list[0].GetComponent<Button>();
            switch (GameManager.Instance.WeaponDataList.Enhancement(owner.GetComponent<Blacksmith>()._EnhancementWeaponID))

            {
                case 0:
                    button0.onClick.AddListener(() =>
                    {
                        ConfirmationSelect = true;
                        var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
                        icon._textData.text = "���łɏ������Ă��܂�";
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
                        icon._textData.text = "��������";
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
                        icon._textData.text = "�f�ނ�����܂���";
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
            button1Text.text = "������";
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

