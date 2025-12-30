using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02001299 RID: 4761
[Preserve]
public class vp_Attempt<V> : vp_Attempt
{
	// Token: 0x06009502 RID: 38146 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool AlwaysOK<T>(T value)
	{
		return true;
	}

	// Token: 0x06009503 RID: 38147 RVA: 0x003B690F File Offset: 0x003B4B0F
	public vp_Attempt(string name) : base(name)
	{
	}

	// Token: 0x06009504 RID: 38148 RVA: 0x003B6918 File Offset: 0x003B4B18
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Try")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetStaticGenericMethod(base.GetType(), "AlwaysOK", this.m_ArgumentType, typeof(bool))
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Attempt<>.Tryer<>)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnAttempt_",
				0
			}
		};
		if (this.m_DefaultMethods[0] != null)
		{
			base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
		}
	}

	// Token: 0x06009505 RID: 38149 RVA: 0x003B69E0 File Offset: 0x003B4BE0
	public override void Register(object t, string m, int v)
	{
		if (((Delegate)this.m_Fields[v].GetValue(this)).Method.Name != this.m_DefaultMethods[v].Name)
		{
			Debug.LogWarning("Warning: Event '" + base.EventName + "' of type (vp_Attempt) targets multiple methods. Events of this type must reference a single method (only the last reference will be functional).");
		}
		if (m != null)
		{
			base.SetFieldToExternalMethod(t, this.m_Fields[0], m, base.MakeGenericType(this.m_DelegateTypes[v]));
		}
	}

	// Token: 0x06009506 RID: 38150 RVA: 0x003B6A59 File Offset: 0x003B4C59
	public override void Unregister(object t)
	{
		if (this.m_DefaultMethods[0] != null)
		{
			base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
		}
	}

	// Token: 0x040071E5 RID: 29157
	public new vp_Attempt<V>.Tryer<V> Try;

	// Token: 0x0200129A RID: 4762
	// (Invoke) Token: 0x06009508 RID: 38152
	public delegate bool Tryer<T>(T value);
}
