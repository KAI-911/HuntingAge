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
    //�{�^���̐F�ݒ�
    [SerializeField] Color _cantColor;
    //�������镐���ID
    string _createWeaponID;
    //����𐻑����邩�������邩
    enum Mode { create, Enhancement }
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
        //_materialList = new MaterialList();
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
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
            //�߂��ɗ��Ă��� && ����{�^���������Ă��� && �L�����o�X��active�łȂ�
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
            Debug.Log("Blacksmith_TypeSelectMode_OnEnter");
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("�b���F����");
            UI.SetTable(new Vector2(2, 1));
            var list = UI.CreateButton();

            UI.SetButtonText(0, "����");
            UI.SetButtonOnClick(0, () => owner.ChangeState<ProductionSelect>());

            UI.SetButtonText(1, "����");
            UI.SetButtonOnClick(1, () => owner.ChangeState<EnhancementSelect>());
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
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.TypeSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            UISoundManager.Instance._player.IsAction = true;
            owner.ChangeState<Close>();
        }
    }

    public class ProductionSelect : UIStateBase
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
                                 owner.ChangeState<ProductionWeapon>();
                             });
                        break;
                    case 1:
                        UI.SetButtonText(i, "���n��");
                        UI.SetButtonOnClick(i, () =>
                            {
                                owner.GetComponent<Blacksmith>().productionWeaponType = 1;
                                owner.ChangeState<ProductionWeapon>();
                            });
                        break;
                    case 2:
                        UI.SetButtonText(i, "�|�n��");
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
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
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
            owner.ChangeState<ChoiceMode>();
        }
    }

    public class ProductionWeapon : UIStateBase
    {
        List<WeaponData> _CreatableWeapon = new List<WeaponData>();
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_ProductionWeaponMode_OnEnter");

            var ButtonUI = owner.ItemIconList[(int)IconType.TypeSelect];

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
                string text2 = string.Format("{0,4:d}", (material.BoxHoldNumber + material.PoachHoldNumber).ToString());
                Debug.Log(text1);
                Debug.Log(text2);
                ListUI.SetButtonText(i, "�@�@" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
            }

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            //owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            //owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            UISoundManager.Instance.PlayDecisionSE();
            owner.ItemIconList[(int)IconType.TypeSelect].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            Debug.Log("modoru");
            owner.ChangeState<ProductionSelect>();
        }
    }
    public class EnhancementSelect : UIStateBase
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

            if (_TmpWeapon.Count <= 0)
            {
                var confUI = owner.ItemIconList[(int)IconType.Confirmation];
                confUI.SetText("�����ł��镐�킪����܂���");
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
                    Debug.Log("11111111");
                    var name = GameManager.Instance.WeaponDataList; int index = name.Keys.FindIndex(n => n.StartsWith(enhancementID));
                    Debug.Log("22222222");
                    Debug.Log(name.Values[index].Name);
                    UI.SetButtonText(i, name.Values[index].Name);
                    Debug.Log("44444444");
                    if (!owner.GetComponent<Blacksmith>().Check(name.Values[index], false))
                    {
                        Debug.Log("333333");
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
            //���[�h�I�����
            _prevState = prevState;
            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
            string WeaponName = Weapon[owner.GetComponent<Blacksmith>()._createWeaponID].ID;
            var UI = owner.ItemIconList[(int)IconType.Confirmation];

            Debug.Log(11111);
            if (prevState.GetType() == typeof(EnhancementSelect)) UI.SetText("�f�ނ�����ĕ�����������܂����H");
            else if (prevState.GetType() == typeof(ProductionWeapon)) UI.SetText("�f�ނ�����ĕ���𐻑����܂����H");

            Debug.Log(111111);
            UI.SetTable(new Vector2(1, 2));
            UI.CreateButton();
            UI.SetButtonText(0, "�͂�");
            UI.SetButtonOnClick(0, () =>
             {
                 UI.DeleteButton();
                 bool count = false;
                 if (prevState.GetType() == typeof(EnhancementSelect)) count = GameManager.Instance.WeaponDataList.Enhancement(WeaponName, true);
                 else if (prevState.GetType() == typeof(ProductionWeapon)) count = GameManager.Instance.WeaponDataList.Production(WeaponName, true);

                 var confUI = owner.ItemIconList[(int)IconType.Confirmation];
                 confUI.SetTable(new Vector2(1, 1));

                 if (!count) confUI.SetText("���łɏ������Ă��܂�");
                 else
                 {
                     if (prevState.GetType() == typeof(EnhancementSelect)) confUI.SetText("��������");
                     else if (prevState.GetType() == typeof(ProductionWeapon)) confUI.SetText("��������");
                 }

                 confUI.SetLeftTopPos(new Vector2(-200, -100));
                 confUI.CreateButton();

                 confUI.SetButtonText(0, "OK");
                 confUI.SetButtonOnClick(0, () => { owner.ChangeState<ChoiceMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
             });
            UI.SetButtonText(1, "������");
            UI.SetButtonOnClick(1, () => { owner.ChangeState<ChoiceMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Confirmation].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
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
            owner.ChangeState<ChoiceMode>();
        }
    }
}