using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Save、Load機能の実装
/// </summary>
public static class JsonDataManager
{
    /// <summary>
    /// パスを取得 & セーブファイル名記録
    /// </summary>
    //public static string getFilePath() { return Application.persistentDataPath + "/SaveData.json"; }

    /// <summary>
    /// 書き込み機能
    /// </summary>
    /// <param name="_saveData">シリアライズするデータ</param>
    public static void Save<T>(T SaveData, string Path)
    {
        //シリアライズ実行
        string jsonSerializedData = JsonUtility.ToJson(SaveData);
        Debug.Log(jsonSerializedData);

        //実際にファイル作って書き込む
        using (var sw = new StreamWriter(Path, false))
        {
            try
            {
                //ファイルに書き込む
                sw.Write(jsonSerializedData);
            }
            catch (Exception e) //失敗した時の処理
            {
                Debug.Log(e);
            }
        }
    }

    /// <summary>
    /// 読み込み機能
    /// </summary>
    /// <returns>デシリアライズした構造体</returns>
    public static T Load<T>(string Path) where T : new()
    {
       


        T SaveData = new T();

        try
        {
            Debug.Log(SaveData.GetType() + "読み込み前");
            //ファイルを読み込む
            using (FileStream fs = new FileStream(Path, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                Debug.Log(SaveData.GetType() + "読み込み後");
                string result = sr.ReadToEnd();
                Debug.Log(result);

                //読み込んだJsonを構造体にぶちこむ
                //デシリアライズした構造体を返す
                SaveData = JsonUtility.FromJson<T>(result);
            }
        }
        catch (Exception e) //失敗した時の処理
        {
            Debug.Log(SaveData.GetType() + "読み込み失敗");
            Debug.Log(e);
        }
        Debug.Log(SaveData);
        return SaveData;
    }
}