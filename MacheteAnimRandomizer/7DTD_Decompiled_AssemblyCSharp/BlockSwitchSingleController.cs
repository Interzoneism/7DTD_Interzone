using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class BlockSwitchSingleController : MonoBehaviour
{
	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000526 RID: 1318 RVA: 0x00024CD3 File Offset: 0x00022ED3
	// (set) Token: 0x06000527 RID: 1319 RVA: 0x00024CDB File Offset: 0x00022EDB
	public bool Activated
	{
		get
		{
			return this.activated;
		}
		set
		{
			this.activated = value;
			this.SetState();
		}
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x00024CEA File Offset: 0x00022EEA
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.SetState();
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x00024CDB File Offset: 0x00022EDB
	public void SetState(bool _activated)
	{
		this.activated = _activated;
		this.SetState();
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x00024CF2 File Offset: 0x00022EF2
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void SetState()
	{
		if (this.ItemPrefab != null)
		{
			this.ItemPrefab.SetActive(!this.activated);
		}
	}

	// Token: 0x040005DF RID: 1503
	public GameObject ItemPrefab;

	// Token: 0x040005E0 RID: 1504
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool activated;
}
