using System;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class MuzzleFlash : MonoBehaviour
{
	// Token: 0x0600001F RID: 31 RVA: 0x00002930 File Offset: 0x00000B30
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		base.GetComponent<Light>().intensity = 0f;
		this.targetIntensity = this.highIntensity;
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002950 File Offset: 0x00000B50
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		if (this.alarmOn)
		{
			base.GetComponent<Light>().intensity = Mathf.Lerp(base.GetComponent<Light>().intensity, this.targetIntensity, this.fadeSpeed * Time.deltaTime);
			this.CheckTargetIntensity();
			return;
		}
		base.GetComponent<Light>().intensity = Mathf.Lerp(base.GetComponent<Light>().intensity, 0f, this.fadeSpeed * Time.deltaTime);
	}

	// Token: 0x06000021 RID: 33 RVA: 0x000029C8 File Offset: 0x00000BC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckTargetIntensity()
	{
		if (Mathf.Abs(this.targetIntensity - base.GetComponent<Light>().intensity) < this.changeMargin)
		{
			if (this.targetIntensity == this.highIntensity)
			{
				this.targetIntensity = this.lowIntensity;
				return;
			}
			this.targetIntensity = this.highIntensity;
		}
	}

	// Token: 0x04000022 RID: 34
	public float fadeSpeed = 2f;

	// Token: 0x04000023 RID: 35
	public float highIntensity = 2f;

	// Token: 0x04000024 RID: 36
	public float lowIntensity = 0.5f;

	// Token: 0x04000025 RID: 37
	public float changeMargin = 0.2f;

	// Token: 0x04000026 RID: 38
	public bool alarmOn;

	// Token: 0x04000027 RID: 39
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float targetIntensity;
}
