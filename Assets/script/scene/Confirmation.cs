using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;

public class Confirmation : MonoBehaviour
{
    //Confirmation 確認

    [SerializeField] Canvas _popUp;
    [SerializeField] Button _proceedButton;
    [SerializeField] Button _backButton;
    [SerializeField] Text _infoText;
    private Button _currntButton;
    //インプットシステム
    [SerializeField] private InputControls _input;
    private InputAction _inputAction;
    public InputAction InputAction { get => _inputAction; }

    private void Awake()
    {
        _input = new InputControls();
        _popUp.enabled = true;
        _proceedButton.onClick.RemoveAllListeners();
        _backButton.onClick.RemoveAllListeners();
        _infoText.text = "";
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        _inputAction = _input.UI.Selection;
        _input.UI.Proceed.started += Proceed;

        _input.UI.Back.started += Back;

        _input.UI.Enable();

        _currntButton = _proceedButton;
        _proceedButton.Select();
    }


    private void OnDisable()
    {
        _input.UI.Proceed.started -= Proceed;
        _input.UI.Back.started -= Back;

        _input.UI.Disable();
    }

    public void Selecte()
    {
        //選択できるようにしておく
        float v = _inputAction.ReadValue<Vector2>().x;
        if (v < 0) _currntButton = _proceedButton;
        if (v > 0) _currntButton = _backButton;
        _currntButton.Select();
    }

    public void SetProceedButton(UnityAction action)
    {
        _proceedButton.onClick.RemoveAllListeners();
        _proceedButton.onClick.AddListener(action);
    }
    public void SetBackButton(UnityAction action)
    {
        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(action);
    }
    public void SetText(string str)
    {
        _infoText.text = str;
    }
    private void Back(InputAction.CallbackContext obj)
    {
        _backButton.onClick.Invoke();
    }

    private void Proceed(InputAction.CallbackContext obj)
    {
        _currntButton.onClick.Invoke();
    }

}
