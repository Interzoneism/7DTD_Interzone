using System;
using UnityEngine;

// Token: 0x02001328 RID: 4904
public class vp_Remover : MonoBehaviour
{
	// Token: 0x06009885 RID: 39045 RVA: 0x003CA7E9 File Offset: 0x003C89E9
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnEnable()
	{
		vp_Timer.In(Mathf.Max(this.LifeTime, 0.1f), delegate()
		{
			vp_Utility.Destroy(base.gameObject);
		}, this.m_DestroyTimer);
	}

	// Token: 0x06009886 RID: 39046 RVA: 0x003CA812 File Offset: 0x003C8A12
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		this.m_DestroyTimer.Cancel();
	}

	// Token: 0x04007502 RID: 29954
	public float LifeTime = 10f;

	// Token: 0x04007503 RID: 29955
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public vp_Timer.Handle m_DestroyTimer = new vp_Timer.Handle();
}
