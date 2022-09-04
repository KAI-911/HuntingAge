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
    [SerializeField] Image panel;

    private Action _fadeOutaction;
    private Action _fadeInaction;
    private bool _fadeInactionFlg;
    public bool IsFinish { get => _isFinish; }


    public void SetFadeTime(float fadeTime)
    {
        _fadeTime = fadeTime;
    }

    public void FadeInStart()
    {
        _isFadeIn = true;
        _isFinish = false;
        _fadeInactionFlg = false;
    }
    public void FadeInStart(Action action)
    {
        _isFadeIn = true;
        _isFinish = false;
        _fadeInaction = action;
        _fadeInactionFlg = true;
    }

    public void FadeOutStart(Action action)
    {
        _canvas.enabled = true;
        _isFadeOut = true;
        _isFinish = false;
        _fadeOutaction = action;
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        _fadeInactionFlg = false;
        _isFadeIn = false;
        _isFadeOut = false;
        if (panel == null)
        {
            panel = GetComponentInChildren<Image>();
        }
        _color.a = 0;
        panel.color = _color;
        _canvas.enabled = false;
        _fadeInaction = () => { };
        _fadeOutaction = () => { };
    }
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }


    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    //シーン切り替え時に呼ばれる
    private void OnSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        FadeInStart();
    }

    private void Update()
    {
        if (_isFinish) return;

        if (_isFadeIn)
        {
            //経過時間から透明度計算
            _color.a -= Time.deltaTime / _fadeTime;
            //フェードイン終了判定
            if (_color.a <= 0.0f)
            {
                _isFadeIn = false;
                _isFinish = true;
                _canvas.enabled = false;
                _color.a = 0.0f;
                if (_fadeInactionFlg) _fadeInaction.Invoke();
            }
            panel.color = _color;
        }
        else if (_isFadeOut)
        {

            //経過時間から透明度計算
            _color.a += Time.deltaTime / _fadeTime;
            Debug.Log("_isFadeOut");
            //フェードイン終了判定
            if (_color.a >= 1.0f)
            {
                _isFadeOut = false;
                _color.a = 1.0f;
                _fadeOutaction.Invoke();
            }
            panel.color = _color;
        }
    }
}
