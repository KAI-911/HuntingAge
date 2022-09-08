using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringCount
{
    [SerializeField] Dictionary<string, int> _countList = new Dictionary<string, int>();
    public Dictionary<string, int> CountList { get => _countList; }


    public void Add(string _id,int add)
    {
        Entry(_id);
        //���ɓo�^����Ă����炻�̂܂ܒǉ�
        if (_countList.ContainsKey(_id))
        {
            Debug.Log("�ǉ�");
            _countList[_id] += add;
        }
        Debug.Log("�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[");
        foreach (var item in _countList)
        {
            Debug.Log(item.Key+"  "+item.Value);
        }
        Debug.Log("�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[�[");

    }
    public void Entry(string _id, int _number = 0)
    {
        if (!_countList.ContainsKey(_id))
        {
            _countList.Add(_id, _number);
        }
    }
}
