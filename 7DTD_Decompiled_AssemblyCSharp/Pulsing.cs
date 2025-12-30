using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000578 RID: 1400
[Preserve]
public class Pulsing : LightState
{
	// Token: 0x1700047F RID: 1151
	// (get) Token: 0x06002D3C RID: 11580 RVA: 0x0012DD74 File Offset: 0x0012BF74
	public override float LODThreshold
	{
		get
		{
			return 0.75f;
		}
	}

	// Token: 0x17000480 RID: 1152
	// (get) Token: 0x06002D3D RID: 11581 RVA: 0x0012E1EC File Offset: 0x0012C3EC
	public override float Intensity
	{
		get
		{
			return (Mathf.Sin(6.2831855f * this.lightLOD.StateRate * Time.time) + 1f) / 2f;
		}
	}

	// Token: 0x17000481 RID: 1153
	// (get) Token: 0x06002D3E RID: 11582 RVA: 0x0012E216 File Offset: 0x0012C416
	public override float Emissive
	{
		get
		{
			return this.Intensity;
		}
	}
}
