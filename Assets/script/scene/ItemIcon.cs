using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemIcon : MonoBehaviour
{

    [SerializeField] List<GameObject> _ItemBoxButtons;
    [SerializeField] Vector2 _leftTopPos;
    [SerializeField] Vector2 _width;
    [SerializeField] Vector2 _tableSize;
    [SerializeField] bool _canBeChanged;
    [SerializeField] bool _buttonBack;
    [SerializeField] bool _texture;
    [SerializeField] GameObject _buttonPrefab;
    [SerializeField] GameObject _buttonBackPrefab;
    private GameObject _buttonBackObj;
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
                int w = Mathf.Abs((i % (int)_tableSize.y) * (int)_width.x);
                int h = Mathf.Abs((i / (int)_tableSize.y) * (int)_width.y);
                _ItemBoxButtons[i].transform.position = new Vector3(_leftTopPos.x + w, _leftTopPos.y - h, 0);
            }
        }
    }

    public int Select(Vector2 vector2)
    {
        //‰¡•ûŒü
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
        if (_buttonBack)
        {
            Vector2 buttonSize = _buttonPrefab.GetComponent<RectTransform>().sizeDelta;
            Vector2 width = new Vector2(Mathf.Clamp(_width.x - buttonSize.x, 0, float.MaxValue), Mathf.Clamp(_width.y - buttonSize.y, 0, float.MaxValue));
            Vector2 size = new Vector2(_tableSize.y * (buttonSize.x + width.x), _tableSize.x * (buttonSize.y + width.y));
            _buttonBackObj = Instantiate(_buttonBackPrefab, new Vector3(_leftTopPos.x + (size.x / 2) - (buttonSize.x / 2) - (width.x / 2), _leftTopPos.y - (size.y / 2) + (buttonSize.y / 2) + (width.y / 2), 0), Quaternion.identity) as GameObject;
            _buttonBackObj.transform.SetParent(c.transform);
            _buttonBackObj.GetComponent<RectTransform>().sizeDelta = size;
        }
        for (int i = 0; i < (int)(_tableSize.x * _tableSize.y); i++)
        {
            int w = Mathf.Abs((i % (int)_tableSize.y) * (int)_width.x);
            int h = Mathf.Abs((i / (int)_tableSize.y) * (int)_width.y);
            var obj = Instantiate(_buttonPrefab, new Vector3(_leftTopPos.x + w, _leftTopPos.y - h, 0), Quaternion.identity) as GameObject;
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
    }

    private enum Alignment
    {
        right,
        center,
        left
    }
}
