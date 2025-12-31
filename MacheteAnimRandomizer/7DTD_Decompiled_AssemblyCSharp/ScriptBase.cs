using System;
using UnityEngine;

// Token: 0x0200120E RID: 4622
public abstract class ScriptBase : MonoBehaviour
{
	// Token: 0x17000EF8 RID: 3832
	// (get) Token: 0x06009034 RID: 36916 RVA: 0x0039888C File Offset: 0x00396A8C
	public new Transform transform
	{
		get
		{
			if (this.myTransform != null)
			{
				return this.myTransform;
			}
			return this.myTransform = base.transform;
		}
	}

	// Token: 0x17000EF9 RID: 3833
	// (get) Token: 0x06009035 RID: 36917 RVA: 0x003988C0 File Offset: 0x00396AC0
	public new GameObject gameObject
	{
		get
		{
			if (this.myGameObject != null)
			{
				return this.myGameObject;
			}
			return this.myGameObject = base.gameObject;
		}
	}

	// Token: 0x06009036 RID: 36918 RVA: 0x003988F1 File Offset: 0x00396AF1
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.className = base.GetType().Name;
		this.sbAwake();
	}

	// Token: 0x06009037 RID: 36919 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void sbAwake()
	{
	}

	// Token: 0x06009038 RID: 36920 RVA: 0x0039890A File Offset: 0x00396B0A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Update()
	{
		this.sbUpdate();
	}

	// Token: 0x06009039 RID: 36921 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void sbUpdate()
	{
	}

	// Token: 0x0600903A RID: 36922 RVA: 0x00398912 File Offset: 0x00396B12
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void FixedUpdate()
	{
		this.sbFixedUpdate();
	}

	// Token: 0x0600903B RID: 36923 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void sbFixedUpdate()
	{
	}

	// Token: 0x0600903C RID: 36924 RVA: 0x0039891A File Offset: 0x00396B1A
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void LateUpdate()
	{
		this.sbLateUpdate();
	}

	// Token: 0x0600903D RID: 36925 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void sbLateUpdate()
	{
	}

	// Token: 0x0600903E RID: 36926 RVA: 0x00002AAB File Offset: 0x00000CAB
	[PublicizedFrom(EAccessModifier.Protected)]
	public ScriptBase()
	{
	}

	// Token: 0x04006F2B RID: 28459
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform myTransform;

	// Token: 0x04006F2C RID: 28460
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject myGameObject;

	// Token: 0x04006F2D RID: 28461
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public string className;
}
