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
    }
    public void Entry(string _id, int _number = 0)
    {
        if (!_countList.ContainsKey(_id))
        {
            _countList.Add(_id, _number);
        }
    }
}
