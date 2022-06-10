using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

/// <summary>
/// Json�`���ŃZ�[�u�ł���N���X��񋟂��܂��B
/// </summary>
/// <remarks>
/// �ŏ��ɒl��ݒ�A�擾����^�C�~���O�Ńt�@�C���ǂݏo���܂��B
/// </remarks>
public class SaveData
{
	/// <summary>
	/// Singleton��SaveDatabase�N���X
	/// </summary>
	private static SaveDataBase savedatabase = null;

	private static SaveDataBase Savedatabase
	{
		get
		{
			if (savedatabase == null)
			{
				string path = Application.persistentDataPath + "/";
				string fileName = Application.companyName + "." + Application.productName + ".savedata.json";
				savedatabase = new SaveDataBase(path, fileName);
			}
			return savedatabase;
		}
	}

	private SaveData()
	{
	}

	#region Public Static Methods

	/// <summary>
	/// �w�肵���L�[��T�^�̃N���X�R���N�V�������Z�[�u�f�[�^�ɒǉ����܂��B
	/// </summary>
	/// <typeparam name="T">�W�F�l���b�N�N���X</typeparam>
	/// <param name="key">�L�[</param>
	/// <param name="list">T�^��List</param>
	/// <exception cref="System.ArgumentException"></exception>
	/// <remarks>�w�肵���L�[��T�^�̃N���X�R���N�V�������Z�[�u�f�[�^�ɒǉ����܂��B</remarks>
	public static void SetList<T>(string key, List<T> list)
	{
		Savedatabase.SetList<T>(key, list);
	}

	/// <summary>
	///  �w�肵���L�[��T�^�̃N���X�R���N�V�������Z�[�u�f�[�^����擾���܂��B
	/// </summary>
	/// <typeparam name="T">�W�F�l���b�N�N���X</typeparam>
	/// <param name="key">�L�[</param>
	/// <param name="_default">�f�t�H���g�̒l</param>
	/// <exception cref="System.ArgumentException"></exception>
	/// <returns></returns>
	public static List<T> GetList<T>(string key, List<T> _default)
	{
		return Savedatabase.GetList<T>(key, _default);
	}

	/// <summary>
	///  �w�肵���L�[��T�^�̃N���X���Z�[�u�f�[�^�ɒǉ����܂��B
	/// </summary>
	/// <typeparam name="T">�W�F�l���b�N�N���X</typeparam>
	/// <param name="key">�L�[</param>
	/// <param name="_default">�f�t�H���g�̒l</param>
	/// <exception cref="System.ArgumentException"></exception>
	/// <returns></returns>
	public static T GetClass<T>(string key, T _default) where T : class, new()
	{
		return Savedatabase.GetClass(key, _default);

	}

	/// <summary>
	///  �w�肵���L�[��T�^�̃N���X�R���N�V�������Z�[�u�f�[�^����擾���܂��B
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="key"></param>
	/// <param name="obj"></param>
	/// <exception cref="System.ArgumentException"></exception>
	public static void SetClass<T>(string key, T obj) where T : class, new()
	{
		Savedatabase.SetClass<T>(key, obj);
	}

	/// <summary>
	/// �w�肳�ꂽ�L�[�Ɋ֘A�t�����Ă���l���擾���܂��B
	/// </summary>
	/// <param name="key">�L�[</param>
	/// <param name="value">�l</param>
	/// <exception cref="System.ArgumentException"></exception>
	public static void SetString(string key, string value)
	{
		Savedatabase.SetString(key, value);
	}

	/// <summary>
	/// �w�肳�ꂽ�L�[�Ɋ֘A�t�����Ă���String�^�̒l���擾���܂��B
	/// �l���Ȃ��ꍇ�A_default�̒l��Ԃ��܂��B�ȗ������ꍇ�A��̕������Ԃ��܂��B
	/// </summary>
	/// <param name="key">�L�[</param>
	/// <param name="_default">�f�t�H���g�̒l</param>
	/// <exception cref="System.ArgumentException"></exception>
	/// <returns></returns>
	public static string GetString(string key, string _default = "")
	{
		return Savedatabase.GetString(key, _default);
	}

	/// <summary>
	/// �w�肳�ꂽ�L�[�Ɋ֘A�t�����Ă���Int�^�̒l���擾���܂��B
	/// </summary>
	/// <param name="key">�L�[</param>
	/// <param name="value">�f�t�H���g�̒l</param>
	/// <exception cref="System.ArgumentException"></exception>
	public static void SetInt(string key, int value)
	{
		Savedatabase.SetInt(key, value);
	}

