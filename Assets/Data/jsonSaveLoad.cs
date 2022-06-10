using System.IO;
using UnityEngine;

/// <summary>
/// jsonSaveLoad
/// : simple save/load(json format)
/// </summary>
public class jsonSaveLoad
{
    /// <summary>
    /// delete file
    /// </summary>
    /// <param name="filename"></param>
    public static void Delete(string filename)
    {
        filename = Path.Combine(Application.persistentDataPath, filename);

        File.Delete(filename);
    }
    
    /// <summary>
    /// save data
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="data"></param>
    /// <returns>false..error</returns>
    public static bool Save(string filename, object data)
    {
        string jsonData = JsonUtility.ToJson(data);
        bool   success  = true;

        using (StreamWriter writer = new StreamWriter(Path.Combine(Application.persistentDataPath, filename), false))
        {
            try
            {
                writer.Write(jsonData);
                writer.Flush();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                success = false;
            }
        }

        return success;
    }

    /// <summary>
    /// load data
    /// </summary>
    /// <typeparam name="T">data class</typeparam>
    /// <param name="filename"></param>
    /// <returns>null..error</returns>
    public static T Load<T>(string filename) where T : class
    {
        string data = null;

        filename = Path.Combine(Application.persistentDataPath, filename);

        if (File.Exists(filename) == false)
        {
            return null;
        }

        using (StreamReader reader = new StreamReader(filename, false))
        {
            try
            {
                data = reader.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
                data = null;
            }
        }
        
        return data == null ? null : JsonUtility.FromJson<T>(data);
    }
}
