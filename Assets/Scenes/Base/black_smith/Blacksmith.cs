using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class Blacksmith : UIBase
{
    //ÉvÉåÉCÉÑÅ[Ç™ãﬂÇ≠Ç‹Ç≈óàÇΩÇ©îªíf
    [SerializeField] TargetChecker _blacksmithChecker;
    //ã≠âªÇ∑ÇÈïêäÌÇÃID
    string _createWeaponID;
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
            //ãﬂÇ≠Ç…óàÇƒÇ¢ÇÈ && åàíËÉ{É^ÉìÇâüÇµÇƒÇ¢ÇÈ && ÉLÉÉÉìÉoÉXÇ™activeÇ≈Ç»Ç¢
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
            UI.SetText("íbñËèÍÅFïêäÌ");
            UI.SetTable(new Vector2(2, 1));
            var list = UI.CreateButton();

            UI.SetButtonText(0, "êªë¢");
            UI.SetButtonOnClick(0, () => owner.ChangeState<ProductionSelect>());

            UI.SetButtonText(1, "ã≠âª");
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

    public class ProductionSelect : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("ïêäÌêªë¢");
            UI.SetTable(new Vector2(3, 1));
            var list = UI.CreateButton();
            for (int i = 0; i < 3/*ïÄÅEëÑÅEã|*/; i++)
            {
                switch (i)
                {
                    case 0:
                        UI.SetButtonText(i, "ïÄånìù");
                        UI.SetButtonOnClick(i, () =>
                             {
                                 owner.GetComponent<Blacksmith>().productionWeaponType = 0;
                                 owner.ChangeState<ProductionWeapon>();
                             });
                        break;
                    case 1:
                        UI.SetButtonText(i, "ëÑånìù");
                        UI.SetButtonOnClick(i, () =>
                            {
                                owner.GetComponent<Blacksmith>().productionWeaponType = 1;
                                owner.ChangeState<ProductionWeapon>();
                            });
                        break;
                    case 2:
                        UI.SetButtonText(i, "ã|ånìù");
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
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
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

            ButtonUI.SetText("ïêäÌêªë¢");
            ButtonUI.SetTable(new Vector2(_CreatableWeapon.Count, 1));
            ButtonUI.CreateButton();

            for (int i = 0; i < _CreatableWeapon.Count; i++)
            {
                ButtonUI.SetButtonText(i, _CreatableWeapon[i].Name);
                int num = i;
                ButtonUI.SetButtonOnClick(i, () =>
                {
                    owner.GetComponent<Blacksmith>()._createWeaponID = _CreatableWeapon[num].ID;
                    owner.ChangeState<Confirmation>();
                });
            }
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            int buttonCount = owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber;

            var ListUI = owner.ItemIconList[(int)IconType.MaterialList];
            var data = _CreatableWeapon[buttonCount].ProductionNeedMaterialLst;
            ListUI.SetText("ïKóvëfçﬁÅ^ëfçﬁèäéùêî");
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
                ListUI.SetButtonText(i, "Å@Å@" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
            }

        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            //owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            //owner.ItemIconList[(int)IconType.MaterialList].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
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
            //É{É^ÉìÇÃí«â¡
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("ïêäÌã≠âªêÊ");

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
                    owner.GetComponent<Blacksmith>()._createWeaponID = _TmpWeapon[num].ID;
                    owner.ChangeState<Confirmation>();
                });
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            //owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            int buttonCount = owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber;
            var ListUI = owner.ItemIconList[(int)IconType.MaterialList];
            var data = _TmpWeapon[buttonCount].EnhancementNeedMaterialLst;
            ListUI.SetText("ïKóvëfçﬁÅ^ëfçﬁèäéùêî");
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
                ListUI.SetButtonText(i, "Å@Å@" + material.Name + Data.Convert.HanToZenConvert(text1 + "/" + text2), TextAnchor.MiddleLeft);
            }
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
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
            //ÉÇÅ[ÉhëIëâÊñ 
            _prevState = prevState;
            var Weapon = GameManager.Instance.WeaponDataList.Dictionary;
            string WeaponName = Weapon[owner.GetComponent<Blacksmith>()._createWeaponID].ID;
            var UI = owner.ItemIconList[(int)IconType.Confirmation];

            Debug.Log(11111);
            if (prevState.GetType() == typeof(EnhancementSelect)) UI.SetText("ëfçﬁÇè¡îÔÇµÇƒïêäÌÇã≠âªÇµÇ‹Ç∑Ç©ÅH");
            else if(prevState.GetType() == typeof(ProductionWeapon)) UI.SetText("ëfçﬁÇè¡îÔÇµÇƒïêäÌÇêªë¢ÇµÇ‹Ç∑Ç©ÅH");

            Debug.Log(111111);
            UI.SetTable(new Vector2(1, 2));
            UI.CreateButton();
            UI.SetButtonText(0, "ÇÕÇ¢");
            UI.SetButtonOnClick(0, () =>
             {
                 UI.DeleteButton();
                 int count = -1;
                 if (prevState.GetType() == typeof(EnhancementSelect)) count = GameManager.Instance.WeaponDataList.Enhancement(WeaponName);
                 else if (prevState.GetType() == typeof(ProductionWeapon)) count = GameManager.Instance.WeaponDataList.Production(WeaponName);

                 var confUI = owner.ItemIconList[(int)IconType.Confirmation];
                 confUI.SetTable(new Vector2(1, 1));

                 switch (count)
                 {
                     case 1:
                         Debug.Log("syozidayo");
                         confUI.SetText("Ç∑Ç≈Ç…èäéùÇµÇƒÇ¢Ç‹Ç∑");
                         break;
                     case 2:
                         Debug.Log("kyoukadayo");
                         if (prevState.GetType() == typeof(EnhancementSelect)) confUI.SetText("ã≠âªäÆóπ");
                         else if (prevState.GetType() == typeof(ProductionWeapon)) confUI.SetText("êªë¢äÆóπ");
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
                             confUI.SetButtonOnClick(0, () => { owner.ChangeState<ChoiceMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                         }
                         break;
                     case 1:
                         {
                             confUI.SetButtonText(0, "OK");
                             confUI.SetButtonOnClick(0, () => { owner.ChangeState<ChoiceMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                         }
                         break;
                     case 2:
                         {
                             confUI.SetButtonText(0, "OK");
                             confUI.SetButtonOnClick(0, () => { owner.ChangeState<ChoiceMode>(); owner.ItemIconList[(int)IconType.Confirmation].DeleteButton(); });
                         }
                         break;
                     default:
                         break;
                 }
             });

            UI.SetButtonText(1, "Ç¢Ç¢Ç¶");
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
            owner.ItemIconList[(int)IconType.Confirmation].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<ChoiceMode>();
        }
    }
}