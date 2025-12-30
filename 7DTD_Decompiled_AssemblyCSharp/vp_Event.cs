using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200129B RID: 4763
[Preserve]
public abstract class vp_Event
{
	// Token: 0x17000F4D RID: 3917
	// (get) Token: 0x0600950B RID: 38155 RVA: 0x003B6A8F File Offset: 0x003B4C8F
	public string EventName
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x17000F4E RID: 3918
	// (get) Token: 0x0600950C RID: 38156 RVA: 0x003B6A97 File Offset: 0x003B4C97
	[Preserve]
	public Type ArgumentType
	{
		get
		{
			return this.m_ArgumentType;
		}
	}

	// Token: 0x17000F4F RID: 3919
	// (get) Token: 0x0600950D RID: 38157 RVA: 0x003B6A9F File Offset: 0x003B4C9F
	[Preserve]
	public Type ReturnType
	{
		get
		{
			return this.m_ReturnType;
		}
	}

	// Token: 0x0600950E RID: 38158
	[Preserve]
	public abstract void Register(object target, string method, int variant);

	// Token: 0x0600950F RID: 38159
	[Preserve]
	public abstract void Unregister(object target);

	// Token: 0x06009510 RID: 38160
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void InitFields();

	// Token: 0x06009511 RID: 38161 RVA: 0x003B6AA7 File Offset: 0x003B4CA7
	[Preserve]
	public vp_Event(string name = "")
	{
		this.m_ArgumentType = this.GetArgumentType;
		this.m_ReturnType = this.GetGenericReturnType;
		this.m_Name = name;
	}

	// Token: 0x06009512 RID: 38162 RVA: 0x003B6AD0 File Offset: 0x003B4CD0
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public void StoreInvokerFieldNames()
	{
		this.InvokerFieldNames = new string[this.m_Fields.Length];
		for (int i = 0; i < this.m_Fields.Length; i++)
		{
			this.InvokerFieldNames[i] = this.m_Fields[i].Name;
		}
	}

	// Token: 0x06009513 RID: 38163 RVA: 0x003B6B18 File Offset: 0x003B4D18
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public Type MakeGenericType(Type type)
	{
		if (this.m_ReturnType == typeof(void))
		{
			return type.MakeGenericType(new Type[]
			{
				this.m_ArgumentType,
				this.m_ArgumentType
			});
		}
		return type.MakeGenericType(new Type[]
		{
			this.m_ArgumentType,
			this.m_ReturnType,
			this.m_ArgumentType,
			this.m_ReturnType
		});
	}

