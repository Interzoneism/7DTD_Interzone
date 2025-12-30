using System;
using UnityEngine;

// Token: 0x020011D8 RID: 4568
public class OnActivateItemGameObjectReference : MonoBehaviour
{
	// Token: 0x06008EB3 RID: 36531 RVA: 0x00391D11 File Offset: 0x0038FF11
	public void ActivateItem(bool _activate)
	{
		if (this.ActivateGameObjectTransform != null)
		{
			this.ActivateGameObjectTransform.gameObject.SetActive(_activate);
		}
	}

	// Token: 0x06008EB4 RID: 36532 RVA: 0x00391D32 File Offset: 0x0038FF32
	public bool IsActivated()
	{
		return this.ActivateGameObjectTransform != null && this.ActivateGameObjectTransform.gameObject.activeSelf;
	}

	// Token: 0x04006E6E RID: 28270
	public Transform ActivateGameObjectTransform;
}
