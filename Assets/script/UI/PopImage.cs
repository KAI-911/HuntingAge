using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopImage : MonoBehaviour
{
    [SerializeField] protected GameObject _imagePrefab;
    [SerializeField] protected TargetChecker _targetChecker;
    [SerializeField] protected GameObject _image;
    [SerializeField] Sprite _spritePrefab;
    [SerializeField] string _text;

    public TargetChecker TargetChecker { get => _targetChecker; }

    public void Awake()
    {
        _targetChecker = GetComponentInChildren<TargetChecker>();
    }
    public virtual void Update()
    {
        if (_image != null)
        {
            if (!UISoundManager.Instance._player.IsAction)
            {
                _image.SetActive(false);
            }
            else
            {
                _image.SetActive(true);
            }
        }
    }
    private void OnDisable()
    {
        if (_image != null)
        {
            Destroy(_image);
        }
        if (UISoundManager.Instance != null && UISoundManager.Instance._player.CollectionScript == this)
        {
            UISoundManager.Instance._player.CollectionScript = null;
        }
    }
    public void OnDestroy()
    {
        if (_image != null)
        {
            Destroy(_image);
        }
    }

    public virtual void CreateImage()
    {
        _image = Instantiate(_imagePrefab);
        _image.transform.SetParent(GameManager.Instance.ItemCanvas.Canvas.transform);
        var rext = _image.GetComponent<RectTransform>();
        rext.anchoredPosition = new Vector2(0, rext.sizeDelta.y);
        var text = _image.GetComponentInChildren<Text>();
        text.text = _text;
        var icon = _image.GetComponentsInChildren<Image>();
        icon[1].sprite = _spritePrefab;
        //icon[1]なのは一番後ろのイメージとアイコンの2つのスプライトがあるため
    }
    public virtual void DeleteImage()
    {
        if (_image != null)
        {
            Destroy(_image);
        }
    }

}
