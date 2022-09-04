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
    //�������镐���ID
    [SerializeField] string _cleateItemID;
    //�{�b�N�X���|�[�`�ǂ����ɑ��邩
    [SerializeField] bool _toPouch;
    //����邩
    int cleateNum;
    private GameObject countObject;

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
        ItemIconList[(int)IconType.TypeSelect].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }

    [Serializable]
    public class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("Blacksmith_close_OnEnter");
            UISoundManager.Instance._player.IsAction = true;
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log("Blacksmith_close_OnProceed");
            //�߂��ɗ��Ă��� && ����{�^���������Ă��� && �L�����o�X��active�łȂ�
            if (owner.GetComponent<kichen>()._kichenChecker.TriggerHit && UISoundManager.Instance._player.IsAction)
            {
                owner.ChangeState<BoxorPouch>();
            }
        }
    }

    public class BoxorPouch : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("�A�C�e�����ǂ��ɑ���܂����H");
            UI.SetTable(new Vector2(2, 1));
            UI.CreateButton();
            Debug.Log("dasitayo");
            UI.SetButtonText(0, "�{�b�N�X��");
            UI.SetButtonOnClick(0, () => { owner.GetComponent<kichen>()._toPouch = false; owner.ChangeState<TypeSelectMode>(); });

            UI.SetButtonText(1, "�|�[�`��");
            UI.SetButtonOnClick(1, () => { owner.GetComponent<kichen>()._toPouch = true; owner.ChangeState<TypeSelectMode>(); });
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
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
        private GameObject count;
        private int min, max, now;
        private bool lockflg;
        private bool _check;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(CleateItem))
            {
                owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();
            }
            _check = false;
            var UI = owner.ItemIconList[(int)IconType.TypeSelect];
            UI.SetText("���A�C�e��");
            //���[�h�I�����
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

            for (int i = 0; i < _createItem.Count; i++)
            {
                int numi = i;
                UI.SetButtonText(numi, _createItem[numi].baseData.Name);
                UI.SetButtonOnClick(numi, () =>
                {
                    _check = true;
                    count = Instantiate(Resources.Load("UI/Count"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
                    owner.ItemIconList[(int)IconType.TypeSelect].AdjustmentImage(count.GetComponent<RectTransform>());
                   
                    now = 0;
                    min = 0;
                    max = 9999;

                    string id = owner.GetComponent<kichen>()._cleateItemID;
                    var _Item = GameManager.Instance.ItemDataList;
                    var _material = GameManager.Instance.MaterialDataList;
                    var counter = _Item.Dictionary[id].NeedMaterialLst;
                    for (int i = 0; i < counter.Count; i++)
                    {
                        string _needMaterialID = _Item.Dictionary[id].NeedMaterialLst[i].materialID;
                        var tmp = counter[i].requiredCount;
                        var materialNum = _material.Dictionary[_needMaterialID].BoxHoldNumber + _material.Dictionary[_needMaterialID].PoachHoldNumber;
                        if ((materialNum / tmp) < max)
                        {
                            max = materialNum / tmp;
                        }
                    }
                });
            }
        }
        public override void OnUpdate(UIBase owner)
        {

            if (!_check)
            {
                owner.ItemIconList[(int)IconType.TypeSelect].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            }
            else
            {
                var vec = UISoundManager.Instance.InputSelection.ReadValue<Vector2>();
                if (vec.sqrMagnitude > 0)
                {
                    if (lockflg == false)
                    {
                        if (vec.y > 0) now++;
                        else now--;
                        now = Mathf.Clamp(now, min, max);
                        lockflg = true;

                        count.GetComponentInChildren<Text>().text = now.ToString();
                    }
                }
                else
                {
                    lockflg = false;
                }
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(owner.GetComponent<kichen>().countObject);
            owner.ItemIconList[(int)IconType.TypeSelect].DeleteButton();

        }
        public override void OnProceed(UIBase owner)
        {
            if (!_check)
            {
                Debug.Log("futuu");
                owner.ItemIconList[(int)IconType.TypeSelect].Buttons[owner.ItemIconList[(int)IconType.TypeSelect].CurrentNunber].GetComponent<Button>().onClick.Invoke();
            }
            else
            {
                owner.GetComponent<kichen>().cleateNum = now;
                owner.ChangeState<CleateItem>();
            }
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
        private ItemIcon confimationIcon;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            confimationIcon = owner.ItemIconList[(int)IconType.Confirmation];
            Debug.Log("cleateItemnikitayo");
            ConfirmationSelect = false;
            confimationIcon.SetText("�f�ނ�����ăA�C�e�������܂����H");
            confimationIcon.SetTable(new Vector2(1, 2));
            confimationIcon.CreateButton();

            confimationIcon.SetButtonText(0, "�͂�");
            confimationIcon.SetButtonOnClick(0, () =>
            {
                GameManager.Instance.WeaponDataList.Enhancement(owner.GetComponent<kichen>()._cleateItemID);
                ConfirmationSelect = true;
                var UI = confimationIcon;
                UI.SetText("��������");
                UI.SetTable(new Vector2(1, 1));
                UI.CreateButton();
                UI.SetButtonText(0,"OK");
                UI.SetButtonOnClick(0,() => owner.ChangeState<TypeSelectMode>());
            });

            confimationIcon.SetButtonText(1, "������");
            confimationIcon.SetButtonOnClick(1, () => owner.ChangeState<TypeSelectMode>());
        }
        public override void OnUpdate(UIBase owner)
        {
            confimationIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            Destroy(owner.GetComponent<kichen>().countObject);
            confimationIcon.DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            confimationIcon.Buttons[confimationIcon.CurrentNunber].GetComponent<Button>().onClick.Invoke();
        }
        public override void OnBack(UIBase owner)
        {
            Debug.Log("modoru");
            owner.ChangeState<TypeSelectMode>();
        }
    }


}

