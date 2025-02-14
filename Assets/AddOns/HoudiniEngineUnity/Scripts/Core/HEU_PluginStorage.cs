﻿/*
* Copyright (c) <2018> Side Effects Software Inc.
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*
* 1. Redistributions of source code must retain the above copyright notice,
*    this list of conditions and the following disclaimer.
*
* 2. The name of Side Effects Software may not be used to endorse or
*    promote products derived from this software without specific prior
*    written permission.
*
* THIS SOFTWARE IS PROVIDED BY SIDE EFFECTS SOFTWARE "AS IS" AND ANY EXPRESS
* OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
* OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN
* NO EVENT SHALL SIDE EFFECTS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
* INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
* LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
* OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
* LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
* NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
* EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#if (UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_LINUX)
#define HOUDINIENGINEUNITY_ENABLED
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace HoudiniEngineUnity
{
	/// <summary>
	/// Manages storage for Houdini Engine plugin data.
	/// </summary>
	public class HEU_PluginStorage
	{
		// Internally this is using JSON as the format. The JSON data is stored into EditorPrefs.

		// Note: Unity's JSON support is streamlined for predefined objects (JsonUtility).
		// Unstructured data is not supported, but is a necessary part of this plugin.
		// To support unstructured data, the workaround used here is to store the data into a 
		// dictionary in memory, then write out as 2 ordered lists (keys, values) on to disk.
		// The lists are added to a temporary object then written out using JsonUtility.

		private enum DataType
		{
			BOOL,
			INT,
			LONG,
			FLOAT,
			STRING
		}

		// Dictionary for unstructured data.
		private Dictionary<string, StoreData> _dataMap = new Dictionary<string, StoreData>();
		// Class for unstructured data.
		[System.Serializable]
		private class StoreData
		{
			public DataType _type;
			public string _valueStr;
		}

#pragma warning disable 0649
		// Temporary class to enable us to write out arrays using JsonUtility.
		[System.Serializable]
		private class StoreDataArray<T>
		{
			public T[] _array;
		}
#pragma warning restore 0649

		// Whether plugin setting need to be saved out to file.
		private bool _requiresSave;
		public bool RequiresSave { get { return _requiresSave; } }

		private static HEU_PluginStorage _instance;

		public static HEU_PluginStorage Instance
		{
			get
			{
				if(_instance == null)
				{
					InstantiateAndLoad();
				}
				return _instance;
			}
		}

		/// <summary>
		/// Create new instance if none found.
		/// Loads plugin data from file.
		/// </summary>
		public static void InstantiateAndLoad()
		{
			if (_instance == null)
			{
				_instance = new HEU_PluginStorage();
				_instance.LoadPluginData();

				HEU_SessionManager.LoadAllSessionData();
			}
		}


		/// <summary>
		/// Retrieve the array from given JSON string.
		/// </summary>
		/// <typeparam name="T">Type of array</typeparam>
		/// <param name="jsonArray">String containing JSON array.</param>
		/// <returns>Array of objects of type T.</returns>
		private T[] GetJSONArray<T>(string jsonArray)
		{
			// Parse out array string into array class, then just grab the array.
			StoreDataArray<T> dataArray = JsonUtility.FromJson<StoreDataArray<T>>(jsonArray);
			return dataArray._array;
		}

		public void Set(string key, bool value)
		{
			StoreData data = new StoreData();
			data._type = DataType.BOOL;
			data._valueStr = System.Convert.ToString(value);

			_dataMap[key] = data;
			MarkDirtyForSave();
		}

		public void Set(string key, int value)
		{
			StoreData data = new StoreData();
			data._type = DataType.INT;
			data._valueStr = System.Convert.ToString(value);

			_dataMap[key] = data;
			MarkDirtyForSave();
		}

		public void Set(string key, long value)
		{
			StoreData data = new StoreData();
			data._type = DataType.LONG;
			data._valueStr = System.Convert.ToString(value);

			_dataMap[key] = data;
			MarkDirtyForSave();
		}

		public void Set(string key, float value)
		{
			StoreData data = new StoreData();
			data._type = DataType.FLOAT;
			data._valueStr = System.Convert.ToString(value);

			_dataMap[key] = data;
			MarkDirtyForSave();
		}
		
		public void Set(string key, string value)
		{
			StoreData data = new StoreData();
			data._type = DataType.STRING;
			data._valueStr = value;

			_dataMap[key] = data;
			MarkDirtyForSave();
		}

		public bool Get(string key, out bool value, bool defaultValue)
		{
			if(_dataMap.ContainsKey(key))
			{
				StoreData data = _dataMap[key];
				if(data._type == DataType.BOOL)
				{
					value = System.Convert.ToBoolean(data._valueStr);
					return true;
				}
			}
			value = defaultValue;
			return false;
		}

		public bool Get(string key, out int value, int defaultValue)
		{
			if (_dataMap.ContainsKey(key))
			{
				StoreData data = _dataMap[key];
				if (data._type == DataType.INT)
				{
					value = System.Convert.ToInt32(data._valueStr);
					return true;
				}
			}
			value = defaultValue;
			return false;
		}

		public bool Get(string key, out long value, long defaultValue)
		{
			if (_dataMap.ContainsKey(key))
			{
				StoreData data = _dataMap[key];
				if (data._type == DataType.LONG)
				{
					value = System.Convert.ToInt64(data._valueStr);
					return true;
				}
			}
			value = defaultValue;
			return false;
		}

		public bool Get(string key, out float value, float defaultValue)
		{
			if (_dataMap.ContainsKey(key))
			{
				StoreData data = _dataMap[key];
				if (data._type == DataType.FLOAT)
				{
					value = System.Convert.ToSingle(data._valueStr);
					return true;
				}
			}
			value = defaultValue;
			return false;
		}

		public bool Get(string key, out string value, string defaultValue)
		{
			if (_dataMap.ContainsKey(key))
			{
				StoreData data = _dataMap[key];
				if (data._type == DataType.STRING)
				{
					value = data._valueStr;
					return true;
				}
			}
			value = defaultValue;
			return false;
		}

		/// <summary>
		/// Set flag so that the plugin data will be saved out
		/// at end of update.
		/// </summary>
		private void MarkDirtyForSave()
		{
			if (!_requiresSave)
			{
#if UNITY_EDITOR
				_requiresSave = true;
				UnityEditor.EditorApplication.delayCall += SaveIfRequired;
#endif
			}
		}

		/// <summary>
		/// Saves plugin data if there are outstanding changes.
		/// </summary>
		public static void SaveIfRequired()
		{
			if(_instance != null && _instance.RequiresSave)
			{
				_instance.SavePluginData();
			}
		}

		/// <summary>
		/// Save plugin data to disk.
		/// </summary>
		private void SavePluginData()
		{
#if HOUDINIENGINEUNITY_ENABLED
			// Convert dictionary to JSON and store as string in EditorPrefs

			// Retrieve dictionary keys and store as array in temporary class.
			// Then write out array class using JsonUtility.
			StoreDataArray<string> keyArray = new StoreDataArray<string>();
			keyArray._array = new string[_dataMap.Count];
			_dataMap.Keys.CopyTo(keyArray._array, 0);
			string keyJson = JsonUtility.ToJson(keyArray);

			// Retrieve dictionary values and store as array in temporary class.
			// Then write out array class using JsonUtility.
			StoreDataArray<StoreData> dataArray = new StoreDataArray<StoreData>();
			dataArray._array = new StoreData[_dataMap.Count];
			_dataMap.Values.CopyTo(dataArray._array, 0);
			string dataJson = JsonUtility.ToJson(dataArray);

			//Debug.Log("Save:: Keys: " + keyJson);
			//Debug.Log("Save:: Data: " + dataJson);

#if UNITY_EDITOR
			// Store in Editor Prefs
			UnityEditor.EditorPrefs.SetString(HEU_Defines.PLUGIN_STORE_KEYS, keyJson);
			UnityEditor.EditorPrefs.SetString(HEU_Defines.PLUGIN_STORE_DATA, dataJson);
#endif

#endif
			_requiresSave = false;

		}

		/// <summary>
		/// Load plugin data from disk.
		/// </summary>
		/// <returns>True if successfully found and loaded data.</returns>
		private bool LoadPluginData()
		{
#if UNITY_EDITOR && HOUDINIENGINEUNITY_ENABLED
			if (UnityEditor.EditorPrefs.HasKey(HEU_Defines.PLUGIN_STORE_KEYS) && UnityEditor.EditorPrefs.HasKey(HEU_Defines.PLUGIN_STORE_DATA))
			{
				// Grab JSON strings from EditorPrefs, then use temporary array class to grab the JSON array.
				// Finally add into dictionary.

				string keyJson = UnityEditor.EditorPrefs.GetString(HEU_Defines.PLUGIN_STORE_KEYS);
				string dataJson = UnityEditor.EditorPrefs.GetString(HEU_Defines.PLUGIN_STORE_DATA);

				//Debug.Log("Load:: Keys: " + keyJson);
				//Debug.Log("Load:: Data: " + dataJson);

				string[] keyList = GetJSONArray<string>(keyJson);
				StoreData[] dataList = GetJSONArray<StoreData>(dataJson);

				_dataMap = new Dictionary<string, StoreData>();
				int numKeys = keyList.Length;
				int numData = dataList.Length;
				if (numKeys != numData)
				{
					return false;
				}
				// TODO: faster way to do this?
				for (int i = 0; i < numKeys; ++i)
				{
					_dataMap.Add(keyList[i], dataList[i]);
					//Debug.Log(string.Format("{0} : {1}", keyList[i], dataList[i]._valueStr));
				}

				return true;
			}
#endif
			return false;
		}

		/// <summary>
		/// Removes all plugin data from persistent storage.
		/// </summary>
		public static void ClearPluginData()
		{
			if (_instance != null)
			{
				_instance._dataMap = new Dictionary<string, StoreData>();
				_instance.SavePluginData();
			}
		}

		/// <summary>
		/// Save session data to disk.
		/// </summary>
		/// <param name="sessionData">The session to save.</param>
		public static void SaveSessionData(HEU_SessionData sessionData)
		{
#if UNITY_EDITOR && HOUDINIENGINEUNITY_ENABLED
			string jsonStr = JsonUtility.ToJson(sessionData);
			UnityEditor.EditorPrefs.SetString(HEU_Defines.PLUGIN_SESSION_DATA, jsonStr);
#endif
		}

		/// <summary>
		/// Load session data from disk.
		/// </summary>
		/// <returns>Loaded session data.</returns>
		public static HEU_SessionData LoadSessionData()
		{
#if UNITY_EDITOR && HOUDINIENGINEUNITY_ENABLED
			string jsonStr = UnityEditor.EditorPrefs.GetString(HEU_Defines.PLUGIN_SESSION_DATA);
			//Debug.Log("LOAD json: " + jsonStr);
			HEU_SessionData sessionData = JsonUtility.FromJson<HEU_SessionData>(jsonStr);
			return sessionData;
#else
			return null;
#endif
		}

		/// <summary>
		/// Save given list of sessions (HEU_SessionData) into storage for retrieval later.
		/// A way to persist current session information through code refresh/compiles.
		/// </summary>
		/// <param name="allSessions"></param>
		public static void SaveAllSessionData(List<HEU_SessionBase> allSessions)
		{
#if UNITY_EDITOR && HOUDINIENGINEUNITY_ENABLED
			// Formulate the JSON string for existing sessions.
			StringBuilder sb = new StringBuilder();
			foreach(HEU_SessionBase session in allSessions)
			{
				if (session.GetSessionData() != null)
				{
					sb.AppendFormat("{0};", JsonUtility.ToJson(session.GetSessionData()));
				}
			}
			UnityEditor.EditorPrefs.SetString(HEU_Defines.PLUGIN_SESSION_DATA, sb.ToString());
#endif
		}

		/// <summary>
		/// Returns list of session data retrieved from storage.
		/// </summary>
		/// <returns>List of HEU_SessionData stored on disk.</returns>
		public static List<HEU_SessionData> LoadAllSessionData()
		{
			// Retrieve saved JSON string from storage, and parse it to create the session datas.
			List<HEU_SessionData> sessions = new List<HEU_SessionData>();
#if UNITY_EDITOR && HOUDINIENGINEUNITY_ENABLED
			string jsonStr = UnityEditor.EditorPrefs.GetString(HEU_Defines.PLUGIN_SESSION_DATA);
			if (jsonStr != null && !string.IsNullOrEmpty(jsonStr))
			{
				string[] jsonSplit = jsonStr.Split(';');
				foreach(string entry in jsonSplit)
				{
					if (!string.IsNullOrEmpty(entry))
					{
						HEU_SessionData sessionData = JsonUtility.FromJson<HEU_SessionData>(entry);
						if(sessionData != null)
						{
							sessions.Add(sessionData);
						}
					}
				}
			}
#endif
			return sessions;
		}

		public static HEU_SessionData LoadSessionData(long SessionID)
		{
			// TODO: combine session string + session ID and look up in EditorPrefs
			// return the found session data
#if UNITY_EDITOR && HOUDINIENGINEUNITY_ENABLED
			string sessionKeyStr = string.Format("{0}_{1}", HEU_Defines.PLUGIN_SESSION_DATA, SessionID);
			string jsonStr = UnityEditor.EditorPrefs.GetString(sessionKeyStr);
			//Debug.Log("LOAD json: " + jsonStr);
			HEU_SessionData sessionData = JsonUtility.FromJson<HEU_SessionData>(jsonStr);
			return sessionData;
#else
			return null;
#endif
		}
	}

}   // HoudiniEngineUnity