	/// <summary>
	/// �w�肳�ꂽ�L�[�Ɋ֘A�t�����Ă���Int�^�̒l���擾���܂��B
	/// �l���Ȃ��ꍇ�A_default�̒l��Ԃ��܂��B�ȗ������ꍇ�A0��Ԃ��܂��B
	/// </summary>
	/// <param name="key">�L�[</param>
	/// <param name="_default">�f�t�H���g�̒l</param>
	/// <exception cref="System.ArgumentException"></exception>
	/// <returns></returns>
	public static int GetInt(string key, int _default = 0)
	{
		return Savedatabase.GetInt(key, _default);
	}

	/// <summary>
	/// �w�肳�ꂽ�L�[�Ɋ֘A�t�����Ă���float�^�̒l���擾���܂��B
	/// </summary>
	/// <param name="key">�L�[</param>
	/// <param name="value">�f�t�H���g�̒l</param>
	/// <exception cref="System.ArgumentException"></exception>
	public static void SetFloat(string key, float value)
	{
		Savedatabase.SetFloat(key, value);
	}

	/// <summary>
	/// �w�肳�ꂽ�L�[�Ɋ֘A�t�����Ă���float�^�̒l���擾���܂��B
	/// �l���Ȃ��ꍇ�A_default�̒l��Ԃ��܂��B�ȗ������ꍇ�A0.0f��Ԃ��܂��B
	/// </summary>
	/// <param name="key">�L�[</param>
	/// <param name="_default">�f�t�H���g�̒l</param>
	/// <exception cref="System.ArgumentException"></exception>
	/// <returns></returns>
	public static float GetFloat(string key, float _default = 0.0f)
	{
		return Savedatabase.GetFloat(key, _default);
	}

	/// <summary>
	/// �Z�[�u�f�[�^���炷�ׂẴL�[�ƒl���폜���܂��B
	/// </summary>
	public static void Clear()
	{
		Savedatabase.Clear();
	}

	/// <summary>
	/// �w�肵���L�[�����l�� �Z�[�u�f�[�^����폜���܂��B
	/// </summary>
	/// <param name="key">�L�[</param>
	/// <exception cref="System.ArgumentException"></exception>
	public static void Remove(string key)
	{
		Savedatabase.Remove(key);
	}

	/// <summary>
	/// �Z�[�u�f�[�^���ɃL�[�����݂��邩���擾���܂��B
	/// </summary>
	/// <param name="_key">�L�[</param>
	/// <exception cref="System.ArgumentException"></exception>
	/// <returns></returns>
	public static bool ContainsKey(string _key)
	{
		return Savedatabase.ContainsKey(_key);
	}

	/// <summary>
	/// �Z�[�u�f�[�^�Ɋi�[���ꂽ�L�[�̈ꗗ���擾���܂��B
	/// </summary>
	/// <exception cref="System.ArgumentException"></exception>
	/// <returns></returns>
	public static List<string> Keys()
	{
		return Savedatabase.Keys();
	}

	/// <summary>
	/// �����I�Ƀt�@�C���ɏ������݂܂��B
	/// </summary>
	public static void Save()
	{
		Savedatabase.Save();
	}

	#endregion

	#region SaveDatabase Class

	[Serializable]
	private class SaveDataBase
	{
		#region Fields

		private string path;
		//�ۑ���
		public string Path
		{
			get { return path; }
			set { path = value; }
		}

		private string fileName;
		//�t�@�C����
		public string FileName
		{
			get { return fileName; }
			set { fileName = value; }
		}

		private Dictionary<string, string> saveDictionary;
		//key��json��������i�[

		#endregion

		#region Constructor&Destructor

		public SaveDataBase(string _path, string _fileName)
		{
			path = _path;
			fileName = _fileName;
			saveDictionary = new Dictionary<string, string>();
			Load();

		}

		/// <summary>
		/// �N���X���j������鎞�_�Ńt�@�C���ɏ������݂܂��B
		/// </summary>
		~SaveDataBase()
		{
			Save();
		}

		#endregion

		#region Public Methods

		public void SetList<T>(string key, List<T> list)
		{
			keyCheck(key);
			var serializableList = new Serialization<T>(list);
			string json = JsonUtility.ToJson(serializableList);
			saveDictionary[key] = json;
		}

		public List<T> GetList<T>(string key, List<T> _default)
		{
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
			{
				return _default;
			}
			string json = saveDictionary[key];
			Serialization<T> deserializeList = JsonUtility.FromJson<Serialization<T>>(json);

			return deserializeList.ToList();
		}

