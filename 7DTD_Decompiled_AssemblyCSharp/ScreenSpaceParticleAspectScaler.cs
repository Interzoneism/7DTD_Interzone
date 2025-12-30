using System;
using UnityEngine;

// Token: 0x02000070 RID: 112
public class ScreenSpaceParticleAspectScaler : MonoBehaviour
{
	// Token: 0x060001FB RID: 507 RVA: 0x0001129C File Offset: 0x0000F49C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		float x = (float)Screen.width / (float)Screen.height / this.designTimeAspectRatio;
		base.transform.localScale = new Vector3(x, 1f, 1f);
	}

	// Token: 0x040002C5 RID: 709
	public float designTimeAspectRatio;
}
