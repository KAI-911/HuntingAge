using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Save�ALoad�@�\�̎���
/// </summary>
public static class JsonDataManager
{
    /// <summary>
    /// �p�X���擾 & �Z�[�u�t�@�C�����L�^
    /// </summary>
    //public static string getFilePath() { return Application.persistentDataPath + "/SaveData.json"; }

    /// <summary>
    /// �������݋@�\
    /// </summary>
    /// <param name="_saveData">�V���A���C�Y����f�[�^</param>
    public static void Save<T>(T SaveData, string Path)
    {
        //�V���A���C�Y���s
        string jsonSerializedData = JsonUtility.ToJson(SaveData);
        Debug.Log(jsonSerializedData);

        //���ۂɃt�@�C������ď�������
        using (var sw = new StreamWriter(Path, false))
        {
            try
            {
                //�t�@�C���ɏ�������
                sw.Write(jsonSerializedData);
            }
            catch (Exception e) //���s�������̏���
            {
                Debug.Log(e);
            }
        }
    }

    /// <summary>
    /// �ǂݍ��݋@�\
    /// </summary>
    /// <returns>�f�V���A���C�Y�����\����</returns>
    public static T Load<T>(string Path) where T : new()
    {
       


        T SaveData = new T();

        try
        {
            Debug.Log(SaveData.GetType() + "�ǂݍ��ݑO");
            //�t�@�C����ǂݍ���
            using (FileStream fs = new FileStream(Path, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs))
            {
                Debug.Log(SaveData.GetType() + "�ǂݍ��݌�");
                string result = sr.ReadToEnd();
                Debug.Log(result);

                //�ǂݍ���Json���\���̂ɂԂ�����
                //�f�V���A���C�Y�����\���̂�Ԃ�
                SaveData = JsonUtility.FromJson<T>(result);
            }
        }
        catch (Exception e) //���s�������̏���
        {
            Debug.Log(SaveData.GetType() + "�ǂݍ��ݎ��s");
            Debug.Log(e);
        }
        Debug.Log(SaveData);
        return SaveData;
    }
}