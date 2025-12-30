using System;
using UnityEngine;

// Token: 0x02001049 RID: 4169
public class OriginParticles : MonoBehaviour
{
	// Token: 0x060083DA RID: 33754 RVA: 0x00354B50 File Offset: 0x00352D50
	public void OnEnable()
	{
		Origin.particleSystemTs.Add(base.transform);
	}

	// Token: 0x060083DB RID: 33755 RVA: 0x00354B62 File Offset: 0x00352D62
	public void OnDisable()
	{
		Origin.particleSystemTs.Remove(base.transform);
	}
}
