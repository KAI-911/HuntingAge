using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemIcon : MonoBehaviour
{

    [SerializeField] List<GameObject> _ItemBoxButtons;
    [SerializeField] Vector2 _leftTopPos;
    [SerializeField] float _padding;
    [SerializeField] Vector2 _tableSize;
    [SerializeField] bool _canBeChanged;
    [SerializeField] bool _buttonBack;
    [SerializeField] bool _textFlg;
    [SerializeField] GameObject _buttonPrefab;
    [SerializeField] GameObject _buttonBackPrefab;
    [SerializeField] GameObject _textPrefab;
    [SerializeField] TEXT _textData;
    private GameObject _buttonBackObj;
    private GameObject _textObj;
    private RunOnce _once = new RunOnce();
    private int _currentNunber;
    public List<GameObject> Buttons { get => _ItemBoxButtons; set => _ItemBoxButtons = value; }
    public Vector2 TableSize { get => _tableSize; set => _tableSize = value; }
    public int CurrentNunber { get => _currentNunber; }
    public int GetSize { get => (int)_tableSize.x * (int)_tableSize.y; }

    public bool WithinRange()
    {
        if (GetSize == 0) return false;
        if (0 <= _currentNunber && _currentNunber < GetSize)
        {
            return true;
        }
        return false;
    }
    private void Awake()
    {
        _currentNunber = 0;
    }

    private void Update()
    {
        if (_canBeChanged)
        {
            for (int i = 0; i < _ItemBoxButtons.Count; i++)
            {
                int w = Mathf.Abs((i % (int)_tableSize.y) * (int)_padding);
                int h = Mathf.Abs((i / (int)_tableSize.y) * (int)_padding);
                _ItemBoxButtons[i].transform.position = new Vector3(_leftTopPos.x + w, _leftTopPos.y - h, 0);
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


    [ContextMenu("createbutton")]
    public List<GameObject> CreateButton(int currentNum = 0)
    {
        DeleteButton();
        var c = GameManager.Instance.ItemCanvas.Canvas;
        var buttonSize = _buttonPrefab.GetComponent<RectTransform>().sizeDelta;

        //背面画像
        if (_buttonBack)
        {
            //幅を決める
            Vector2 size = new Vector2();
            size.x = _tableSize.y * (buttonSize.x + _padding) + _padding;
            size.y = _tableSize.x * (buttonSize.y + _padding) + _padding;

            //位置を設定・調整
            var pos = new Vector3(_leftTopPos.x, _leftTopPos.y, 0);
            pos.x -= _padding;
            pos.y += _padding;

            //実体化
            _buttonBackObj = Instantiate(_buttonBackPrefab, pos, Quaternion.identity);
            //親の設定
            _buttonBackObj.transform.SetParent(c.transform);
            //幅の設定
            _buttonBackObj.GetComponent<RectTransform>().sizeDelta = size;
        }

        //テキスト
        if (_textFlg)
        {
            if (_buttonBack)
            {
                //背面画像の位置調整
                var backpos = _buttonBackObj.transform.position;
                backpos.y += _textData.hight + _padding;
                _buttonBackObj.transform.position = backpos;
                //背面画像の幅調整
                var backsize = _buttonBackObj.GetComponent<RectTransform>().sizeDelta;
                backsize.y += _textData.hight + _padding;
                _buttonBackObj.GetComponent<RectTransform>().sizeDelta = backsize;
            }

            //幅を決める
            Vector2 size = new Vector2();
            size.x = _tableSize.y * (buttonSize.x + _padding) + _padding;
            size.y = _textData.hight;

            //位置を設定
            var pos = new Vector3(_leftTopPos.x, _leftTopPos.y, 0);
            pos.x -= _padding;
            pos.y += (_textData.hight + _padding);


            //実体化
            _textObj = Instantiate(_textPrefab, pos, Quaternion.identity);
            //親の設定
            _textObj.transform.SetParent(c.transform);
            //幅の設定
            _textObj.GetComponent<RectTransform>().sizeDelta = size;

            //テキストの各種設定
            var text = _textObj.GetComponent<Text>();
            text.text = _textData.text;
            text.fontSize = _textData.fontSize;
            if(_textData.autoFontSize)
            {
                text.resizeTextForBestFit = true;
            }
            text.color = _textData.color;
            text.alignment = _textData.textAnchor;
        }

        for (int i = 0; i < (int)(_tableSize.x * _tableSize.y); i++)
        {
            int w = Mathf.Abs((i % (int)_tableSize.y) * (int)(buttonSize.x + _padding));
            int h = Mathf.Abs((i / (int)_tableSize.y) * (int)(buttonSize.y + _padding));
            var obj = Instantiate(_buttonPrefab, new Vector3(_leftTopPos.x + w, _leftTopPos.y - h, 0), Quaternion.identity);
            obj.transform.SetParent(c.transform);
            _ItemBoxButtons.Add(obj);
        }
        _currentNunber = currentNum;
        return _ItemBoxButtons;
    }
    public void DeleteButton()
    {
        foreach (var item in _ItemBoxButtons)
        {
            Destroy(item);
        }
        _ItemBoxButtons.Clear();
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
