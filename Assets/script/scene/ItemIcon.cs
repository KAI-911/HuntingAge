using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class ItemIcon : MonoBehaviour
{

    [SerializeField] List<GameObject> _Buttons;
    [SerializeField] ItemIconData _iconData;

    private GameObject _buttonBackObj;
    private GameObject _textObj;
    private RunOnce _once = new RunOnce();
    private int _currentNunber;
    public List<GameObject> Buttons { get => _Buttons; set => _Buttons = value; }
    public Vector2 TableSize { get => _iconData._tableSize; set => _iconData._tableSize = value; }
    public int CurrentNunber { get => _currentNunber; set => _currentNunber = value; }
    public int GetSize { get => (int)_iconData._tableSize.x * (int)_iconData._tableSize.y; }
    public ItemIconData IconData { get => _iconData; }
    public GameObject TextObj { get => _textObj; }
    public GameObject ButtonBackObj { get => _buttonBackObj; }

    public bool WithinRange()
    {
        if (GetSize == 0) return false;
        if (0 <= _currentNunber && _currentNunber < GetSize)
        {
            return true;
        }
        return false;
    }
    public void SetIcondata(string _key)
    {
        _iconData = UIManager.Instance.UIPresetData.Dictionary[_key];
    }
    public void SetIcondata(ItemIconData _value)
    {
        _iconData = _value;
    }

    public void SetText(string _text)
    {
        this._iconData._textData.text = _text;
    }
    public void SetTable(Vector2 _table)
    {
        var iconData = this.IconData;
        iconData._tableSize = _table;
        this.SetIcondata(iconData);
    }
    public void SetLeftTopPos(Vector2 vector2)
    {
        var iconData = this.IconData;
        iconData._leftTopPos = vector2;
        this.SetIcondata(iconData);
    }
    public bool SetButtonText(int _buttonNumber, string _taxt)
    {
        if (_buttonNumber < 0 || _buttonNumber > GetSize) return false;
        Buttons[_buttonNumber].GetComponentInChildren<Text>().text = _taxt;
        return true;
    }

    public void SetButtonOnClick(int _buttonNumber, UnityAction action)
    {
        if (_buttonNumber < 0 || _buttonNumber > GetSize) return;
        var button = Buttons[_buttonNumber].GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
    public void CurrentButtonInvoke()
    {
        Buttons[_currentNunber].GetComponent<Button>().onClick.Invoke();
    }
    public void AdjustmentImage(RectTransform rectTransform)
    {
        AdjustmentImage(rectTransform, CurrentNunber);
    }
    public bool AdjustmentImage(RectTransform rectTransform, int currentNunber)
    {
        if (currentNunber < 0 || currentNunber > GetSize) return false;
        //currentNunber???{?^????UI???W??????
        var rect = Buttons[currentNunber].GetComponent<RectTransform>();
        //currentNunber???{?^????UI???W??????????????
        rectTransform.anchoredPosition = new Vector2(rect.anchoredPosition.x + rect.sizeDelta.x + _iconData._padding, rect.anchoredPosition.y);
        return true;
    }
    public Vector2 ImagePos(int currentNunber)
    {
        Vector2 returnVec = new Vector2();
        if (currentNunber < 0 || currentNunber > GetSize) return returnVec;
        //currentNunber???{?^????UI???W??????
        var rect = Buttons[currentNunber].GetComponent<RectTransform>();
        //currentNunber???{?^????UI???W??????????????
        returnVec = new Vector2(rect.anchoredPosition.x + rect.sizeDelta.x + _iconData._padding, rect.anchoredPosition.y);
        return returnVec;
    }
    public Vector2 ImagePos()
    {
        Vector2 returnVec = new Vector2();
        if (CurrentNunber < 0 || CurrentNunber > GetSize) return returnVec;
        //currentNunber???{?^????UI???W??????
        var rect = Buttons[CurrentNunber].GetComponent<RectTransform>();
        //currentNunber???{?^????UI???W??????????????
        returnVec = new Vector2(rect.anchoredPosition.x + rect.sizeDelta.x + _iconData._padding, rect.anchoredPosition.y);
        return returnVec;
    }
    private void Awake()
    {
        _currentNunber = 0;
    }

    private void Update()
    {
        if (_iconData._canBeChanged)
        {
            var buttonSize = _iconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
            for (int i = 0; i < _Buttons.Count; i++)
            {
                int w = Mathf.Abs((i % (int)_iconData._tableSize.y) * (int)(buttonSize.x + _iconData._padding));
                int h = Mathf.Abs((i / (int)_iconData._tableSize.y) * (int)(buttonSize.y + _iconData._padding));
                _Buttons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(_iconData._leftTopPos.x + w, _iconData._leftTopPos.y - h);
            }
        }
    }

    public int Select(Vector2 vector2)
    {
        if (!WithinRange())
        {
            return 0;
        }

        //??????
        if (vector2.sqrMagnitude > 0)
        {
            if (_once.Flg) return _currentNunber;
            if (Mathf.Abs(vector2.x) > 0)
            {
                int i = _currentNunber % (int)TableSize.y;
                if (vector2.x > 0)
                {
                    if (i != ((int)TableSize.y - 1))
                    {
                        _currentNunber++;
                    }
                }
                else if (i != 0)
                {
                    _currentNunber--;
                }

            }
            if (Mathf.Abs(vector2.y) > 0)
            {
                int i = _currentNunber / (int)TableSize.y;

                if (vector2.y > 0)
                {
                    if (i != 0)
                    {
                        _currentNunber -= (int)TableSize.y;
                    }
                }
                else if (i != (int)TableSize.x - 1)
                {
                    _currentNunber += (int)TableSize.y;
                }

            }
            _once.Flg = true;
        }
        else
        {
            _once.Flg = false;
        }

        if (WithinRange())
        {
            Buttons[CurrentNunber].GetComponent<Button>().Select();
        }

        return _currentNunber;
    }


    /// <summary>
    /// ??????????????????-1??????
    /// </summary>
    /// <returns></returns>
    public int FirstNotSetNumber()
    {
        for (int i = 0; i < _Buttons.Count; i++)
        {
            var itembutton = _Buttons[i].GetComponent<ItemButton>();
            if (itembutton.ID == "") return i;
        }
        return -1;
    }

    [ContextMenu("CreateButton")]
    public List<GameObject> CreateButton(int currentNum = 0)
    {
        DeleteButton();
        var c = GameManager.Instance.ItemCanvas.Canvas;
        var buttonSize = _iconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;
        var _buttonBackObjRect = new Vector2(_iconData._leftTopPos.x, _iconData._leftTopPos.y);
        //?w??????
        if (_iconData._buttonBack)
        {
            //??????????
            Vector2 size = new Vector2();
            size.x = _iconData._tableSize.y * (buttonSize.x + _iconData._padding) + _iconData._padding;
            size.y = _iconData._tableSize.x * (buttonSize.y + _iconData._padding) + _iconData._padding;

            //???u??????
            _buttonBackObjRect.x = _iconData._leftTopPos.x - _iconData._padding;
            _buttonBackObjRect.y = _iconData._leftTopPos.y + _iconData._padding;
            //??????
            _buttonBackObj = Instantiate(_iconData._buttonBackPrefab);
            //?e??????
            _buttonBackObj.transform.SetParent(c.transform);
            var rect = _buttonBackObj.GetComponent<RectTransform>();
            //????????
            rect.sizeDelta = size;
            //???u??????
            rect.anchoredPosition = _buttonBackObjRect;
        }

        //?e?L?X?g
        if (_iconData._textFlg)
        {
            if (_iconData._buttonBack)
            {
                //?w???????????u????
                _buttonBackObjRect.y += _iconData._textData.hight + _iconData._padding;
                _buttonBackObj.GetComponent<RectTransform>().anchoredPosition = _buttonBackObjRect;
                //?w??????????????
                var backsize = _buttonBackObj.GetComponent<RectTransform>().sizeDelta;
                backsize.y += _iconData._textData.hight + _iconData._padding;
                _buttonBackObj.GetComponent<RectTransform>().sizeDelta = backsize;
            }

            //??????????
            Vector2 size = new Vector2();
            size.x = _iconData._tableSize.y * (buttonSize.x + _iconData._padding) + _iconData._padding;
            size.y = _iconData._textData.hight;

            //???u??????
            var pos = new Vector2(_iconData._leftTopPos.x, _iconData._leftTopPos.y);
            pos.x -= _iconData._padding;
            pos.y += (_iconData._textData.hight + _iconData._padding);


            //??????
            _textObj = Instantiate(_iconData._textPrefab);
            //?e??????
            _textObj.transform.SetParent(c.transform);
            //????????
            _textObj.GetComponent<RectTransform>().sizeDelta = size;
            //???u??????
            _textObj.GetComponent<RectTransform>().anchoredPosition = pos;
            //?e?L?X?g???e??????
            var text = _textObj.GetComponent<Text>();
            text.text = _iconData._textData.text;
            text.fontSize = _iconData._textData.fontSize;
            if (_iconData._textData.autoFontSize)
            {
                text.resizeTextForBestFit = true;
            }
            text.color = _iconData._textData.color;
            text.alignment = _iconData._textData.textAnchor;
        }

        //?{?^??
        for (int i = 0; i < (int)(_iconData._tableSize.x * _iconData._tableSize.y); i++)
        {
            int w = Mathf.Abs((i % (int)_iconData._tableSize.y) * (int)(buttonSize.x + _iconData._padding));
            int h = Mathf.Abs((i / (int)_iconData._tableSize.y) * (int)(buttonSize.y + _iconData._padding));
            var obj = Instantiate(_iconData._buttonPrefab);
            obj.transform.SetParent(c.transform);
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(_iconData._leftTopPos.x + w, _iconData._leftTopPos.y - h);
            _Buttons.Add(obj);
        }
        _currentNunber = currentNum;
        return _Buttons;
    }
    public void DeleteButton()
    {
        foreach (var item in _Buttons)
        {
            Destroy(item);
        }
        _Buttons.Clear();
        if (_buttonBackObj != null)
        {
            Destroy(_buttonBackObj);
            _buttonBackObj = null;
        }
        if (_textObj != null)
        {
            Destroy(_textObj);
            _textObj = null;
        }
    }


    //CurrentNunber???A?C?e?????Z?b?g????????????
    public bool CheckCurrentNunberItem()
    {
        if (!WithinRange()) return false;

        var button = _Buttons[CurrentNunber].GetComponent<ItemButton>();
        if (button.ID == "") return false;
        return true;
    }

}

[System.Serializable]
public struct TEXT
{
    public int fontSize;
    public bool autoFontSize;
    public string text;
    public Color color;
    public float hight;
    public float margin;
    public TextAnchor textAnchor;
}
