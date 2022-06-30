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
    [SerializeField] GameObject _buttonPrefab;

    public List<GameObject> ItemBoxButtons { get => _ItemBoxButtons; set => _ItemBoxButtons = value; }

    private void Awake()
    {


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



    [ContextMenu("createbutton")]
    public List<GameObject> CreateButton()
    {
        DeleteButton();
        var c = GameManager.Instance.ItemCanvas.Canvas;
        for (int i = 0; i < (int)(_tableSize.x * _tableSize.y); i++)
        {
            int w = Mathf.Abs((i % (int)_tableSize.y) * (int)_width.x);
            int h = Mathf.Abs((i / (int)_tableSize.y) * (int)_width.y);
            var obj = Instantiate(_buttonPrefab, new Vector3(_leftTopPos.x + w, _leftTopPos.y - h, 0), Quaternion.identity) as GameObject;
            obj.transform.parent = c.transform;
            _ItemBoxButtons.Add(obj);
        }
        return _ItemBoxButtons;
    }
    public void DeleteButton()
    {
        foreach (var item in _ItemBoxButtons)
        {
            Destroy(item);
        }
        _ItemBoxButtons.Clear();

    }




    //[SerializeField] List<ItemButton> _ItemBoxButtons;
    //[SerializeField] List<ItemButton> _ItemPoachButtons;
    //[SerializeField] GameObject ItemBoxObject;
    //[SerializeField] GameObject ItemPoachObject;
    //void Start()
    //{
    //}
    //// Update is called once per frame
    //void Update()
    //{
    //}
    //[ContextMenu("Set")]
    //private void Set()
    //{
    //    foreach (var button in _ItemBoxButtons)
    //    {
    //        button.clear();
    //    }
    //    foreach (var button in _ItemPoachButtons)
    //    {
    //        button.clear();
    //    }
    //    foreach (var item in GameManager.Instance.ItemBox.Dictionary)
    //    {
    //        var data = GameManager.Instance.ItemDataList.Dictionary[item.Key];
    //        Debug.Log(data.Name + "  BoxUINumber  " + data.BoxUINumber + "  PoachUINumber  " + data.PoachUINumber);
    //        if (data.BoxUINumber >= 0)
    //        {
    //            Debug.Log("dsafadsgojslkhoisdnvfgsemtfcjse;c");
    //            _ItemBoxButtons[data.BoxUINumber].SetID(data.ID);
    //        }
    //        if (data.PoachUINumber >= 0)
    //        {
    //            Debug.Log("dsafadsgojslkhoisdnvfgsemtfcjse;c");
    //            _ItemPoachButtons[data.PoachUINumber].SetID(data.ID);
    //        }
    //    }
    //}

}
