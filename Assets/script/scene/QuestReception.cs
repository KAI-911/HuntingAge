using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class QuestReception : MonoBehaviour
{
    [SerializeField] Text _questName;
    [SerializeField] Text _questContents;
    [SerializeField] TargetChecker _targetChecker;
    [SerializeField] Canvas _canvas;

    //�C���v�b�g�V�X�e��
    private InputControls _input;
    private InputAction _inputAction;
    public InputAction InputMoveAction { get => _inputAction; }

    void Start()
    {
        _canvas.enabled = false;
    }
    private void OnEnable()
    {
        _inputAction = _input.UI.Selection;
        _input.UI.Proceed.started += Proceed;
        _input.UI.Back.started += Back;
        _input.UI.Enable();
    }


    private void OnDisable()
    {
        _input.UI.Proceed.started -= Proceed;
        _input.UI.Back.started -= Back;

        _input.UI.Disable();
    }

    void Update()
    {
        _inputAction.ReadValue<Vector2>();
        if (_targetChecker.TriggerHit)
        {

        }
    }

    public void SelectQuest_Rec(string QuestID)
    {
        GameManager.Instance.Quest.QusetSelect(QuestID);
        var data = GameManager.Instance.Quest.QuestData;
        string str = "";
        switch (data.Clear)
        {
            case ClearConditions.TargetSubjugation:
                Dictionary<string, int> list = new Dictionary<string, int>();
                foreach (var enemy in data.TargetName)
                {
                    //���ɒǉ�����Ă���ꍇ
                    if(list.ContainsKey(enemy))
                    {
                        list[enemy]++;
                    }
                    else
                    {
                        list.Add(enemy, 1);
                    }
                }

                str += "�N���A����: ";
                foreach (var item in list)
                {
                    var tmp = SaveData.GetClass<EnemyData>(item.Key, new EnemyData());

                    str += tmp.DisplayName + "��" + item.Value + "�̓�������\n";
                }
                break;
            case ClearConditions.Gathering:
                break;
            default:
                break;
        }
        str += "���s����: " + (int)(data.Failure + 1) + "��͐s����\n";
        switch (data.Field)
        {
            case Scene.Forest:
                str += "���: �X��";
                break;
            case Scene.Animal:
                str += "���: �����p";
                break;
            default:
                break;
        }
        _questName.text = data.Name;
        _questContents.text = str;

    }
    public void GoToQuest_Rec()
    {
        GameManager.Instance.Quest.GoToQuset();
    }

    private void Back(InputAction.CallbackContext obj)
    {
        
    }

    private void Proceed(InputAction.CallbackContext obj)
    {
        
    }



}
