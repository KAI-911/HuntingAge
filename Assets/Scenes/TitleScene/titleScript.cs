using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;

public class titleScript : MonoBehaviour
{
    [SerializeField] FadeManager _fadeManager;
    [SerializeField] Scene _scene;
    [SerializeField] private InputControls _input;

    bool _anyKey;
    void Start()
    {
        _input = new InputControls();
        _anyKey = false;
        _input.UI.Proceed.started += Next;

        _fadeManager.FadeInStart();
    }
    private void OnDestroy()
    {
        _input.UI.Proceed.started -= Next;
    }
    private void Update()
    {
        Debug.Log("aasda");

    }
    private void Next(InputAction.CallbackContext obj)
    {
        Debug.Log("‰Ÿ‚³‚ê‚½1");
        if (_anyKey == false)
        {
            Debug.Log("‰Ÿ‚³‚ê‚½2");

            _anyKey = true;
            _fadeManager.FadeOutStart(() =>
            {
                Debug.Log("‰Ÿ‚³‚ê‚½3");

                SceneManager.LoadScene((int)_scene);
                _fadeManager.FadeInStart(() => Destroy(this));
            });
        }


    }

}
