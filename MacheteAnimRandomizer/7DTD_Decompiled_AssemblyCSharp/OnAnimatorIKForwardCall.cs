using System;
using UnityEngine;

// Token: 0x020011DA RID: 4570
public class OnAnimatorIKForwardCall : MonoBehaviour
{
	// Token: 0x06008EB7 RID: 36535 RVA: 0x00391D54 File Offset: 0x0038FF54
	public void OnAnimatorIK(int layerIndex)
	{
		if (this.Callback != null)
		{
			this.Callback.OnAnimatorIK(layerIndex);
		}
	}

	// Token: 0x04006E6F RID: 28271
	public IOnAnimatorIKCallback Callback;
}
