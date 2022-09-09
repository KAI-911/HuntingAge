using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using UnityEngine.EventSystems;

public class kichen_varSato : UIBase
{
    //�v���C���[���߂��܂ŗ��������f
    [SerializeField] TargetChecker _kichenChecker;
    //���삷��A�C�e����ID
    [SerializeField] string _cleateItemID;
    [SerializeField] GameObject _materialIDandNumber;

    //�{�b�N�X���|�[�`�ǂ����ɑ��邩
    [SerializeField] bool _toPouch;
    //����邩
    int cleateNum;

    private Count _count;
    private ItemExplanation _itemExplanation;

    enum IconType
    {
        Select,
        Confirmation,
        Needmaterial
    }


    void Start()
    {
        _count = new Count();
        ItemIconList[(int)IconType.Select].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Confirmation].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["Confirmation"]);
        //���v����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        ItemIconList[(int)IconType.Needmaterial].SetIcondata(UISoundManager.Instance.UIPresetData.Dictionary["BlacksmithButton"]);
        ItemIconList[(int)IconType.Needmaterial].SetLeftTopPos(new Vector2(100, 200));

        _currentState = new Close();
        _currentState.OnEnter(this, null);
    }

    [Serializable]
    public class Close : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            Debug.Log("kichen_close_OnEnter");
            UISoundManager.Instance._player.IsAction = true;
        }
        public override void OnProceed(UIBase owner)
        {
            Debug.Log("kichen_close_OnProceed");
            //�߂��ɗ��Ă��� && ����{�^���������Ă��� && �L�����o�X��active�łȂ�
            if (owner.GetComponent<kichen_varSato>()._kichenChecker.TriggerHit && UISoundManager.Instance._player.IsAction)
            {
                owner.ChangeState<BoxorPouch>();
            }
        }
    }

    public class BoxorPouch : UIStateBase
    {
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            var UI = owner.ItemIconList[(int)IconType.Select];
            UI.SetText("�A�C�e�����ǂ��ɑ���܂����H");
            UI.SetTable(new Vector2(2, 1));
            UI.CreateButton();
            UI.SetButtonText(0, "�{�b�N�X��");
            UI.SetButtonOnClick(0, () => { owner.GetComponent<kichen_varSato>()._toPouch = false; owner.ChangeState<SelectMode>(); });

            UI.SetButtonText(1, "�|�[�`��");
            UI.SetButtonOnClick(1, () => { owner.GetComponent<kichen_varSato>()._toPouch = true; owner.ChangeState<SelectMode>(); });
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Select].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.ItemIconList[(int)IconType.Select].DeleteButton();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Select].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<Close>();
        }
    }

    public class SelectMode : UIStateBase
    {
        int _memoryNum;
        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            if (prevState.GetType() == typeof(BoxorPouch))
            {
                _memoryNum = -1;
                //��������A�C�e���̃��X�g���쐬�[�[�[�[�[�[�[�[�[�[�[�[�[
                var UI = owner.ItemIconList[(int)IconType.Select];
                UI.SetText("���A�C�e��");
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


                //�{�^���������ꂽ�牽��邩�̃J�E���g�p�̃I�u�W�F�N�g���쐬
                for (int i = 0; i < _createItem.Count; i++)
                {
                    int numi = i;
                    UI.SetButtonText(numi, _createItem[numi].baseData.Name);
                    UI.SetButtonOnClick(numi, () =>
                    {
                        //������ȏ㐧��ł���Ȃ���I���Ɉڍs
                        if (owner.GetComponent<kichen_varSato>()._count.Check(owner)) owner.ChangeState<CountMode>();
                    });
                }
            }

        }
        public override void OnUpdate(UIBase owner)
        {
            var current = owner.ItemIconList[(int)IconType.Select].Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            //�J�����g�i���o�[���X�V���ꂽ��_cleateItemID���X�V����
            if (_memoryNum != current)
            {
                _memoryNum = current;
                var name = owner.ItemIconList[(int)IconType.Select].Buttons[current].GetComponentInChildren<Text>();
                foreach (var item in GameManager.Instance.ItemDataList.Dictionary.Values)
                {
                    if (item.baseData.Name != name.text) continue;
                    owner.GetComponent<kichen_varSato>()._cleateItemID = item.baseData.ID;
                }
                ChangeNeedMatrialButtons(owner);
            }
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            //�{�b�N�X���|�[�`���̑I���ɖ߂�Ȃ�{�^��������
            if (nextState.GetType() == typeof(BoxorPouch))
            {
                owner.ItemIconList[(int)IconType.Select].DeleteButton();
                owner.ItemIconList[(int)IconType.Confirmation].DeleteButton();
                owner.ItemIconList[(int)IconType.Needmaterial].DeleteButton();
            }
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ItemIconList[(int)IconType.Select].CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<BoxorPouch>();
        }

        /// <summary>
        /// �K�v�f�ނȂǂ�\��
        /// �{�^�����Đ������邽�ߕp�ɂɂ͌Ă΂Ȃ��悤��
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="_namespace"></param>
        private void ChangeNeedMatrialButtons(UIBase owner, int _namespace = 3)
        {
            //�K�v�f�ނ̃��X�g���쐬�[�[�[�[�[�[�[�[�[�[�[�[�[
            var list = owner.ItemIconList[(int)IconType.Needmaterial];
            list.DeleteButton();
            list.SetText("�f�ޖ��@�@�K�v�f�ށ^�f�ޏ�����");
            //���삷��A�C�e���ɕK�v�ȑf�ނ𒲂ׂ�
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _itemDataDic = GameManager.Instance.ItemDataList.Dictionary;

            list.SetTable(new Vector2(_itemDataDic[id].NeedMaterialLst.Count, 1));
            list.CreateButton();
            for (int i = 0; i < list.Buttons.Count; i++)
            {
                string materialID = _itemDataDic[id].NeedMaterialLst[i].materialID;
                string name = GameManager.Instance.MaterialDataList.Dictionary[materialID].Name;
                //���l�̎n�܂�𓯂��ɂ��邽�߂�_namespace���ɕ����������낦��
                for (int j = name.Length; j < _namespace; j++) name += "�@";

                string needNum = string.Format("{0,3:d}", _itemDataDic[id].NeedMaterialLst[i].requiredCount);
                string holdNum = string.Format("{0,4:d}", GameManager.Instance.MaterialDataList.Dictionary[materialID].PoachHoldNumber + GameManager.Instance.MaterialDataList.Dictionary[materialID].BoxHoldNumber);

                list.SetButtonText(i,"�@"+ name + Data.Convert.HanToZenConvert(needNum + "/" + holdNum));
            }
        }
    }
    public class CountMode : UIStateBase
    {

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            owner.GetComponent<kichen_varSato>()._count.Init(owner);
        }
        public override void OnUpdate(UIBase owner)
        {
            owner.GetComponent<kichen_varSato>()._count.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
            //�K�v�f�ނ̌��̍X�V
            UpdateNeedMatrialText(owner);
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            owner.GetComponent<kichen_varSato>()._count.Delete();
        }
        public override void OnProceed(UIBase owner)
        {
            owner.ChangeState<CleateMode>();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<SelectMode>();
        }
        private void UpdateNeedMatrialText(UIBase owner, int _namespace = 4)
        {
            //�K�v�f�ނ̃��X�g���쐬�[�[�[�[�[�[�[�[�[�[�[�[�[
            var list = owner.ItemIconList[(int)IconType.Needmaterial];
            //���삷��A�C�e���ɕK�v�ȑf�ނ𒲂ׂ�
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _itemDataDic = GameManager.Instance.ItemDataList.Dictionary;

            for (int i = 0; i < list.Buttons.Count; i++)
            {
                string materialID = _itemDataDic[id].NeedMaterialLst[i].materialID;
                string name = GameManager.Instance.MaterialDataList.Dictionary[materialID].Name;
                //���l�̎n�܂�𓯂��ɂ��邽�߂�_namespace���ɕ����������낦��
                for (int j = name.Length; j < _namespace; j++) name += "�@";

                string needNum = string.Format("{0,3:d}", _itemDataDic[id].NeedMaterialLst[i].requiredCount * owner.GetComponent<kichen_varSato>()._count.Now);
                string holdNum = string.Format("{0,4:d}", GameManager.Instance.MaterialDataList.Dictionary[materialID].PoachHoldNumber + GameManager.Instance.MaterialDataList.Dictionary[materialID].BoxHoldNumber);

                list.SetButtonText(i, "�@" + name + Data.Convert.HanToZenConvert(needNum + "/" + holdNum));
            }
        }

    }

    public class CleateMode : UIStateBase
    {
        private ItemIcon confimationIcon;

        public override void OnEnter(UIBase owner, UIStateBase prevState)
        {
            confimationIcon = owner.ItemIconList[(int)IconType.Confirmation];
            confimationIcon.SetText("�f�ނ�����ăA�C�e�������܂����H");
            confimationIcon.SetTable(new Vector2(1, 2));
            confimationIcon.CreateButton();

            confimationIcon.SetButtonText(0, "�͂�");
            confimationIcon.SetButtonOnClick(0, () =>
            {
                string resultStr = "";
                //���Y����A�C�e����ID
                string createItemID = owner.GetComponent<kichen_varSato>()._cleateItemID;
                //�A�C�e�����Y
                if (owner.GetComponent<kichen_varSato>()._toPouch)
                {
                    int number = owner.GetComponent<kichen_varSato>()._count.Now;
                    GameManager.Instance.ItemDataList.GetToPoach(createItemID, number, 0/*��*/);
                    var list = GameManager.Instance.ItemDataList.Dictionary[createItemID].NeedMaterialLst;
                    var materialValues = GameManager.Instance.MaterialDataList.Values;
                    var materialkeys = GameManager.Instance.MaterialDataList.Keys;
                    foreach (var item in list)
                    {
                        int cost = item.requiredCount * number;
                        int index = materialkeys.IndexOf(item.materialID);
                        var data = materialValues[index];
                        //��Ƀ|�[�`����K�v���������
                        if (data.PoachHoldNumber > 0)
                        {
                            if (data.PoachHoldNumber < cost)
                            {
                                cost = cost - data.PoachHoldNumber;
                                data.PoachHoldNumber = 0;
                            }
                            else
                            {
                                data.PoachHoldNumber = data.PoachHoldNumber - cost;
                                cost = 0;
                            }
                        }
                        //�{�b�N�X��������
                        data.BoxHoldNumber -= cost;
                        materialValues[index] = data;
                        GameManager.Instance.MaterialDataList.DesrializeDictionary();
                    }
                    resultStr = GameManager.Instance.ItemDataList.Dictionary[createItemID].baseData.Name + "��"
                                + Data.Convert.HanToZenConvert(number.ToString()) + "���Y���܂���";


                }
                else
                {
                    int number = owner.GetComponent<kichen_varSato>()._count.Now;
                    GameManager.Instance.ItemDataList.GetToBox(createItemID, number, 0/*��*/);
                    var list = GameManager.Instance.ItemDataList.Dictionary[createItemID].NeedMaterialLst;
                    var materialValues = GameManager.Instance.MaterialDataList.Values;
                    var materialkeys = GameManager.Instance.MaterialDataList.Keys;
                    foreach (var item in list)
                    {
                        int cost = item.requiredCount * number;
                        int index = materialkeys.IndexOf(item.materialID);
                        var data = materialValues[index];
                        //��Ƀ|�[�`����K�v���������
                        if (data.PoachHoldNumber > 0)
                        {
                            if (data.PoachHoldNumber < cost)
                            {
                                cost = cost - data.PoachHoldNumber;
                                data.PoachHoldNumber = 0;
                            }
                            else
                            {
                                data.PoachHoldNumber = data.PoachHoldNumber - cost;
                                cost = 0;
                            }
                        }
                        //�{�b�N�X��������
                        data.BoxHoldNumber -= cost;
                        materialValues[index] = data;
                        GameManager.Instance.MaterialDataList.DesrializeDictionary();
                    }
                    resultStr = GameManager.Instance.ItemDataList.Dictionary[createItemID].baseData.Name + "��"
                                + Data.Convert.HanToZenConvert(number.ToString()) + "���Y���܂���";

                }

                confimationIcon.DeleteButton();
                confimationIcon.SetText(resultStr);
                confimationIcon.SetTable(new Vector2(1, 1));
                confimationIcon.CreateButton();

                confimationIcon.SetButtonText(0, "OK");
                confimationIcon.SetButtonOnClick(0, () => owner.ChangeState<SelectMode>());
            });

            confimationIcon.SetButtonText(1, "������");
            confimationIcon.SetButtonOnClick(1, () => owner.ChangeState<CountMode>());
        }
        public override void OnUpdate(UIBase owner)
        {
            confimationIcon.Select(UISoundManager.Instance.InputSelection.ReadValue<Vector2>());
        }
        public override void OnExit(UIBase owner, UIStateBase nextState)
        {
            confimationIcon.DeleteButton();
            if (nextState.GetType() == typeof(SelectMode))
            {
                owner.GetComponent<kichen_varSato>()._count.Delete();
            }
        }
        public override void OnProceed(UIBase owner)
        {
            confimationIcon.CurrentButtonInvoke();
        }
        public override void OnBack(UIBase owner)
        {
            owner.ChangeState<CountMode>();
        }
    }

    //�J�E���g���邽�߂̃N���X
    public class Count
    {
        private GameObject countObject;
        private int min, max, now;
        private bool lockflg;

        public int Min { get => min; }
        public int Max { get => max; }
        public int Now { get => now; }

        /// <summary>
        /// �J�E���g�p��UI���쐬����ő�l�̐ݒ���s��
        /// </summary>
        /// <param name="owner"></param>
        public void Init(UIBase owner)
        {
            if (!Check(owner))
            {
                Debug.LogError("�K�v��������Ȃ�");
                return;
            }

            //�ő�l�ŏ��l�̏�����
            now = 1;
            min = 1;
            max = 9999;
            //�ő�l�̐ݒ�
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _ItemDic = GameManager.Instance.ItemDataList.Dictionary;
            var _materialDataDic = GameManager.Instance.MaterialDataList.Dictionary;
            foreach (var item in _ItemDic[id].NeedMaterialLst)
            {
                var materialNum = _materialDataDic[item.materialID].BoxHoldNumber + _materialDataDic[item.materialID].PoachHoldNumber;
                if (item.requiredCount <= 0) Debug.LogError("�K�v����0�ȉ��ł��B");
                if ((materialNum / item.requiredCount) < max)
                {
                    max = materialNum / item.requiredCount;
                }
            }
            //�J�E���g�p��UI�𐧍�A�ʒu�ݒ�
            countObject = Instantiate(Resources.Load("UI/Count"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            owner.ItemIconList[(int)IconType.Select].AdjustmentImage(countObject.GetComponent<RectTransform>());
        }
        /// <summary>
        /// �J�E���g�p��UI���폜
        /// </summary>
        public void Delete()
        {
            Destroy(countObject);
        }
        /// <summary>
        /// �J�E���g�A�b�v�_�E��
        /// </summary>
        /// <param name="vec"></param>
        public void Select(Vector2 vec)
        {
            if (vec.sqrMagnitude > 0)
            {
                if (lockflg == false)
                {
                    if (vec.y > 0) now++;
                    else now--;
                    now = Mathf.Clamp(now, min, max);
                    lockflg = true;

                    countObject.GetComponentInChildren<Text>().text = now.ToString();
                }
            }
            else
            {
                lockflg = false;
            }

        }
        /// <summary>
        /// ����\�Ȃ�ture��Ԃ�
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public bool Check(UIBase owner)
        {
            //�ő�l�̐ݒ�
            string id = owner.GetComponent<kichen_varSato>()._cleateItemID;
            var _ItemDic = GameManager.Instance.ItemDataList.Dictionary;
            var _materialDataDic = GameManager.Instance.MaterialDataList.Dictionary;
            foreach (var item in _ItemDic[id].NeedMaterialLst)
            {
                var materialNum = _materialDataDic[item.materialID].BoxHoldNumber + _materialDataDic[item.materialID].PoachHoldNumber;
                if (item.requiredCount <= 0) Debug.LogError("�K�v����0�ȉ��ł��B");
                //����o���Ȃ�
                if ((materialNum / item.requiredCount) <= 0) return false;
            }
            return true;
        }
    }

    //�A�C�R���▼�O�A���ʁi�t���[�o�[�e�L�X�g�j���Ǘ�����N���X
    public class ItemExplanation
    {
        private GameObject Icon;
        private GameObject name;
        private GameObject effect;

        public void Set(ItemData itemData, UIBase owner)
        {
            //�A�C�R��
            if (Icon == null) Icon = Instantiate(Resources.Load("UI/Image3"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //�v�ʒu����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            Icon.GetComponent<Image>().sprite = Resources.Load<Sprite>(itemData.baseData.IconName);

            //���O
            if (name == null) name = Instantiate(Resources.Load("UI/Image3"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //�v�ʒu����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            name.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, 50);
            //name.GetComponent<Image>().sprite = Resources.Load<Sprite>("�w�i��ς�������΂����Ƀp�X������");
            name.GetComponentInChildren<Text>().text = itemData.baseData.Name;

            //������
            if (effect == null) effect = Instantiate(Resources.Load("UI/Image3"), GameManager.Instance.ItemCanvas.Canvas.transform) as GameObject;
            //�v�ʒu����!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            effect.GetComponent<RectTransform>().anchoredPosition = new Vector2(-50, 100);
            //name.GetComponent<Image>().sprite = Resources.Load<Sprite>("�w�i��ς�������΂����Ƀp�X������");
            effect.GetComponentInChildren<Text>().text = itemData.FlavorText;

        }

        public void Delete()
        {
            //�A�C�R��
            if (Icon != null) Destroy(Icon);
            //���O
            if (name != null) Destroy(name);
            //������
            if (effect != null) Destroy(effect);
        }
    }

}

