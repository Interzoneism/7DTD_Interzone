using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

// Token: 0x020012B3 RID: 4787
[Preserve]
public class vp_Message : vp_Event
{
	// Token: 0x0600957D RID: 38269 RVA: 0x00002914 File Offset: 0x00000B14
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void Empty()
	{
	}

	// Token: 0x0600957E RID: 38270 RVA: 0x003B6831 File Offset: 0x003B4A31
	[Preserve]
	public vp_Message(string name) : base(name)
	{
		this.InitFields();
	}

	// Token: 0x0600957F RID: 38271 RVA: 0x003B8C20 File Offset: 0x003B6E20
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
			base.GetType().GetMethod("Empty")
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Message.Sender)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnMessage_",
				0
			}
		};
		this.Send = new vp_Message.Sender(vp_Message.Empty);
	}

	// Token: 0x06009580 RID: 38272 RVA: 0x003B8CB3 File Offset: 0x003B6EB3
	[Preserve]
	public override void Register(object t, string m, int v)
	{
		this.Send = (vp_Message.Sender)Delegate.Combine(this.Send, (vp_Message.Sender)Delegate.CreateDelegate(this.m_DelegateTypes[v], t, m));
		base.Refresh();
	}

	// Token: 0x06009581 RID: 38273 RVA: 0x003B8CE5 File Offset: 0x003B6EE5
	[Preserve]
	public override void Unregister(object t)
	{
		base.RemoveExternalMethodFromField(t, this.m_Fields[0]);
		base.Refresh();
	}

	// Token: 0x04007200 RID: 29184
	[Preserve]
	public vp_Message.Sender Send;

	// Token: 0x020012B4 RID: 4788
	// (Invoke) Token: 0x06009583 RID: 38275
	[Preserve]
	public delegate void Sender();
}
