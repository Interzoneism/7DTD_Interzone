using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

// Token: 0x020007F4 RID: 2036
public sealed class ObjectMessaging
{
	// Token: 0x06003A7C RID: 14972 RVA: 0x001782D2 File Offset: 0x001764D2
	public object CheckedSendMessage(Type _returnType, string methodName, ObjectMessaging.MethodSignature _methodSignature, object _target, params object[] _arguments)
	{
		return this.CheckedSendMessage(_returnType, methodName, _methodSignature, _target, ObjectMessaging.CacheDisposition.CacheTypeInfo, _arguments);
	}

	// Token: 0x06003A7D RID: 14973 RVA: 0x001782E4 File Offset: 0x001764E4
	public object CheckedSendMessage(Type _returnType, string methodName, ObjectMessaging.MethodSignature _methodSignature, object _target, ObjectMessaging.CacheDisposition _cacheDispostion, params object[] _arguments)
	{
		object result = null;
		this.SendMessageEx(_returnType, methodName, _methodSignature, _target, _cacheDispostion, out result, true, _arguments);
		return result;
	}

	// Token: 0x06003A7E RID: 14974 RVA: 0x00178307 File Offset: 0x00176507
	public object SendMessage(Type _returnType, string _methodName, ObjectMessaging.MethodSignature _methodSignature, object _target, params object[] _arguments)
	{
		return this.SendMessage(_returnType, _methodName, _methodSignature, _target, ObjectMessaging.CacheDisposition.CacheTypeInfo, _arguments);
	}

	// Token: 0x06003A7F RID: 14975 RVA: 0x00178318 File Offset: 0x00176518
	public object SendMessage(Type _returnType, string _methodName, ObjectMessaging.MethodSignature _methodSignature, object _target, ObjectMessaging.CacheDisposition _cacheDispostion, params object[] _arguments)
	{
		object result = null;
		this.SendMessageEx(_returnType, _methodName, _methodSignature, _target, _cacheDispostion, out result, false, _arguments);
		return result;
	}

	// Token: 0x06003A80 RID: 14976 RVA: 0x0017833B File Offset: 0x0017653B
	public ObjectMessaging.MethodSignature GenerateMethodSignature(Type _returnType, Type[] _types)
	{
		if (_returnType == null)
		{
			_returnType = typeof(void);
		}
		return new ObjectMessaging.MethodSignature
		{
			ArgumentTypes = _types,
			ReturnType = _returnType
		};
	}

	// Token: 0x06003A81 RID: 14977 RVA: 0x00178368 File Offset: 0x00176568
	public bool SendMessageEx(Type _returnType, string _methodName, ObjectMessaging.MethodSignature _messageSignature, object _target, ObjectMessaging.CacheDisposition _cacheDispostion, out object _returnValue, bool checkedCall, params object[] _arguments)
	{
		if (_returnType == null)
		{
			_returnType = typeof(void);
		}
		Type[] array = null;
		int num;
		if (_messageSignature == null)
		{
			num = _returnType.GetHashCode();
			for (int i = 0; i < _arguments.Length; i++)
			{
				num ^= _arguments[i].GetType().GetHashCode();
			}
		}
		else
		{
			num = _messageSignature.GetHashCode();
			_returnType = _messageSignature.ReturnType;
			array = _messageSignature.ArgumentTypes;
		}
		Type type = _target.GetType();
		MethodInfo methodInfo = null;
		Dictionary<int, MethodInfo> dictionary = null;
		if (_cacheDispostion == ObjectMessaging.CacheDisposition.CacheTypeInfo)
		{
			int key = _methodName.GetHashCode() ^ num;
			if (this.typeCache.TryGetValue(type, out dictionary))
			{
				if (dictionary.TryGetValue(key, out methodInfo))
				{
					dictionary = null;
				}
			}
			else
			{
				dictionary = new Dictionary<int, MethodInfo>();
				this.typeCache[type] = dictionary;
			}
			if (methodInfo == null && dictionary != null)
			{
				if (array == null)
				{
					array = new Type[_arguments.Length];
					for (int j = 0; j < _arguments.Length; j++)
					{
						array[j] = _arguments[j].GetType();
					}
				}
				methodInfo = this.findMethod(_methodName, type, _returnType, array);
				dictionary[key] = methodInfo;
			}
		}
		else
		{
			if (array == null)
			{
				array = new Type[_arguments.Length];
				for (int k = 0; k < _arguments.Length; k++)
				{
					array[k] = _arguments[k].GetType();
				}
			}
			methodInfo = this.findMethod(_methodName, type, _returnType, array);
		}
		if (methodInfo != null)
		{
			_returnValue = methodInfo.Invoke(_target, _arguments);
		}
		else if (checkedCall)
		{
			throw new TargetInvocationException("Method signature '" + this.buildMethodSignature(_methodName, _returnType, array) + " does not exist in object type '" + type.FullName, null);
		}
		_returnValue = null;
		return false;
	}

