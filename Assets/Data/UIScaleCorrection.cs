using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScaleCorrection : MonoBehaviour
{
    Vector2 _baseScreen;
    [SerializeField] RectTransform[] _rectTransforms;
    // Start is called before the first frame update
    void Start()
    {
        _baseScreen = new Vector2(1280, 720);
        Correction();
    }

    public void Correction()
    {
        Vector2 _magnification;
        _magnification.x = Screen.width / _baseScreen.x;
        _magnification.y = Screen.height / _baseScreen.y;
        foreach (var item in _rectTransforms)
        {
            Vector3 scale;
            scale.x = item.localScale.x * _magnification.x;
            scale.y = item.localScale.y * _magnification.y;
            scale.z = item.localScale.z;
            item.localScale = scale;
        }
    }

}