		public T GetClass<T>(string key, T _default) where T : class, new()
		{
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
				return _default;

			string json = saveDictionary[key];
			T obj = JsonUtility.FromJson<T>(json);
			return obj;

		}

		public void SetClass<T>(string key, T obj) where T : class, new()
		{
			keyCheck(key);
			string json = JsonUtility.ToJson(obj);
			saveDictionary[key] = json;
		}

		public void SetString(string key, string value)
		{
			keyCheck(key);
			saveDictionary[key] = value;
		}

		public string GetString(string key, string _default)
		{
			keyCheck(key);

			if (!saveDictionary.ContainsKey(key))
				return _default;
			return saveDictionary[key];
		}

		public void SetInt(string key, int value)
		{
			keyCheck(key);
			saveDictionary[key] = value.ToString();
		}

		public int GetInt(string key, int _default)
		{
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
				return _default;
			int ret;
			if (!int.TryParse(saveDictionary[key], out ret))
			{
				ret = 0;
			}
			return ret;
		}

		public void SetFloat(string key, float value)
		{
			keyCheck(key);
			saveDictionary[key] = value.ToString();
		}

		public float GetFloat(string key, float _default)
		{
			float ret;
			keyCheck(key);
			if (!saveDictionary.ContainsKey(key))
				ret = _default;

			if (!float.TryParse(saveDictionary[key], out ret))
			{
				ret = 0.0f;
			}
			return ret;
		}

		public void Clear()
		{
			saveDictionary.Clear();

		}

		public void Remove(string key)
		{
			keyCheck(key);
			if (saveDictionary.ContainsKey(key))
			{
				saveDictionary.Remove(key);
			}

		}

		public bool ContainsKey(string _key)
		{

			return saveDictionary.ContainsKey(_key);
		}

		public List<string> Keys()
		{
			return saveDictionary.Keys.ToList<string>();
		}

		public void Save()
		{
			using (StreamWriter writer = new StreamWriter(path + fileName, false, Encoding.GetEncoding("utf-8")))
			{
				var serialDict = new Serialization<string, string>(saveDictionary);
				serialDict.OnBeforeSerialize();
				string dictJsonString = JsonUtility.ToJson(serialDict);
				writer.WriteLine(dictJsonString);
			}
		}

		public void Load()
		{
			if (File.Exists(path + fileName))
			{
				using (StreamReader sr = new StreamReader(path + fileName, Encoding.GetEncoding("utf-8")))
				{
					if (saveDictionary != null)
					{
						var sDict = JsonUtility.FromJson<Serialization<string, string>>(sr.ReadToEnd());
						sDict.OnAfterDeserialize();
						saveDictionary = sDict.ToDictionary();
					}
				}
			}
			else { saveDictionary = new Dictionary<string, string>(); }
		}

		public string GetJsonString(string key)
		{
			keyCheck(key);
			if (saveDictionary.ContainsKey(key))
			{
				return saveDictionary[key];
			}
			else
			{
				return null;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// �L�[�ɕs�����Ȃ����`�F�b�N���܂��B
		/// </summary>
		private void keyCheck(string _key)
		{
			if (string.IsNullOrEmpty(_key))
			{
				throw new ArgumentException("invalid key!!");
			}
		}

		#endregion
	}

	#endregion

	#region Serialization Class

	// List<T>
	[Serializable]
	private class Serialization<T>
	{
		public List<T> target;

		public List<T> ToList()
		{
			return target;
		}

		public Serialization()
		{
		}

		public Serialization(List<T> target)
		{
			this.target = target;
		}
	}
	// Dictionary<TKey, TValue>
	[Serializable]
	private class Serialization<TKey, TValue>
	{
		public List<TKey> keys;
		public List<TValue> values;
		private Dictionary<TKey, TValue> target;

		public Dictionary<TKey, TValue> ToDictionary()
		{
			return target;
		}

		public Serialization()
		{
		}

		public Serialization(Dictionary<TKey, TValue> target)
		{
			this.target = target;
		}

		public void OnBeforeSerialize()
		{
			keys = new List<TKey>(target.Keys);
			values = new List<TValue>(target.Values);
		}

		public void OnAfterDeserialize()
		{
			int count = Math.Min(keys.Count, values.Count);
			target = new Dictionary<TKey, TValue>(count);
			Enumerable.Range(0, count).ToList().ForEach(i => target.Add(keys[i], values[i]));
		}
	}

	#endregion
}