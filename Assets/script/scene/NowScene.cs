using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NowScene : MonoBehaviour
{
    private List<Enemy> _targetEnemies;
    public List<Enemy> TargetEnemies { get => _targetEnemies; }
    private List<Enemy> _otherEnemies;
    public List<Enemy> OtherEnemies { get => _otherEnemies; }

    private Quest _quest;
    private QuestStatus _questStatus;
    [SerializeField] float _sceneChengeTime;
    private Player _player;
    private int _deathCount;
    private RunOnce runOnce;


    private void Start()
    {
        _questStatus = QuestStatus.quest;
        _quest = GameManager.Instance.Quest;
        _targetEnemies = new List<Enemy>();
        _otherEnemies = new List<Enemy>();
        _sceneChengeTime = 30f;
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _deathCount = 0;
        runOnce = new RunOnce();
        if (_quest != null)
        {
            //�����Ώ�
            foreach (var target in _quest.QuestData.TargetName)
            {
                var data = SaveData.GetClass<EnemyData>(target, new EnemyData());
                Vector3 popPos = Vector3.zero;
                //�V�[�����Ƃ̍ŏ��̈ʒu��ݒ�
                popPos = data.EnemyPosition(_quest.QuestData.Field).pos[Random.Range(0, data.EnemyPosition(_quest.QuestData.Field).pos.Count)];
                var obj = Instantiate(Resources.Load(data.InstanceName), popPos, Quaternion.identity) as GameObject;
                var ene = obj.GetComponent<Enemy>();
                ene.WaningPos = data.EnemyPosition(_quest.QuestData.Field).pos;
                _targetEnemies.Add(ene);
            }
            //�񓢔��Ώ�
            foreach (var target in _quest.QuestData.OtherName)
            {
                var data = SaveData.GetClass<EnemyData>(target, new EnemyData());
                Vector3 popPos = Vector3.zero;
                //�V�[�����Ƃ̍ŏ��̈ʒu��ݒ�
                popPos = data.EnemyPosition(_quest.QuestData.Field).pos[Random.Range(0, data.EnemyPosition(_quest.QuestData.Field).pos.Count)];
                var obj = Instantiate(Resources.Load(data.InstanceName), popPos, Quaternion.identity) as GameObject;
                var ene = obj.GetComponent<Enemy>();
                ene.WaningPos = data.EnemyPosition(_quest.QuestData.Field).pos;
                _otherEnemies.Add(ene);
                Debug.Log("�񓢔��Ώۂ̏o��");
            }
        }
    }
    private void Update()
    {
        Debug.Log("_deathCount:" + _deathCount);
        //�����N�G�X�g�̏ꍇ
        if (_quest.QuestData.Clear == ClearConditions.TargetSubjugation)
        {
            int activeCount = 0;
            foreach (var enemy in _targetEnemies)
            {
                //��̂ł��c���Ă�����܂��������Ă��Ȃ�
                if (enemy.Status.HP > 0) break;
                activeCount++;
            }
            if (activeCount == _targetEnemies.Count) _questStatus = QuestStatus.clear;
        }
        //�̏W�N�G�X�g�̏ꍇ
        else if (_quest.QuestData.Clear == ClearConditions.Gathering)
        {

        }

        if (_player.Status.HP == 0)
        {
            foreach (var ene in _targetEnemies) ene.DiscoverFlg = false;
            foreach (var ene in _otherEnemies) ene.DiscoverFlg = false;

            _ = runOnce.WaitForAsync(2,
                () =>
                {
                    _deathCount++;
                    if (_deathCount >= (int)_quest.QuestData.Failure + 1)
                    {
                        _questStatus = QuestStatus.failure;
                    }
                    else
                    {
                        _player.Revival();
                        
                        runOnce.Flg = false;//������x�s����悤��
                    }
                }
            );


        }

        //�N���A�E���s�����Ƃ��̏���
        //��莞�Ԍo�߂��Ă���scene�`�F���W������
        if (_questStatus == QuestStatus.clear)
        {
            Debug.Log("�N���A���܂���");
            _player.Status.InvincibleFlg = true;
             _sceneChengeTime -= Time.deltaTime;
            if (_sceneChengeTime < 0) SceneManager.LoadSceneAsync((int)Scene.Sato);
        }
        else if (_questStatus == QuestStatus.failure)
        {
            Debug.Log("���s���܂���");
            //���S���͑����߂�
            _player.Status.InvincibleFlg = true;
            _sceneChengeTime -= Time.deltaTime * 2;
            if (_sceneChengeTime < 0) SceneManager.LoadSceneAsync((int)GameManager.Instance.VillageScene);
        }
    }
    enum QuestStatus
    {
        quest,
        clear,
        failure
    }
}
