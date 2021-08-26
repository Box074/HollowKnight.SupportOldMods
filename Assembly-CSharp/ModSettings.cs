using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
namespace Modding
{
	[Obsolete("Use ModSettings class instead.")]
	[Serializable]
	public class IModSettings : ModSettings
	{
		public IModSettings()
		{
		}
	}

	public class VoidModSettings : ModSettings { }

	/// <summary>
	///     Base class for storing settings for a Mod in the save file.
	/// </summary>
	[Serializable]
	public class ModSettings
	{
		/// <summary>
		///     Initializes All Dictionaries
		/// </summary>
		protected ModSettings()
		{
			StringValues = new();
			IntValues = new();
			BoolValues = new();
			FloatValues = new();
		}

		/// <summary>
		///     Hydrates the classes dictionaries with incoming data.
		/// </summary>
		/// <param name="incomingSettings">Incoming Settings</param>
		public void SetSettings(ModSettings incomingSettings)
		{
			StringValues = incomingSettings.StringValues;
			IntValues = incomingSettings.IntValues;
			BoolValues = incomingSettings.BoolValues;
			FloatValues = incomingSettings.FloatValues;
		}

		/// <summary>
		///     Handles fetching of a value in the StringValues Dictionary
		/// </summary>
		/// <param name="defaultValue">Default Value to use if value is not found in the Settings Dictionary</param>
		/// <param name="name">Compiler Generated Name of Property</param>
		/// <returns>String Value for the dictionary</returns>
		public string GetString(string defaultValue = null, [CallerMemberName] string name = null)
		{
			bool flag = name == null;
			string result;
			if (flag)
			{
				result = null;
			}
			else
			{
				result = (StringValues.ContainsKey(name) ? StringValues[name] : defaultValue);
			}
			return result;
		}

		/// <summary>
		///     Handles setting of a value in the StringValues Dictionary
		/// </summary>
		/// <param name="value">Value to Set</param>
		/// <param name="name">Compiler Generated Name of the Property</param>
		public void SetString(string value, [CallerMemberName] string name = null)
		{
			bool flag = name == null;
			if (!flag)
			{
				bool flag2 = StringValues.ContainsKey(name);
				if (flag2)
				{
					StringValues[name] = value;
				}
				else
				{
					StringValues.Add(name, value);
				}
			}
		}

		/// <summary>
		///     Handles fetching of a value in the IntValues Dictionary
		/// </summary>
		/// <param name="defaultValue">Default Value to use if value is not found in the Settings Dictionary</param>
		/// <param name="name">Compiler Generated Name of Property</param>
		/// <returns>Int Value for the dictionary</returns>
		public int GetInt(int? defaultValue = null, [CallerMemberName] string name = null)
		{
			bool flag = name == null;
			int result;
			if (flag)
			{
				result = 0;
			}
			else
			{
				result = (IntValues.ContainsKey(name) ? IntValues[name] : defaultValue.GetValueOrDefault());
			}
			return result;
		}

		/// <summary>
		///     Handles setting of a value in the IntValues Dictionary
		/// </summary>
		/// <param name="value">Value to Set</param>
		/// <param name="name">Compiler Generated Name of the Property</param>
		public void SetInt(int value, [CallerMemberName] string name = null)
		{
			bool flag = name == null;
			if (!flag)
			{
				bool flag2 = IntValues.ContainsKey(name);
				if (flag2)
				{
					IntValues[name] = value;
				}
				else
				{
					IntValues.Add(name, value);
				}
			}
		}

		/// <summary>
		///     Handles fetching of a value in the BoolValues Dictionary
		/// </summary>
		/// <param name="defaultValue">Default Value to use if value is not found in the Settings Dictionary</param>
		/// <param name="name">Compiler Generated Name of Property</param>
		/// <returns>Bool Value for the dictionary</returns>
		public bool GetBool(bool? defaultValue = null, [CallerMemberName] string name = null)
		{
			bool flag = name == null;
			return !flag && (BoolValues.ContainsKey(name) ? BoolValues[name] : defaultValue.GetValueOrDefault());
		}

		/// <summary>
		///     Handles setting of a value in the BoolValues Dictionary
		/// </summary>
		/// <param name="value">Value to Set</param>
		/// <param name="name">Compiler Generated Name of the Property</param>
		public void SetBool(bool value, [CallerMemberName] string name = null)
		{
			bool flag = name == null;
			if (!flag)
			{
				bool flag2 = BoolValues.ContainsKey(name);
				if (flag2)
				{
					BoolValues[name] = value;
				}
				else
				{
					BoolValues.Add(name, value);
				}
			}
		}

