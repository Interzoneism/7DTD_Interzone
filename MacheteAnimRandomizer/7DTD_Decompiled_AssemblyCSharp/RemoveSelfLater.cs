using System;
using System.Collections;
using UnityEngine;

// Token: 0x02001200 RID: 4608
public class RemoveSelfLater : MonoBehaviour
{
	// Token: 0x06008FE1 RID: 36833 RVA: 0x00396C4F File Offset: 0x00394E4F
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		base.StartCoroutine(this.remove());
	}

	// Token: 0x06008FE2 RID: 36834 RVA: 0x00396C5E File Offset: 0x00394E5E
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator remove()
	{
		yield return new WaitForSeconds(this.WaitSeconds);
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04006EDD RID: 28381
	public float WaitSeconds = 1f;
}
