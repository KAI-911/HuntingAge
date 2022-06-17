using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quest : MonoBehaviour
{
    //クエストデータ
    [SerializeField] QuestData _questData = null;
    public QuestData QuestData { get => _questData; set => _questData = value; }

    //討伐対象
    [SerializeField] List<EnemyData> _targetEnemyDatas;
    public List<EnemyData> TargetEnemyData { get => _targetEnemyDatas; set => _targetEnemyDatas = value; }

    //非討伐対象
    [SerializeField] List<EnemyData> _otherEnemyDatas;
    public List<EnemyData> OtherEnemyData { get => _otherEnemyDatas; set => _otherEnemyDatas = value; }

    public bool _questset = false;
    public bool _enemytset = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_questset)
        {
            _questset = false;
            SaveData.SetClass<QuestData>(_questData.ID, _questData);
            Debug.Log("クエストセット");
        }
        if (_enemytset)
        {
            _enemytset = false;
            SaveData.SetClass<EnemyData>(_targetEnemyDatas[0].ID, _targetEnemyDatas[0]);
            Debug.Log("エネミーセット");
        }
    }
    public void QusetSelect(string QuestID)
    {
        QuestReset();
        _questData = SaveData.GetClass<QuestData>(QuestID, new QuestData());
        foreach (var enemy in _questData.TargetName)
        {
            _targetEnemyDatas.Add(SaveData.GetClass<EnemyData>(enemy, new EnemyData()));
        }
        foreach (var enemy in _questData.OtherName)
        {
            _targetEnemyDatas.Add(SaveData.GetClass<EnemyData>(enemy, new EnemyData()));
        }
    }
    public void GoToQuset()
    {
        if (_questData != null)
        {
            SceneManager.LoadSceneAsync((int)_questData.Field);
        }
    }
    public void QuestReset()
    {
        _targetEnemyDatas.Clear();
        _otherEnemyDatas.Clear();
    }
}