		/// <summary>
		///     Handles fetching of a value in the FloatValues Dictionary
		/// </summary>
		/// <param name="defaultValue">Default Value to use if value is not found in the Settings Dictionary</param>
		/// <param name="name">Compiler Generated Name of Property</param>
		/// <returns>Float Value for the dictionary</returns>
		public float GetFloat(float? defaultValue = null, [CallerMemberName] string name = null)
		{
			bool flag = name == null;
			float result;
			if (flag)
			{
				result = 0f;
			}
			else
			{
				result = (FloatValues.ContainsKey(name) ? FloatValues[name] : defaultValue.GetValueOrDefault());
			}
			return result;
		}

		/// <summary>
		///     Handles setting of a value in the FloatValues Dictionary
		/// </summary>
		/// <param name="value">Value to Set</param>
		/// <param name="name">Compiler Generated Name of the Property</param>
		public void SetFloat(float value, [CallerMemberName] string name = null)
		{
			bool flag = name == null;
			if (!flag)
			{
				bool flag2 = FloatValues.ContainsKey(name);
				if (flag2)
				{
					FloatValues[name] = value;
				}
				else
				{
					FloatValues.Add(name, value);
				}
			}
		}

		/// <summary>
		///     Bools to be Stored
		/// </summary>
		public SerializableBoolDictionary BoolValues;

		/// <summary>
		///     Float Values to be Stored
		/// </summary>
		public SerializableFloatDictionary FloatValues;

		/// <summary>
		///     Int Values to be Stored
		/// </summary>
		public SerializableIntDictionary IntValues;

		/// <summary>
		///     String Values to be Stored
		/// </summary>
		public SerializableStringDictionary StringValues;
	}
	/// <inheritdoc />
	/// <summary>
	///     Represents a Dictionary of Strings that can be serialized with Unity's JsonUtil
	/// </summary>
	[Serializable]
	public class SerializableStringDictionary : SerializableDictionary<string, string>
	{
		public SerializableStringDictionary()
		{
		}
	}
	/// <inheritdoc />
	/// <summary>
	///     Represents a Dictionary of Ints that can be serialized with Unity's JsonUtil
	/// </summary>
	[Serializable]
	public class SerializableIntDictionary : SerializableDictionary<string, int>
	{
		public SerializableIntDictionary()
		{
		}
	}
	[Serializable]
	public class SerializableFloatDictionary : SerializableDictionary<string, float>
	{
		public SerializableFloatDictionary()
		{
		}
	}
	/// <inheritdoc />
	/// <summary>
	///     Represents a Dictionary of Bools that can be serialized with Unity's JsonUtil
	/// </summary>
	[Serializable]
	public class SerializableBoolDictionary : SerializableDictionary<string, bool>
	{
		public SerializableBoolDictionary()
		{
		}
	}
	/// <inheritdoc cref="T:System.Collections.Generic.Dictionary`2" />
	/// <inheritdoc cref="T:UnityEngine.ISerializationCallbackReceiver" />
	/// <summary>
	///     Represents a Dictionary of &lt;<see cref="!:TKey" />,<see cref="!:TValue" />&gt; that can be serialized with
	///     Unity's JsonUtil
	/// </summary>
	[Serializable]
	public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
	{
		/// <summary>
		///     Occurse before something is serialized.
		/// </summary>
		public void OnBeforeSerialize()
		{
			this._keys.Clear();
			this._values.Clear();
			foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
			{
				this._keys.Add(keyValuePair.Key);
				this._values.Add(keyValuePair.Value);
			}
		}

		/// <summary>
		///     Occurs after the object was deserialized
		/// </summary>
		public void OnAfterDeserialize()
		{
			base.Clear();
			bool flag = this._keys.Count != this._values.Count;
			if (flag)
			{
				throw new Exception(string.Format("there are {0} keys and {1} values after deserialization. Make sure that both key and value types are serializable.", this._keys.Count, this._values.Count));
			}
			for (int i = 0; i < this._keys.Count; i++)
			{
				base.Add(this._keys[i], this._values[i]);
			}
		}

		public SerializableDictionary()
		{
		}

		[FormerlySerializedAs("keys")]
		[SerializeField]
		private List<TKey> _keys = new List<TKey>();

		[SerializeField]
		[FormerlySerializedAs("values")]
		private List<TValue> _values = new List<TValue>();
	}
}
