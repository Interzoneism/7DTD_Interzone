using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02001303 RID: 4867
[Preserve]
public class vp_DamageInfo
{
	// Token: 0x060097AF RID: 38831 RVA: 0x003C5B40 File Offset: 0x003C3D40
	public vp_DamageInfo(float damage, Transform source)
	{
		this.Damage = damage;
		this.Source = source;
		this.OriginalSource = source;
	}

	// Token: 0x060097B0 RID: 38832 RVA: 0x003C5B5D File Offset: 0x003C3D5D
	public vp_DamageInfo(float damage, Transform source, Transform originalSource)
	{
		this.Damage = damage;
		this.Source = source;
		this.OriginalSource = originalSource;
	}

	// Token: 0x04007410 RID: 29712
	public float Damage;

	// Token: 0x04007411 RID: 29713
	public Transform Source;

	// Token: 0x04007412 RID: 29714
	public Transform OriginalSource;
}
