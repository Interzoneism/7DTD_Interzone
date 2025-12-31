using System;
using UnityEngine;

// Token: 0x020011DB RID: 4571
public class OnHoverMovePanel : MonoBehaviour
{
	// Token: 0x06008EB9 RID: 36537 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
	}

	// Token: 0x06008EBA RID: 36538 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
	}

	// Token: 0x06008EBB RID: 36539 RVA: 0x00391D6A File Offset: 0x0038FF6A
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnHover(bool isOver)
	{
		if (isOver)
		{
			base.transform.GetComponent<UIPanel>().depth = 1;
			return;
		}
		base.transform.GetComponent<UIPanel>().depth = 0;
	}
}