	// Token: 0x06003A82 RID: 14978 RVA: 0x001784FE File Offset: 0x001766FE
	public void FlushCache()
	{
		this.typeCache.Clear();
	}

	// Token: 0x06003A83 RID: 14979 RVA: 0x0017850C File Offset: 0x0017670C
	[PublicizedFrom(EAccessModifier.Private)]
	public string buildMethodSignature(string _methodName, Type _returnType, Type[] _argTypes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(_returnType.Name);
		stringBuilder.Append(" ");
		stringBuilder.Append(_methodName);
		stringBuilder.Append("(");
		for (int i = 0; i < _argTypes.Length; i++)
		{
			if (i > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(_argTypes[i].Name);
		}
		stringBuilder.Append(")");
		return stringBuilder.ToString();
	}

	// Token: 0x06003A84 RID: 14980 RVA: 0x0017858C File Offset: 0x0017678C
	[PublicizedFrom(EAccessModifier.Private)]
	public MethodInfo findMethod(string _methodName, Type _target, Type _returnType, Type[] args)
	{
		MethodInfo methodInfo = null;
		Type type = _target;
		while (type != typeof(object))
		{
			methodInfo = _target.GetMethod(_methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.ExactBinding, null, args, null);
			if (methodInfo != null)
			{
				break;
			}
			type = type.BaseType;
		}
		if (methodInfo != null && _returnType != methodInfo.ReturnType && !methodInfo.ReturnType.IsSubclassOf(_returnType))
		{
			methodInfo = null;
		}
		return methodInfo;
	}

	// Token: 0x04002F67 RID: 12135
	public static ObjectMessaging Instance = new ObjectMessaging();

	// Token: 0x04002F68 RID: 12136
	public const int DYNAMIC_SIGNATURE = 0;

	// Token: 0x04002F69 RID: 12137
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<Type, Dictionary<int, MethodInfo>> typeCache = new Dictionary<Type, Dictionary<int, MethodInfo>>();

	// Token: 0x020007F5 RID: 2037
	public enum CacheDisposition
	{
		// Token: 0x04002F6B RID: 12139
		CacheTypeInfo,
		// Token: 0x04002F6C RID: 12140
		Uncached
	}

	// Token: 0x020007F6 RID: 2038
	public sealed class MethodSignature
	{
		// Token: 0x06003A86 RID: 14982 RVA: 0x00178610 File Offset: 0x00176810
		public override int GetHashCode()
		{
			if (this.hash == 0)
			{
				this.hash = this.ReturnType.GetHashCode();
				if (this.ArgumentTypes != null && this.ArgumentTypes.Length >= 1)
				{
					for (int i = 0; i < this.ArgumentTypes.Length; i++)
					{
						this.hash ^= this.ArgumentTypes[i].GetHashCode();
					}
				}
			}
			return this.hash;
		}

		// Token: 0x04002F6D RID: 12141
		public Type[] ArgumentTypes;

		// Token: 0x04002F6E RID: 12142
		public Type ReturnType;

		// Token: 0x04002F6F RID: 12143
		[PublicizedFrom(EAccessModifier.Private)]
		public int hash;
	}
}
