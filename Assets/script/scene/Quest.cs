using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quest : MonoBehaviour
{
    //�N�G�X�g�f�[�^
    [SerializeField] QuestData _questData = null;
    public QuestData QuestData { get => _questData; set => _questData = value; }

    [SerializeField] EnemyData _EnemyData;
    public EnemyData TargetEnemyData { get => _EnemyData; set => _EnemyData = value; }


    public bool _questset = false;
    public bool _enemyset = false;
    public bool _enemyload = false;
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
            Debug.Log("�N�G�X�g�Z�b�g");
        }
        if (_enemyset)
        {
            _enemyset = false;
            SaveData.SetClass<EnemyData>(_EnemyData.ID, _EnemyData);
            Debug.Log("�G�l�~�[�Z�b�g");
        }
        if (_enemyload)
        {
            _enemyset = false;
            SaveData.GetClass<EnemyData>(_EnemyData.ID, _EnemyData);
            Debug.Log("�G�l�~�[���[�h");
        }
    }
    public void QusetSelect(string QuestID)
    {
        _questData = SaveData.GetClass<QuestData>(QuestID, new QuestData());
    }
    public void GoToQuset()
    {
        if (_questData != null)
        {
            SceneManager.LoadSceneAsync((int)_questData.Field);
        }
    }

}
