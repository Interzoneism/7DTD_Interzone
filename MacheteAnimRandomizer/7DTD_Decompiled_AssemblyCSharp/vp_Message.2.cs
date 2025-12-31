using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

// Token: 0x020012B5 RID: 4789
[Preserve]
public class vp_Message<V> : vp_Message
{
	// Token: 0x06009586 RID: 38278 RVA: 0x00002914 File Offset: 0x00000B14
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void Empty<T>(T value)
	{
	}

	// Token: 0x06009587 RID: 38279 RVA: 0x003B8CFC File Offset: 0x003B6EFC
	[Preserve]
	public vp_Message(string name) : base(name)
	{
	}

	// Token: 0x06009588 RID: 38280 RVA: 0x003B8D08 File Offset: 0x003B6F08
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
			base.GetStaticGenericMethod(base.GetType(), "Empty", this.m_ArgumentType, typeof(void))
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message<>.Sender<>)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		this.Send = new vp_Message<V>.Sender<V>(vp_Message<V>.Empty<V>);
		if (this.m_DefaultMethods[0] != null)
		{
			base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
		}
	}

	// Token: 0x06009589 RID: 38281 RVA: 0x003B8DE0 File Offset: 0x003B6FE0
	[Preserve]
	public override void Register(object t, string m, int v)
	{
		if (m == null)
		{
			return;
		}
		base.AddExternalMethodToField(t, this.m_Fields[v], m, base.MakeGenericType(this.m_DelegateTypes[v]));
		base.Refresh();
	}

	// Token: 0x0600958A RID: 38282 RVA: 0x003B8CE5 File Offset: 0x003B6EE5
	[Preserve]
	public override void Unregister(object t)
	{
		base.RemoveExternalMethodFromField(t, this.m_Fields[0]);
		base.Refresh();
	}

	// Token: 0x04007201 RID: 29185
	[Preserve]
	public new vp_Message<V>.Sender<V> Send;

	// Token: 0x020012B6 RID: 4790
	// (Invoke) Token: 0x0600958C RID: 38284
	[Preserve]
	public delegate void Sender<T>(T value);
}
