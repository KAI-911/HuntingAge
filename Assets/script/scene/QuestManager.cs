using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class QuestManager : MonoBehaviour
{
    //�|�����G�̎�ނƐ����L�^
    private EnemyCount _killEnemyCount;
    //�|���ׂ��G�̎�ނƐ����L�^
    private EnemyCount _questTarget;
    //�S�Ă̓G�̏����L�^
    private List<Enemy> _enemyList;
    //�ǂ̂悤�ȃN�G�X�g�Ȃ̂�
    private Quest _quest;
    //���̃N�G�X�g�̏�
    private QuestStatus _questStatus;
    //�V�[���؂�ւ��܂ł̎���
    [SerializeField] float _sceneChengeTime;
    //�v���C���[�̏��
    private Player _player;
    //���̃N�G�X�g�ŉ��񎀂񂾂�
    private int _deathCount;
    private RunOnce runOnce;

    private void Start()
    {
        _questStatus = QuestStatus.quest;
        _quest = GameManager.Instance.Quest;
        _killEnemyCount = new EnemyCount();
        _questTarget = new EnemyCount();
        _enemyList = new List<Enemy>();
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
                _questTarget.Add(target);
                _enemyList.Add(ene);
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
                _enemyList.Add(ene);

            }
        }
    }
    private void Update()
    {
        if (_questStatus == QuestStatus.quest)
        {
            Debug.Log("_deathCount:" + _deathCount);
            //�����N�G�X�g�̏ꍇ
            if (_quest.QuestData.Clear == ClearConditions.TargetSubjugation)
            {
                bool clear = true;
                foreach (var item in _questTarget.EnemyCountList)
                {
                    //��������G��ID���Ȃ�==�܂���̂��|���Ă��Ȃ�
                    if (_killEnemyCount.EnemyCountList.ContainsKey(item.Key))
                    {
                        //�N�G�X�g�̓������ȏ�|���Ă��Ȃ�������
                        if (_killEnemyCount.EnemyCountList[item.Key] < item.Value)
                        {
                            clear = false;
                            break;
                        }
                    }
                    else
                    {
                        clear = false;
                        break;
                    }
                }
                if (clear) _questStatus = QuestStatus.clear;
            }
            //�̏W�N�G�X�g�̏ꍇ
            else if (_quest.QuestData.Clear == ClearConditions.Gathering)
            {

            }

            //���s����
            if (_player.Status.HP == 0)
            {
                foreach (var ene in _enemyList) ene.DiscoverFlg = false;

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
                            foreach (var ene in _enemyList) ene.DiscoverFlg = false;
                            runOnce.Flg = false;//������x�s����悤��
                        }
                    }
                );


            }
            //�N���A�E���s�����Ƃ��̏���
            //��莞�Ԍo�߂��Ă���scene�`�F���W������

        }
        else if (_questStatus == QuestStatus.clear)
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
    private void OnDestroy()
    {
        //�|�����G�̑������L�^
        _killEnemyCount.EnemyDataSet();
    }

    public void AddEnemy(Enemy enemy)
    {
        //�����G���ǉ�����Ȃ��悤�ɂ���
        if (_enemyList.Find(n => n == enemy)) return;
        _enemyList.Add(enemy);
    }
    public void AddKillEnemy(string enemyID)
    {
        _killEnemyCount.Add(enemyID);
    }
    enum QuestStatus
    {
        quest,
        clear,
        failure
    }
}
