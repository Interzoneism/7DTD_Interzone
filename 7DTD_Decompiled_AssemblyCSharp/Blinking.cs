using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000573 RID: 1395
[Preserve]
public class Blinking : LightState
{
	// Token: 0x17000472 RID: 1138
	// (get) Token: 0x06002D1D RID: 11549 RVA: 0x0012DD74 File Offset: 0x0012BF74
	public override float LODThreshold
	{
		get
		{
			return 0.75f;
		}
	}

	// Token: 0x17000473 RID: 1139
	// (get) Token: 0x06002D1E RID: 11550 RVA: 0x0012DD7B File Offset: 0x0012BF7B
	public override bool CanBeOn
	{
		get
		{
			return this.switchedOn;
		}
	}

	// Token: 0x17000474 RID: 1140
	// (get) Token: 0x06002D1F RID: 11551 RVA: 0x0012DD83 File Offset: 0x0012BF83
	public override float Emissive
	{
		get
		{
			if (!this.switchedOn)
			{
				return 0f;
			}
			return 1f;
		}
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x0012DD98 File Offset: 0x0012BF98
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator blink()
	{
		for (;;)
		{
			this.switchedOn = !this.switchedOn;
			yield return new WaitForSeconds(1f / this.lightLOD.StateRate);
		}
		yield break;
	}

	// Token: 0x06002D21 RID: 11553 RVA: 0x0012DDA7 File Offset: 0x0012BFA7
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		base.StartCoroutine(this.blink());
	}

	// Token: 0x06002D22 RID: 11554 RVA: 0x0012DDB6 File Offset: 0x0012BFB6
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x040023C7 RID: 9159
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool switchedOn = true;
}
