using System;
using UnityEngine;

// Token: 0x020012F8 RID: 4856
public class pulsingLightEmissive : LightState
{
	// Token: 0x17000F74 RID: 3956
	// (get) Token: 0x06009756 RID: 38742 RVA: 0x0012E1EC File Offset: 0x0012C3EC
	public override float Intensity
	{
		get
		{
			return (Mathf.Sin(6.2831855f * this.lightLOD.StateRate * Time.time) + 1f) / 2f;
		}
	}

	// Token: 0x17000F75 RID: 3957
	// (get) Token: 0x06009757 RID: 38743 RVA: 0x0012E216 File Offset: 0x0012C416
	public override float Emissive
	{
		get
		{
			return this.Intensity;
		}
	}
}
