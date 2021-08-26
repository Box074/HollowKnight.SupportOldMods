using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using JetBrains.Annotations;

namespace HKLab
{
	/// <summary>
	///     A class to aid in reflection while caching it.
	/// </summary>
	public static class ReflectionHelper
	{
		/// <summary>
		///     Caches all fields on a type to frontload cost of reflection
		/// </summary>
		/// <typeparam name="T">The type to cache</typeparam>
		public static void CacheFields<T>()
		{
			Type typeFromHandle = typeof(T);
			Dictionary<string, FieldInfo> dictionary;
			bool flag = !ReflectionHelper.Fields.TryGetValue(typeFromHandle, out dictionary);
			if (flag)
			{
				dictionary = new Dictionary<string, FieldInfo>();
			}
			MethodInfo method = typeof(ReflectionHelper).GetMethod("GetGetter", BindingFlags.Static | BindingFlags.NonPublic);
			MethodInfo method2 = typeof(ReflectionHelper).GetMethod("GetSetter", BindingFlags.Static | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in typeFromHandle.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				dictionary[fieldInfo.Name] = fieldInfo;
				bool flag2 = !fieldInfo.IsLiteral;
				if (flag2)
				{
					if (method != null)
					{
						method.MakeGenericMethod(new Type[]
						{
							typeFromHandle,
							fieldInfo.FieldType
						}).Invoke(null, new object[]
						{
							fieldInfo
						});
					}
				}
				bool flag3 = !fieldInfo.IsLiteral && !fieldInfo.IsInitOnly;
				if (flag3)
				{
					if (method2 != null)
					{
						method2.MakeGenericMethod(new Type[]
						{
							typeFromHandle,
							fieldInfo.FieldType
						}).Invoke(null, new object[]
						{
							fieldInfo
						});
					}
				}
			}
		}

		/// <summary>
		///     Gets a field on a type
		/// </summary>
		/// <param name="t">Type</param>
		/// <param name="field">Field name</param>
		/// <param name="instance"></param>
		/// <returns>FieldInfo for field or null if field does not exist.</returns>
		public static FieldInfo GetField(Type t, string field, bool instance = true)
		{
			Dictionary<string, FieldInfo> dictionary;
			bool flag = !ReflectionHelper.Fields.TryGetValue(t, out dictionary);
			if (flag)
			{
				ReflectionHelper.Fields.Add(t, dictionary = new Dictionary<string, FieldInfo>());
			}
			FieldInfo field2;
			bool flag2 = dictionary.TryGetValue(field, out field2);
			FieldInfo result;
			if (flag2)
			{
				result = field2;
			}
			else
			{
				field2 = t.GetField(field, BindingFlags.Public | BindingFlags.NonPublic | (instance ? BindingFlags.Instance : BindingFlags.Static));
				bool flag3 = field2 != null;
				if (flag3)
				{
					dictionary.Add(field, field2);
				}
				result = field2;
			}
			return result;
		}

		internal static void PreloadCommonTypes()
		{
			bool preloaded = ReflectionHelper._preloaded;
			if (!preloaded)
			{
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ReflectionHelper.CacheFields<PlayerData>();
				ReflectionHelper.CacheFields<HeroController>();
				ReflectionHelper.CacheFields<GameManager>();
				ReflectionHelper.CacheFields<UIManager>();
				stopwatch.Stop();
				Modding.Logger.Log(string.Format("Preloaded reflection in {0}ms", stopwatch.ElapsedMilliseconds));
				ReflectionHelper._preloaded = true;
			}
		}

		/// <summary>
		///     Gets delegate getting field on type
		/// </summary>
		/// <param name="fi">FieldInfo for field.</param>
		/// <returns>Function which gets value of field</returns>
		private static Delegate GetGetter<TType, TField>(FieldInfo fi)
		{
			Delegate @delegate;
			bool flag = ReflectionHelper.Getters.TryGetValue(fi, out @delegate);
			Delegate result;
			if (flag)
			{
				result = @delegate;
			}
			else
			{
				bool isLiteral = fi.IsLiteral;
				if (isLiteral)
				{
					throw new ArgumentException("Field cannot be const", "fi");
				}
				@delegate = (fi.IsStatic ? ReflectionHelper.CreateGetStaticFieldDelegate<TType, TField>(fi) : ReflectionHelper.CreateGetFieldDelegate<TType, TField>(fi));
				ReflectionHelper.Getters.Add(fi, @delegate);
				result = @delegate;
			}
			return result;
		}

		/// <summary>
		///     Gets delegate setting field on type
		/// </summary>
		/// <param name="fi">FieldInfo for field.</param>
		/// <returns>Function which sets field passed as FieldInfo</returns>
		private static Delegate GetSetter<TType, TField>(FieldInfo fi)
		{
			Delegate @delegate;
			bool flag = ReflectionHelper.Setters.TryGetValue(fi, out @delegate);
			Delegate result;
			if (flag)
			{
				result = @delegate;
			}
			else
			{
				bool flag2 = fi.IsLiteral || fi.IsInitOnly;
				if (flag2)
				{
					throw new ArgumentException("Field cannot be readonly or const", "fi");
				}
				@delegate = (fi.IsStatic ? ReflectionHelper.CreateSetStaticFieldDelegate<TType, TField>(fi) : ReflectionHelper.CreateSetFieldDelegate<TType, TField>(fi));
				ReflectionHelper.Setters.Add(fi, @delegate);
				result = @delegate;
			}
			return result;
		}

