using System;
using UnityEngine;

// Token: 0x02000F25 RID: 3877
[PublicizedFrom(EAccessModifier.Internal)]
public class XUiUpdateHelper : MonoBehaviour
{
	// Token: 0x06007BD1 RID: 31697 RVA: 0x00321CC6 File Offset: 0x0031FEC6
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		XUiUpdater.Update();
	}
}
