using System;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class BlockSwitchController : MonoBehaviour
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600051E RID: 1310 RVA: 0x00024C33 File Offset: 0x00022E33
	// (set) Token: 0x0600051F RID: 1311 RVA: 0x00024C3B File Offset: 0x00022E3B
	public bool Powered
	{
		get
		{
			return this.powered;
		}
		set
		{
			this.powered = value;
			this.UpdateLights();
		}
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x06000520 RID: 1312 RVA: 0x00024C4A File Offset: 0x00022E4A
	// (set) Token: 0x06000521 RID: 1313 RVA: 0x00024C52 File Offset: 0x00022E52
	public bool Activated
	{
		get
		{
			return this.activated;
		}
		set
		{
			this.activated = value;
			this.UpdateLights();
		}
	}

	// Token: 0x06000522 RID: 1314 RVA: 0x00024C61 File Offset: 0x00022E61
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.UpdateLights();
	}

	// Token: 0x06000523 RID: 1315 RVA: 0x00024C69 File Offset: 0x00022E69
	public void SetState(bool _powered, bool _activated)
	{
		this.powered = _powered;
		this.activated = _activated;
		this.UpdateLights();
	}

	// Token: 0x06000524 RID: 1316 RVA: 0x00024C80 File Offset: 0x00022E80
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void UpdateLights()
	{
		if (!this.Powered)
		{
			this.GreenLight.SetActive(false);
			this.RedLight.SetActive(false);
			return;
		}
		this.GreenLight.SetActive(this.activated);
		this.RedLight.SetActive(!this.activated);
	}

	// Token: 0x040005DB RID: 1499
	public GameObject RedLight;

	// Token: 0x040005DC RID: 1500
	public GameObject GreenLight;

	// Token: 0x040005DD RID: 1501
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool powered;

	// Token: 0x040005DE RID: 1502
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool activated;
}
