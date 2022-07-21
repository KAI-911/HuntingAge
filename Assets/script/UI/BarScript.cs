using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarScript : MonoBehaviour
{
    [SerializeField] Image _fill;
    [SerializeField] Image _back;
    [SerializeField] Slider _slider;
    public void SetFillColor(Color color) { _fill.color = color; }
    public void SetBackColor(Color color) { _back.color = color; }
    public void SetSlisder(float value) { _slider.value = value; }
    public void SetMaxSlider(float maxvalue) { _slider.maxValue = maxvalue; }
    public void SetMinValue(float minValue) { _slider.minValue = minValue; }
    public void SetRectTransform(Vector2 vector2)
    {
        Debug.Log(vector2);
        var rect = GetComponent<RectTransform>();
        rect.anchoredPosition = vector2;
    }
    public Vector2 GetRectTransform()
    {
        var rect = GetComponent<RectTransform>();
        return rect.anchoredPosition;
    }
    public Vector2 GetRectTransformSize()
    {
        var rect = GetComponent<RectTransform>();
        return rect.sizeDelta;

    }
}
