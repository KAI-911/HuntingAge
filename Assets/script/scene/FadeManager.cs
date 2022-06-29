using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
public class FadeManager : MonoBehaviour
{
    [SerializeField] Canvas _canvas;
    [SerializeField] Color _color;
    [SerializeField] float _fadeTime;
    [SerializeField] bool _isFadeIn;
    [SerializeField] bool _isFadeOut;
    [SerializeField] bool _isFinish;
    private Image panel;
    private Action _action;
    public bool IsFinish { get => _isFinish; }


    public void SetFadeTime(float fadeTime)
    {
        _fadeTime = fadeTime;
    }

    public void FadeInStart()
    {
        _isFadeIn = true;
        _isFinish = false;
    }

    public void FadeOutStart(Action action)
    {
        _isFadeOut = true;
        _isFinish = false;
        _action = action;
    }

    private void Awake()
    {
        _isFadeIn = false;
        _isFadeOut = false;
        panel = GetComponentInChildren<Image>();
        _color.a = 0;
        panel.color = _color;
    }
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }


    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    //�V�[���؂�ւ����ɌĂ΂��
    private void OnSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        FadeInStart();
    }

    private void Update()
    {
        if (_isFinish) return;

        if (_isFadeIn)
        {
            //�o�ߎ��Ԃ��瓧���x�v�Z
            _color.a -= Time.deltaTime / _fadeTime;
            Debug.Log("_isFadeIn");
            //�t�F�[�h�C���I������
            if (_color.a <= 0.0f)
            {
                _isFadeIn = false;
                _isFinish = true;
                _color.a = 0.0f;
            }
            panel.color = _color;
        }
        else
        {
            if (_isFadeOut)
            {
                //�o�ߎ��Ԃ��瓧���x�v�Z
                _color.a += Time.deltaTime / _fadeTime;
                Debug.Log("_isFadeOut");
                //�t�F�[�h�C���I������
                if (_color.a >= 1.0f)
                {
                    _isFadeOut = false;
                    _color.a = 1.0f;
                    _action.Invoke();
                }
                panel.color = _color;
            }
        }
    }
}
