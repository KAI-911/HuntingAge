using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class kichen : UIBase
{
    //�v���C���[���߂��܂ŗ��������f
    [SerializeField] TargetChecker _kichenChecker;
    //�m�F�p
    [SerializeField] Confirmation _confirmation;
    //����f�[�^���X�g
    [SerializeField] MaterialDataList _itemDataList;
    //�������镐���ID
    [SerializeField] string _cleateItem;

    enum IconType
    {
        TypeSelect,
        Confirmation
    }

    private int productionItem;
    private enum ItemType
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
            if (owner.GetComponent<kichen>()._kichenChecker.TriggerHit && UIManager.Instance._player.IsAction)
            {
                owner.ChangeState<BoxorPouch>();
            }
        }
    }

    public class BoxorPouch : UIStateBase
    {
        private bool ConfirmationSelect;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            ConfirmationSelect = true;
            var icon = owner.ItemIconList[(int)IconType.TypeSelect].IconData;
            icon._textData.text = "�A�C�e�����ǂ��ɑ���܂����H";
            owner.ItemIconList[(int)IconType.TypeSelect].SetIcondata(icon);
            var table = owner.ItemIconList[(int)IconType.TypeSelect];
            var iconData = table.IconData;
            iconData._tableSize = new Vector2(2, 1);
            table.SetIcondata(iconData);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "�{�b�N�X��";
            var button0 = list[0].GetComponent<Button>();
            

            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "�|�[�`��";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
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
            owner.ChangeState<Close>();
        }
    }

    public class TypeSelectMode : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var iconText = owner.ItemIconList[(int)IconType.TypeSelect];
            var icon = iconText.IconData;
            icon._textData.text = "���A�C�e��";
            iconText.SetIcondata(icon);
            var list = owner.ItemIconList[(int)IconType.TypeSelect].CreateButton();
            //���[�h�I�����
            var Weapon = owner.GetComponent<kichen>()._itemDataList.Dictionary;
            List<ItemData> _createItem = new List<ItemData>();
            foreach (var item in Weapon)
            {
                //if (item.Value.CreatableLevel < GameManager.Instance.VillageData.KitchenLevel
                    //&& (int)item.Value.ItemType > 0) _createItem.Add(item.Value);
            }

            for (int i = 0; i < _createItem.Count; i++)
            {
                int num = i;
                var buttonText = list[num].GetComponentInChildren<Text>();
                buttonText.text = _createItem[num].Name;
                var button = list[num].GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    owner.GetComponent<kichen>()._cleateItem = _createItem[num].ID;
                    owner.ChangeState<CleateItem>();
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
            owner.ChangeState<Close>();
        }
    }

    public class CleateItem : UIStateBase
    {
        private bool ConfirmationSelect;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            ConfirmationSelect = true;
            var icon = owner.ItemIconList[(int)IconType.Confirmation].IconData;
            icon._textData.text = "�f�ނ�����ăA�C�e�������܂����H";
            owner.ItemIconList[(int)IconType.Confirmation].SetIcondata(icon);
            var list = owner.ItemIconList[(int)IconType.Confirmation].CreateButton();

            var button0Text = list[0].GetComponentInChildren<Text>();
            button0Text.text = "�͂�";
            var button0 = list[0].GetComponent<Button>();
            switch (GameManager.Instance.WeaponDataList.Enhancement(owner.GetComponent<kichen>()._cleateItem))

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
                        button.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
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
                        button.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
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
                        button.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
                    });
                    break;
                default:
                    break;
            }

            var button1Text = list[1].GetComponentInChildren<Text>();
            button1Text.text = "������";
            var button1 = list[1].GetComponent<Button>();
            button1.onClick.AddListener(() => owner.ChangeState<TypeSelectMode>());
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

