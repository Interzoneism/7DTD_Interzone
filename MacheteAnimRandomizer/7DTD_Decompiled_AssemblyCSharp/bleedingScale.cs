using System;
using UnityEngine;

// Token: 0x0200000C RID: 12
public class bleedingScale : MonoBehaviour
{
	// Token: 0x06000023 RID: 35 RVA: 0x00002A50 File Offset: 0x00000C50
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		float x = this.parentObject.transform.lossyScale.x;
		base.gameObject.GetComponent<ParticleSystem>().main.startSize = new ParticleSystem.MinMaxCurve(this.minParticleScale * x, this.maxParticleScale * x)
		{
			mode = ParticleSystemCurveMode.TwoConstants
		};
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
	}

	// Token: 0x04000028 RID: 40
	public GameObject parentObject;

	// Token: 0x04000029 RID: 41
	public float minParticleScale;

	// Token: 0x0400002A RID: 42
	public float maxParticleScale;
}
