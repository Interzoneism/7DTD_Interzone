using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

// Token: 0x02001297 RID: 4759
[Preserve]
public class vp_Attempt : vp_Event
{
	// Token: 0x060094F9 RID: 38137 RVA: 0x000197A5 File Offset: 0x000179A5
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool AlwaysOK()
	{
		return true;
	}

	// Token: 0x060094FA RID: 38138 RVA: 0x003B6831 File Offset: 0x003B4A31
	public vp_Attempt(string name) : base(name)
	{
		this.InitFields();
	}

	// Token: 0x060094FB RID: 38139 RVA: 0x003B6840 File Offset: 0x003B4A40
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
			base.GetType().GetMethod("AlwaysOK")
		};
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Attempt.Tryer)
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"OnAttempt_",
				0
			}
		};
		this.Try = new vp_Attempt.Tryer(vp_Attempt.AlwaysOK);
	}

	// Token: 0x060094FC RID: 38140 RVA: 0x003B68D3 File Offset: 0x003B4AD3
	public override void Register(object t, string m, int v)
	{
		this.Try = (vp_Attempt.Tryer)Delegate.CreateDelegate(this.m_DelegateTypes[v], t, m);
		base.Refresh();
	}

	// Token: 0x060094FD RID: 38141 RVA: 0x003B68F5 File Offset: 0x003B4AF5
	public override void Unregister(object t)
	{
		this.Try = new vp_Attempt.Tryer(vp_Attempt.AlwaysOK);
		base.Refresh();
	}

	// Token: 0x040071E4 RID: 29156
	public vp_Attempt.Tryer Try;

	// Token: 0x02001298 RID: 4760
	// (Invoke) Token: 0x060094FF RID: 38143
	public delegate bool Tryer();
}
