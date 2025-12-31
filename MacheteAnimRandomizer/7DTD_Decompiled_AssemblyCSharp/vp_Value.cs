using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Scripting;

// Token: 0x020012C6 RID: 4806
[Preserve]
public class vp_Value<V> : vp_Event
{
	// Token: 0x060095D9 RID: 38361 RVA: 0x003BA13C File Offset: 0x003B833C
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public static T Empty<T>()
	{
		return default(T);
	}

	// Token: 0x060095DA RID: 38362 RVA: 0x00002914 File Offset: 0x00000B14
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public static void Empty<T>(T value)
	{
	}

	// Token: 0x17000F53 RID: 3923
	// (get) Token: 0x060095DB RID: 38363 RVA: 0x003BA152 File Offset: 0x003B8352
	[Preserve]
	public FieldInfo[] Fields
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.m_Fields;
		}
	}

	// Token: 0x060095DC RID: 38364 RVA: 0x003B6831 File Offset: 0x003B4A31
	[Preserve]
	public vp_Value(string name) : base(name)
	{
		this.InitFields();
	}

	// Token: 0x060095DD RID: 38365 RVA: 0x003BA15C File Offset: 0x003B835C
	public void DoNotCallAOTCompileFix()
	{
		vp_Value<V>.Empty<V>();
		vp_Value<V>.Empty<V>(default(V));
		throw new InvalidOperationException("This method is used for AOT code generation only. Do not call it at runtime.");
	}

	// Token: 0x060095DE RID: 38366 RVA: 0x003BA188 File Offset: 0x003B8388
	[Preserve]
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InitFields()
	{
		this.m_Fields = new FieldInfo[]
		{
			base.GetType().GetField("Get"),
			base.GetType().GetField("Set")
		};
		base.StoreInvokerFieldNames();
		this.m_DelegateTypes = new Type[]
		{
			typeof(vp_Value<>.Getter<>),
			typeof(vp_Value<>.Setter<>)
		};
		this.m_DefaultMethods = new MethodInfo[]
		{
			base.GetStaticGenericMethod(base.GetType(), "Empty", typeof(void), this.m_ArgumentType),
			base.GetStaticGenericMethod(base.GetType(), "Empty", this.m_ArgumentType, typeof(void))
		};
		this.Prefixes = new Dictionary<string, int>
		{
			{
				"get_OnValue_",
				0
			},
			{
				"set_OnValue_",
				1
			}
		};
		if (this.m_DefaultMethods[0] != null)
		{
			base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
		}
		if (this.m_DefaultMethods[1] != null)
		{
			base.SetFieldToLocalMethod(this.m_Fields[1], this.m_DefaultMethods[1], base.MakeGenericType(this.m_DelegateTypes[1]));
		}
	}

	// Token: 0x060095DF RID: 38367 RVA: 0x003BA2D2 File Offset: 0x003B84D2
	[Preserve]
	public override void Register(object t, string m, int v)
	{
		if (m == null)
		{
			return;
		}
		base.SetFieldToExternalMethod(t, this.m_Fields[v], m, base.MakeGenericType(this.m_DelegateTypes[v]));
		base.Refresh();
	}

	// Token: 0x060095E0 RID: 38368 RVA: 0x003BA2FC File Offset: 0x003B84FC
	[Preserve]
	public override void Unregister(object t)
	{
		if (this.m_DefaultMethods[0] != null)
		{
			base.SetFieldToLocalMethod(this.m_Fields[0], this.m_DefaultMethods[0], base.MakeGenericType(this.m_DelegateTypes[0]));
		}
		if (this.m_DefaultMethods[1] != null)
		{
			base.SetFieldToLocalMethod(this.m_Fields[1], this.m_DefaultMethods[1], base.MakeGenericType(this.m_DelegateTypes[1]));
		}
		base.Refresh();
	}

	// Token: 0x0400720D RID: 29197
	[Preserve]
	public vp_Value<V>.Getter<V> Get;

	// Token: 0x0400720E RID: 29198
	[Preserve]
	public vp_Value<V>.Setter<V> Set;

	// Token: 0x020012C7 RID: 4807
	// (Invoke) Token: 0x060095E2 RID: 38370
	[Preserve]
	public delegate T Getter<T>();

	// Token: 0x020012C8 RID: 4808
	// (Invoke) Token: 0x060095E6 RID: 38374
	[Preserve]
	public delegate void Setter<T>(T o);
}