	// Token: 0x06009514 RID: 38164 RVA: 0x003B6B8C File Offset: 0x003B4D8C
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetFieldToExternalMethod(object target, FieldInfo field, string method, Type type)
	{
		Delegate @delegate = Delegate.CreateDelegate(type, target, method, false, false);
		if (@delegate == null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error (",
				(this != null) ? this.ToString() : null,
				") Failed to bind: ",
				(target != null) ? target.ToString() : null,
				" -> ",
				method,
				"."
			}));
			return;
		}
		field.SetValue(this, @delegate);
	}

	// Token: 0x06009515 RID: 38165 RVA: 0x003B6C04 File Offset: 0x003B4E04
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public void AddExternalMethodToField(object target, FieldInfo field, string method, Type type)
	{
		Delegate @delegate = Delegate.Combine((Delegate)field.GetValue(this), Delegate.CreateDelegate(type, target, method, false, false));
		if (@delegate == null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error (",
				(this != null) ? this.ToString() : null,
				") Failed to bind: ",
				(target != null) ? target.ToString() : null,
				" -> ",
				method,
				"."
			}));
			return;
		}
		field.SetValue(this, @delegate);
	}

	// Token: 0x06009516 RID: 38166 RVA: 0x003B6C90 File Offset: 0x003B4E90
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public void SetFieldToLocalMethod(FieldInfo field, MethodInfo method, Type type)
	{
		if (method == null)
		{
			return;
		}
		Delegate @delegate = Delegate.CreateDelegate(type, method);
		if (@delegate == null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error (",
				(this != null) ? this.ToString() : null,
				") Failed to bind: ",
				(method != null) ? method.ToString() : null,
				"."
			}));
			return;
		}
		field.SetValue(this, @delegate);
	}

	// Token: 0x06009517 RID: 38167 RVA: 0x003B6D04 File Offset: 0x003B4F04
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public void RemoveExternalMethodFromField(object target, FieldInfo field)
	{
		List<Delegate> list = new List<Delegate>(((Delegate)field.GetValue(this)).GetInvocationList());
		if (list == null)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error (",
				(this != null) ? this.ToString() : null,
				") Failed to remove: ",
				(target != null) ? target.ToString() : null,
				" -> ",
				field.Name,
				"."
			}));
			return;
		}
		for (int i = list.Count - 1; i > -1; i--)
		{
			if (list[i].Target == target)
			{
				list.Remove(list[i]);
			}
		}
		if (list != null)
		{
			field.SetValue(this, Delegate.Combine(list.ToArray()));
		}
	}

	// Token: 0x06009518 RID: 38168 RVA: 0x003B6DCC File Offset: 0x003B4FCC
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public MethodInfo GetStaticGenericMethod(Type e, string name, Type parameterType, Type returnType)
	{
		foreach (MethodInfo methodInfo in e.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
		{
			if (!(methodInfo == null) && !(methodInfo.Name != name))
			{
				MethodInfo methodInfo2;
				if (this.GetGenericReturnType == typeof(void))
				{
					methodInfo2 = methodInfo.MakeGenericMethod(new Type[]
					{
						this.m_ArgumentType
					});
				}
				else
				{
					methodInfo2 = methodInfo.MakeGenericMethod(new Type[]
					{
						this.m_ArgumentType,
						this.m_ReturnType
					});
				}
				if (methodInfo2.GetParameters().Length <= 1 && (methodInfo2.GetParameters().Length != 1 || !(parameterType == typeof(void))) && (methodInfo2.GetParameters().Length != 0 || !(parameterType != typeof(void))) && (methodInfo2.GetParameters().Length != 1 || !(methodInfo2.GetParameters()[0].ParameterType != parameterType)) && !(returnType != methodInfo2.ReturnType))
				{
					return methodInfo2;
				}
			}
		}
		return null;
	}

	// Token: 0x17000F50 RID: 3920
	// (get) Token: 0x06009519 RID: 38169 RVA: 0x003B6EDA File Offset: 0x003B50DA
	[Preserve]
	public Type GetArgumentType
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (!base.GetType().IsGenericType)
			{
				return typeof(void);
			}
			return base.GetType().GetGenericArguments()[0];
		}
	}

	// Token: 0x17000F51 RID: 3921
	// (get) Token: 0x0600951A RID: 38170 RVA: 0x003B6F04 File Offset: 0x003B5104
	[Preserve]
	public Type GetGenericReturnType
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			if (!base.GetType().IsGenericType)
			{
				return typeof(void);
			}
			if (base.GetType().GetGenericArguments().Length != 2)
			{
				return typeof(void);
			}
			return base.GetType().GetGenericArguments()[1];
		}
	}

	// Token: 0x0600951B RID: 38171 RVA: 0x003B6F54 File Offset: 0x003B5154
	[Preserve]
	public Type GetParameterType(int index)
	{
		if (!base.GetType().IsGenericType)
		{
			return typeof(void);
		}
		if (index > this.m_Fields.Length - 1)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error: (",
				(this != null) ? this.ToString() : null,
				") Event '",
				this.EventName,
				"' only supports ",
				this.m_Fields.Length.ToString(),
				" indices. 'GetParameterType' referenced index ",
				index.ToString(),
				"."
			}));
		}
		if (this.m_DelegateTypes[index].GetMethod("Invoke").GetParameters().Length == 0)
		{
			return typeof(void);
		}
		return this.m_ArgumentType;
	}

	// Token: 0x0600951C RID: 38172 RVA: 0x003B7020 File Offset: 0x003B5220
	[Preserve]
	public Type GetReturnType(int index)
	{
		if (index > this.m_Fields.Length - 1)
		{
			Debug.LogError(string.Concat(new string[]
			{
				"Error: (",
				(this != null) ? this.ToString() : null,
				") Event '",
				this.EventName,
				"' only supports ",
				this.m_Fields.Length.ToString(),
				" indices. 'GetReturnType' referenced index ",
				index.ToString(),
				"."
			}));
			return null;
		}
		if (base.GetType().GetGenericArguments().Length > 1)
		{
			return this.GetGenericReturnType;
		}
		Type returnType = this.m_DelegateTypes[index].GetMethod("Invoke").ReturnType;
		if (returnType.IsGenericParameter)
		{
			return this.m_ArgumentType;
		}
		return returnType;
	}

	// Token: 0x0600951D RID: 38173 RVA: 0x00002914 File Offset: 0x00000B14
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Refresh()
	{
	}

	// Token: 0x040071E6 RID: 29158
	[PublicizedFrom(EAccessModifier.Protected)]
	public string m_Name;

	// Token: 0x040071E7 RID: 29159
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public Type m_ArgumentType;

	// Token: 0x040071E8 RID: 29160
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public Type m_ReturnType;

	// Token: 0x040071E9 RID: 29161
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public FieldInfo[] m_Fields;

	// Token: 0x040071EA RID: 29162
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public Type[] m_DelegateTypes;

	// Token: 0x040071EB RID: 29163
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public MethodInfo[] m_DefaultMethods;

	// Token: 0x040071EC RID: 29164
	[Preserve]
	public string[] InvokerFieldNames;

	// Token: 0x040071ED RID: 29165
	[Preserve]
	public Dictionary<string, int> Prefixes;
}