		/// <summary>
		///     Create delegate returning value of static field.
		/// </summary>
		/// <param name="fi">FieldInfo of field</param>
		/// <typeparam name="TField">Field type</typeparam>
		/// <typeparam name="TType">Type which field resides upon</typeparam>
		/// <returns>Function returning static field</returns>
		private static Delegate CreateGetStaticFieldDelegate<TType, TField>(FieldInfo fi)
		{
			string str = "FieldAccess";
			Type declaringType = fi.DeclaringType;
			DynamicMethod dynamicMethod = new DynamicMethod(str + ((declaringType != null) ? declaringType.Name : null) + fi.Name, typeof(TField), new Type[0], typeof(TType));
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldsfld, fi);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethod.CreateDelegate(typeof(Func<TField>));
		}

		/// <summary>
		///     Create delegate returning value of field of object
		/// </summary>
		/// <param name="fi"></param>
		/// <typeparam name="TType"></typeparam>
		/// <typeparam name="TField"></typeparam>
		/// <returns>Function which returns value of field of object parameter</returns>
		private static Delegate CreateGetFieldDelegate<TType, TField>(FieldInfo fi)
		{
			string str = "FieldAccess";
			Type declaringType = fi.DeclaringType;
			DynamicMethod dynamicMethod = new DynamicMethod(str + ((declaringType != null) ? declaringType.Name : null) + fi.Name, typeof(TField), new Type[]
			{
				typeof(TType)
			}, typeof(TType));
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldfld, fi);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethod.CreateDelegate(typeof(Func<TType, TField>));
		}

		private static Delegate CreateSetFieldDelegate<TType, TField>(FieldInfo fi)
		{
			string str = "FieldSet";
			Type declaringType = fi.DeclaringType;
			DynamicMethod dynamicMethod = new DynamicMethod(str + ((declaringType != null) ? declaringType.Name : null) + fi.Name, typeof(void), new Type[]
			{
				typeof(TType),
				typeof(TField)
			}, typeof(TType));
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldarg_1);
			ilgenerator.Emit(OpCodes.Stfld, fi);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethod.CreateDelegate(typeof(Action<TType, TField>));
		}

		private static Delegate CreateSetStaticFieldDelegate<TType, TField>(FieldInfo fi)
		{
			string str = "FieldSet";
			Type declaringType = fi.DeclaringType;
			DynamicMethod dynamicMethod = new DynamicMethod(str + ((declaringType != null) ? declaringType.Name : null) + fi.Name, typeof(void), new Type[]
			{
				typeof(TField)
			}, typeof(TType));
			ILGenerator ilgenerator = dynamicMethod.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Stsfld, fi);
			ilgenerator.Emit(OpCodes.Ret);
			return dynamicMethod.CreateDelegate(typeof(Action<TField>));
		}

		/// <summary>
		///     Get a field on an object using a string. Cast to TCast before returning and if field doesn't exist return default.
		/// </summary>
		/// <param name="obj">Object/Object of type which the field is on</param>
		/// <param name="name">Name of the field</param>
		/// <param name="default">Default return</param>
		/// <typeparam name="TField">Type of field</typeparam>
		/// <typeparam name="TObject">Type of object being passed in</typeparam>
		/// <typeparam name="TCast">Type of return.</typeparam>
		/// <returns>The value of a field on an object/type</returns>
		[PublicAPI]
		public static TCast GetAttr<TObject, TField, TCast>(TObject obj, string name, TCast @default = default(TCast))
		{
			FieldInfo field = ReflectionHelper.GetField(typeof(TObject), name, true);
			return (field == null) ? @default : ((TCast)((object)((Func<TObject, TField>)ReflectionHelper.GetGetter<TObject, TField>(field))(obj)));
		}

		/// <summary>
		///     Get a field on an object using a string.
		/// </summary>
		/// <param name="obj">Object/Object of type which the field is on</param>
		/// <param name="name">Name of the field</param>
		/// <typeparam name="TField">Type of field</typeparam>
		/// <typeparam name="TObject">Type of object being passed in</typeparam>
		/// <returns>The value of a field on an object/type</returns>
		[PublicAPI]
		public static TField GetAttr<TObject, TField>(TObject obj, string name)
		{
			FieldInfo field = ReflectionHelper.GetField(typeof(TObject), name, true);
			if (field == null)
			{
				throw new MissingFieldException();
			}
			FieldInfo fi = field;
			return ((Func<TObject, TField>)ReflectionHelper.GetGetter<TObject, TField>(fi))(obj);
		}

		/// <summary>
		///     Get a static field on an type using a string.
		/// </summary>
		/// <param name="name">Name of the field</param>
		/// <typeparam name="TType">Type which static field resides upon</typeparam>
		/// <typeparam name="TField">Type of field</typeparam>
		/// <returns>The value of a field on an object/type</returns>
		[PublicAPI]
		public static TField GetAttr<TType, TField>(string name)
		{
			FieldInfo field = ReflectionHelper.GetField(typeof(TType), name, false);
			return (field == null) ? default(TField) : ((Func<TField>)ReflectionHelper.GetGetter<TType, TField>(field))();
		}

		/// <summary>
		///     Set a field on an object using a string.
		/// </summary>
		/// <param name="obj">Object/Object of type which the field is on</param>
		/// <param name="name">Name of the field</param>
		/// <param name="value">Value to set the field to</param>
		/// <typeparam name="TField">Type of field</typeparam>
		/// <typeparam name="TObject">Type of object being passed in</typeparam>
		[PublicAPI]
		public static void SetAttrSafe<TObject, TField>(TObject obj, string name, TField value)
		{
			FieldInfo field = ReflectionHelper.GetField(typeof(TObject), name, true);
			bool flag = field == null;
			if (!flag)
			{
				((Action<TObject, TField>)ReflectionHelper.GetSetter<TObject, TField>(field))(obj, value);
			}
		}

		/// <summary>
		///     Set a field on an object using a string.
		/// </summary>
		/// <param name="obj">Object/Object of type which the field is on</param>
		/// <param name="name">Name of the field</param>
		/// <param name="value">Value to set the field to</param>
		/// <typeparam name="TField">Type of field</typeparam>
		/// <typeparam name="TObject">Type of object being passed in</typeparam>
		[PublicAPI]
		public static void SetAttr<TObject, TField>(TObject obj, string name, TField value)
		{
			FieldInfo field = ReflectionHelper.GetField(typeof(TObject), name, true);
			if (field == null)
			{
				throw new MissingFieldException("Field " + name + " does not exist!");
			}
			FieldInfo fi = field;
			((Action<TObject, TField>)ReflectionHelper.GetSetter<TObject, TField>(fi))(obj, value);
		}

		/// <summary>
		///     Set a static field on an type using a string.
		/// </summary>
		/// <param name="name">Name of the field</param>
		/// <param name="value">Value to set the field to</param>
		/// <typeparam name="TType">Type which static field resides upon</typeparam>
		/// <typeparam name="TField">Type of field</typeparam>
		[PublicAPI]
		public static void SetAttr<TType, TField>(string name, TField value)
		{
			((Action<TField>)ReflectionHelper.GetGetter<TType, TField>(ReflectionHelper.GetField(typeof(TType), name, false)))(value);
		}

		/// <summary>
		///     Set a field on an object using a string.
		/// </summary>
		/// <param name="obj">Object/Object of type which the field is on</param>
		/// <param name="name">Name of the field</param>
		/// <param name="val">Value to set the field to to</param>
		/// <param name="instance">Whether or not to get an instance field or a static field</param>
		/// <typeparam name="T">Type of the object which the field holds.</typeparam>
		[Obsolete("Use SetAttr<TType, TField> and SetAttr<TObject, TField>.")]
		[PublicAPI]
		public static void SetAttr<T>(object obj, string name, T val, bool instance = true)
		{
			bool flag = obj == null || string.IsNullOrEmpty(name);
			if (!flag)
			{
				FieldInfo field = ReflectionHelper.GetField(obj.GetType(), name, instance);
				if (field != null)
				{
					field.SetValue(obj, val);
				}
			}
		}

		/// <summary>
		///     Get a field on an object/type using a string.
		/// </summary>
		/// <param name="obj">Object/Object of type which the field is on</param>
		/// <param name="name">Name of the field</param>
		/// <param name="instance">Whether or not to get an instance field or a static field</param>
		/// <typeparam name="T">Type of the object which the field holds.</typeparam>
		/// <returns>The value of a field on an object/type</returns>
		[PublicAPI]
		[Obsolete("Use GetAttr<TObject, TField>.")]
		public static T GetAttr<T>(object obj, string name, bool instance = true)
		{
			bool flag = obj == null || string.IsNullOrEmpty(name);
			T result;
			if (flag)
			{
				result = default(T);
			}
			else
			{
				FieldInfo field = ReflectionHelper.GetField(obj.GetType(), name, instance);
				result = (T)((object)((field != null) ? field.GetValue(obj) : null));
			}
			return result;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ReflectionHelper()
		{
		}

		private static readonly Dictionary<Type, Dictionary<string, FieldInfo>> Fields = new Dictionary<Type, Dictionary<string, FieldInfo>>();

		private static readonly Dictionary<FieldInfo, Delegate> Getters = new Dictionary<FieldInfo, Delegate>();

		private static readonly Dictionary<FieldInfo, Delegate> Setters = new Dictionary<FieldInfo, Delegate>();

		private static bool _preloaded;
	}
}
