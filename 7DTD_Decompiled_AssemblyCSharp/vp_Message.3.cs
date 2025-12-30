using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

// Token: 0x020012B7 RID: 4791
public class vp_Message<V, VResult> : vp_Message
{
	// Token: 0x0600958F RID: 38287 RVA: 0x003B8E0C File Offset: 0x003B700C
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public static TResult Empty<T, TResult>(T value)
	{
		return default(TResult);
	}

	// Token: 0x06009590 RID: 38288 RVA: 0x003B8CFC File Offset: 0x003B6EFC
	[Preserve]
	public vp_Message(string name) : base(name)
	{
	}

	// Token: 0x06009591 RID: 38289 RVA: 0x003B8E24 File Offset: 0x003B7024
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Send")
		};
		base.StoreInvokerFieldNames();
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetStaticGenericMethod(base.GetType(), "Empty", this.m_ArgumentType, this.m_ReturnType)
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message<, >.Sender<, >)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		if (this.m_DefaultMethods[0] != null)
		{
			base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
		}
	}

	// Token: 0x06009592 RID: 38290 RVA: 0x003B8EE6 File Offset: 0x003B70E6
	[Preserve]
	public override void Register(object t, string m, int v)
	{
		if (m == null)
		{
			return;
		}
		base.AddExternalMethodToField(t, this.m_Fields[0], m, base.MakeGenericType(this.m_DelegateTypes[0]));
		base.Refresh();
	}

	// Token: 0x06009593 RID: 38291 RVA: 0x003B8CE5 File Offset: 0x003B6EE5
	[Preserve]
	public override void Unregister(object t)
	{
		base.RemoveExternalMethodFromField(t, this.m_Fields[0]);
		base.Refresh();
	}

	// Token: 0x04007202 RID: 29186
	[Preserve]
	public new vp_Message<V, VResult>.Sender<V, VResult> Send;

	// Token: 0x020012B8 RID: 4792
	// (Invoke) Token: 0x06009595 RID: 38293
	[Preserve]
	public delegate TResult Sender<T, TResult>(T value);
}
