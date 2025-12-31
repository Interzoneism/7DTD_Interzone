using System;
using UnityEngine.Scripting;

// Token: 0x02000C06 RID: 3078
[Preserve]
public class XUiControllerMissing : XUiController
{
	// Token: 0x06005E4E RID: 24142 RVA: 0x00263F00 File Offset: 0x00262100
	public override void OnOpen()
	{
		base.OnOpen();
		this.OnClose();
	}
}
