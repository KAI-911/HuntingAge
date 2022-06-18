using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class QuestManager : MonoBehaviour
{
    //倒した敵の種類と数を記録
    private EnemyCount _killEnemyCount;
    //倒すべき敵の種類と数を記録
    private EnemyCount _questTarget;
    //全ての敵の情報を記録
    private List<Enemy> _enemyList;
    //どのようなクエストなのか
    private Quest _quest;
    //今のクエストの状況
    private QuestStatus _questStatus;
    //シーン切り替えまでの時間
    [SerializeField] float _sceneChengeTime;
    //プレイヤーの情報
    private Player _player;
    //このクエストで何回死んだか
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
            //討伐対象
            foreach (var target in _quest.QuestData.TargetName)
            {
                var data = SaveData.GetClass<EnemyData>(target, new EnemyData());
                Vector3 popPos = Vector3.zero;
                //シーンごとの最初の位置を設定
                popPos = data.EnemyPosition(_quest.QuestData.Field).pos[Random.Range(0, data.EnemyPosition(_quest.QuestData.Field).pos.Count)];
                var obj = Instantiate(Resources.Load(data.InstanceName), popPos, Quaternion.identity) as GameObject;
                var ene = obj.GetComponent<Enemy>();
                ene.WaningPos = data.EnemyPosition(_quest.QuestData.Field).pos;
                _questTarget.Add(target);
                _enemyList.Add(ene);
            }
            //非討伐対象
            foreach (var target in _quest.QuestData.OtherName)
            {
                var data = SaveData.GetClass<EnemyData>(target, new EnemyData());
                Vector3 popPos = Vector3.zero;
                //シーンごとの最初の位置を設定
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
            //討伐クエストの場合
            if (_quest.QuestData.Clear == ClearConditions.TargetSubjugation)
            {
                bool clear = true;
                foreach (var item in _questTarget.EnemyCountList)
                {
                    //討伐する敵のIDがない==まだ一体も倒していない
                    if (_killEnemyCount.EnemyCountList.ContainsKey(item.Key))
                    {
                        //クエストの討伐数以上倒していなかったら
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
            //採集クエストの場合
            else if (_quest.QuestData.Clear == ClearConditions.Gathering)
            {

            }

            //失敗条件
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
                            runOnce.Flg = false;//もう一度行えるように
                        }
                    }
                );


            }
            //クリア・失敗したときの処理
            //一定時間経過してからsceneチェンジをする

        }
        else if (_questStatus == QuestStatus.clear)
        {
            Debug.Log("クリアしました");
            _player.Status.InvincibleFlg = true;
            _sceneChengeTime -= Time.deltaTime;
            if (_sceneChengeTime < 0) SceneManager.LoadSceneAsync((int)Scene.Sato);
        }
        else if (_questStatus == QuestStatus.failure)
        {
            Debug.Log("失敗しました");
            //死亡時は早く戻る
            _player.Status.InvincibleFlg = true;
            _sceneChengeTime -= Time.deltaTime * 2;
            if (_sceneChengeTime < 0) SceneManager.LoadSceneAsync((int)GameManager.Instance.VillageScene);
        }
    }
    private void OnDestroy()
    {
        //倒した敵の総数を記録
        _killEnemyCount.EnemyDataSet();
    }

    public void AddEnemy(Enemy enemy)
    {
        //同じ敵が追加されないようにする
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
