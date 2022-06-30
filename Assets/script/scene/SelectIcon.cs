using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectIcon : MonoBehaviour
{
    [SerializeField] List<GameObject> _buttons;
    [SerializeField] Vector2 _leftTopPos;
    [SerializeField] Vector2 _width;
    [SerializeField] int _buttonSize;
    [SerializeField] bool _canBeChanged;
    [SerializeField] GameObject _buttonPrefab;

    public List<GameObject> Buttons { get => _buttons; set => _buttons = value; }
    public int ButtonSize { get => _buttonSize; set => _buttonSize = value; }

    void Start()
    {

    }

    void Update()
    {
        if (_canBeChanged)
        {
            for (int i = 0; i < _buttons.Count; i++)
            {
                Vector2 offset = new Vector2(_leftTopPos.x + (_width.x * i), _leftTopPos.y - (_width.y * i));
                _buttons[i].transform.position = new Vector3(_leftTopPos.x + offset.x, _leftTopPos.y + offset.y, 0);
            }
        }

    }
    [ContextMenu("createbutton")]
    public List<GameObject> CreateButton()
    {
        DeleteButton();
        var c = GameManager.Instance.ItemCanvas.Canvas;
        for (int i = 0; i < _buttonSize; i++)
        {
            Vector2 offset = new Vector2(_leftTopPos.x + (_width.x * i), _leftTopPos.y - (_width.y * i));

            var obj = Instantiate(_buttonPrefab, new Vector3(_leftTopPos.x + offset.x, _leftTopPos.y + offset.y, 0), Quaternion.identity) as GameObject;
            obj.transform.parent = c.transform;
            _buttons.Add(obj);
        }
        return _buttons;
    }
    public void DeleteButton()
    {
        foreach (var item in _buttons)
        {
            Destroy(item);
        }
        _buttons.Clear();

    }

}
