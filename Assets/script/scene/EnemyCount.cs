using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyCount
{
    [SerializeField]Dictionary<string,int> _enemyCountList = new Dictionary<string, int>();
    public Dictionary<string, int> EnemyCountList { get => _enemyCountList; }

    /// <summary>
    /// 倒した敵の総数を記録
    /// </summary>
    public void EnemyDataSet()
    {
        foreach (var item in _enemyCountList)
        {
            GameManager.Instance.EnemyDataList.Dictionary[item.Key].DeathCount += item.Value;
            GameManager.Instance.EnemyDataList.DesrializeDictionary();
        }
    }
    public void Add(string _enemyID)
    {
        Entry(_enemyID);
        //既に登録されていたらそのまま追加
        if (_enemyCountList.ContainsKey(_enemyID))
        {
            Debug.Log("追加");
            _enemyCountList[_enemyID]++;
        }
    }
    public void Entry(string _enemyID, int _number = 0)
    {
        if (!_enemyCountList.ContainsKey(_enemyID))
        {
            _enemyCountList.Add(_enemyID,_number);
        }
    }
}
