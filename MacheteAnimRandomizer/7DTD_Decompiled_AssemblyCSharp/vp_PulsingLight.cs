using System;
using UnityEngine;

// Token: 0x020012FD RID: 4861
public class vp_PulsingLight : MonoBehaviour
{
	// Token: 0x06009777 RID: 38775 RVA: 0x003C4A3B File Offset: 0x003C2C3B
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		this.m_Light = base.GetComponent<Light>();
	}

	// Token: 0x06009778 RID: 38776 RVA: 0x003C4A4C File Offset: 0x003C2C4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.m_Light == null)
		{
			return;
		}
		this.m_Light.intensity = this.m_MinIntensity + Mathf.Abs(Mathf.Cos(Time.time * this.m_Rate) * (this.m_MaxIntensity - this.m_MinIntensity));
	}

	// Token: 0x040073DC RID: 29660
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Light m_Light;

	// Token: 0x040073DD RID: 29661
	public float m_MinIntensity = 2f;

	// Token: 0x040073DE RID: 29662
	public float m_MaxIntensity = 5f;

	// Token: 0x040073DF RID: 29663
	public float m_Rate = 1f;
}
