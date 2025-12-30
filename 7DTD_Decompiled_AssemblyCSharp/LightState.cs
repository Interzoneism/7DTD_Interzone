using System;
using UnityEngine;

// Token: 0x02000577 RID: 1399
public abstract class LightState : MonoBehaviour
{
	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x06002D34 RID: 11572 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public virtual float LODThreshold
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x06002D35 RID: 11573 RVA: 0x000197A5 File Offset: 0x000179A5
	public virtual bool CanBeOn
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x06002D36 RID: 11574 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public virtual float Intensity
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x1700047D RID: 1149
	// (get) Token: 0x06002D37 RID: 11575 RVA: 0x0003E2E0 File Offset: 0x0003C4E0
	public virtual float Emissive
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x1700047E RID: 1150
	// (get) Token: 0x06002D38 RID: 11576 RVA: 0x0012E1B1 File Offset: 0x0012C3B1
	public virtual float AudioFrequency
	{
		get
		{
			if (!this.lightLOD)
			{
				return -1f;
			}
			return this.lightLOD.StateRate;
		}
	}

	// Token: 0x06002D39 RID: 11577 RVA: 0x0012E1D1 File Offset: 0x0012C3D1
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Awake()
	{
		this.lightLOD = base.gameObject.GetComponent<LightLOD>();
	}

	// Token: 0x06002D3A RID: 11578 RVA: 0x0012E1E4 File Offset: 0x0012C3E4
	public virtual void Kill()
	{
		UnityEngine.Object.Destroy(this);
	}

	// Token: 0x06002D3B RID: 11579 RVA: 0x00002AAB File Offset: 0x00000CAB
	[PublicizedFrom(EAccessModifier.Protected)]
	public LightState()
	{
	}

	// Token: 0x040023E1 RID: 9185
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public LightLOD lightLOD;
}
