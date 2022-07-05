using System.Collections;
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
    public int CurrentNunber { get => _currentNunber; }
    public int GetSize { get => (int)_iconData._tableSize.x * (int)_iconData._tableSize.y; }

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
                _Buttons[i].transform.position = new Vector3(_iconData._leftTopPos.x + w, _iconData._leftTopPos.y - h, 0);
            }
        }
    }

    public int Select(Vector2 vector2)
    {
        //横方向
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
    /// 見つからない場合は-1を返す
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

    [ContextMenu("createbutton")]
    public List<GameObject> CreateButton(int currentNum = 0)
    {
        DeleteButton();
        var c = GameManager.Instance.ItemCanvas.Canvas;
        var buttonSize = _iconData._buttonPrefab.GetComponent<RectTransform>().sizeDelta;

        //背面画像
        if (_iconData._buttonBack)
        {
            //幅を決める
            Vector2 size = new Vector2();
            size.x = _iconData._tableSize.y * (buttonSize.x + _iconData._padding) + _iconData._padding;
            size.y = _iconData._tableSize.x * (buttonSize.y + _iconData._padding) + _iconData._padding;

            //位置を設定・調整
            var pos = new Vector3(_iconData._leftTopPos.x, _iconData._leftTopPos.y, 0);
            pos.x -= _iconData._padding;
            pos.y += _iconData._padding;

            //実体化
            _buttonBackObj = Instantiate(_iconData._buttonBackPrefab, pos, Quaternion.identity);
            //親の設定
            _buttonBackObj.transform.SetParent(c.transform);
            //幅の設定
            _buttonBackObj.GetComponent<RectTransform>().sizeDelta = size;
        }

        //テキスト
        if (_iconData._textFlg)
        {
            if (_iconData._buttonBack)
            {
                //背面画像の位置調整
                var backpos = _buttonBackObj.transform.position;
                backpos.y += _iconData._textData.hight + _iconData._padding;
                _buttonBackObj.transform.position = backpos;
                //背面画像の幅調整
                var backsize = _buttonBackObj.GetComponent<RectTransform>().sizeDelta;
                backsize.y += _iconData._textData.hight + _iconData._padding;
                _buttonBackObj.GetComponent<RectTransform>().sizeDelta = backsize;
            }

            //幅を決める
            Vector2 size = new Vector2();
            size.x = _iconData._tableSize.y * (buttonSize.x + _iconData._padding) + _iconData._padding;
            size.y = _iconData._textData.hight;

            //位置を設定
            var pos = new Vector3(_iconData._leftTopPos.x, _iconData._leftTopPos.y, 0);
            pos.x -= _iconData._padding;
            pos.y += (_iconData._textData.hight + _iconData._padding);


            //実体化
            _textObj = Instantiate(_iconData._textPrefab, pos, Quaternion.identity);
            //親の設定
            _textObj.transform.SetParent(c.transform);
            //幅の設定
            _textObj.GetComponent<RectTransform>().sizeDelta = size;

            //テキストの各種設定
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

        //ボタン
        for (int i = 0; i < (int)(_iconData._tableSize.x * _iconData._tableSize.y); i++)
        {
            int w = Mathf.Abs((i % (int)_iconData._tableSize.y) * (int)(buttonSize.x + _iconData._padding));
            int h = Mathf.Abs((i / (int)_iconData._tableSize.y) * (int)(buttonSize.y + _iconData._padding));
            var obj = Instantiate(_iconData._buttonPrefab, new Vector3(_iconData._leftTopPos.x + w, _iconData._leftTopPos.y - h, 0), Quaternion.identity);
            obj.transform.SetParent(c.transform);
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


    //CurrentNunberにアイテムがセットされているか
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
