using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class Blacksmith : UIBase
{
    //�v���C���[���߂��܂ŗ��������f
    [SerializeField] TargetChecker _blacksmithChecker;
    //�������镐���ID
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
            //�߂��ɗ��Ă��� && ����{�^���������Ă��� && �L�����o�X��active�łȂ�
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
            UI.SetText("�b���F����");
            UI.SetTable(new Vector2(2, 1));
            var list = UI.CreateButton();

            UI.SetButtonText(0, "����");
            UI.SetButtonOnClick(0, () => owner.ChangeState<ProductionSelectMode>());

            UI.SetButtonText(1, "����");
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
            UI.SetText("���퐻��");
            UI.SetTable(new Vector2(3, 1));
            var list = UI.CreateButton();
            for (int i = 0; i < 3/*���E���E�|*/; i++)
            {
                switch (i)
                {
                    case 0:
                        UI.SetButtonText(i, "���n��");
                        UI.SetButtonOnClick(i, () =>
                             {
                                 owner.GetComponent<Blacksmith>().productionWeaponType = 0;
                                 owner.ChangeState<ProductionWeaponMode>();
                             });
                        break;
                    case 1:
                        UI.SetButtonText(i, "���n��");
                        UI.SetButtonOnClick(i, () =>
                            {
                                owner.GetComponent<Blacksmith>().productionWeaponType = 1;
                                owner.ChangeState<ProductionWeaponMode>();
                            });
                        break;
                    case 2:
                        UI.SetButtonText(i, "�|�n��");
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

            ButtonUI.SetText("���퐻��");
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
                            confUI.SetText("���łɏ������Ă��܂�");
                            break;
                        case 1:
                            confUI.SetText("��������");
                            break;
                        case 2:
                            confUI.SetText("�f�ނ�����܂���");
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
                var ListUI = owner.ItemIconList[(int)IconType.MaterialList];
                var data = _CreatableWeapon[buttonCount].ProductionNeedMaterialLst;
                ListUI.SetText("�K�v�f�ށ^�f�ޏ�����");
                ListUI.SetTable(new Vector2(data.Count, 1));
                ListUI.SetLeftTopPos(new Vector2(100, 200));
                ListUI.CreateButton();
                for (int i = 0; i < data.Count; i++)
                {
                    var materialID = data[i].materialID;
                    var material = GameManager.Instance.MaterialDataList.Dictionary[materialID];
                    string text1 = string.Format("{0,3:d}", data[i].requiredCount.ToString());
                    string text2 = string.Format("{0,4:d}",(material.BoxHoldNumber + material.PoachHoldNumber).ToString());
                    Debug.Log(text1);
                    Debug.Log(text2);
                    ListUI.SetButtonText(i,"�@�@" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
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
        List<WeaponData> _TmpWeapon = new List<WeaponData>();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            //�{�^���̒ǉ�
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("���틭����");

            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
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
            int buttonCount = owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber;
            var ListUI = owner.ItemIconList[(int)IconType.MaterialList];
            var data = _TmpWeapon[buttonCount].ProductionNeedMaterialLst;
            ListUI.SetText("�K�v�f�ށ^�f�ޏ�����");
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
                ListUI.SetButtonText(i, "�@�@" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
            }
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
            //���[�h�I�����
            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
            string WeaponName = Weapon[owner.GetComponent<Blacksmith>()._EnhancementWeaponID].ID;
            var UI = owner.ItemIconList[(int)IconType.Confirmation];

            ConfirmationSelect = true;
            UI.SetText("�f�ނ�����ĕ�����������܂����H");
            UI.SetTable(new Vector2(1, 2));
            UI.CreateButton();
            UI.SetButtonText(0, "�͂�");
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
                         confUI.SetText("���łɏ������Ă��܂�");
                         break;
                     case 1:
                         Debug.Log("kyoukadayo");
                         confUI.SetText("��������");
                         break;
                     case 2:
                         Debug.Log("tarinaiyo");
                         confUI.SetText("�f�ނ�����܂���");
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

            UI.SetButtonText(1, "������");
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

