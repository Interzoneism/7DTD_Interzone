using System;
using UnityEngine.Scripting;

// Token: 0x02000C75 RID: 3189
[Preserve]
public class XUiC_Creative2WindowGroup : XUiController
{
	// Token: 0x06006245 RID: 25157 RVA: 0x0027DB44 File Offset: 0x0027BD44
	public override void OnOpen()
	{
		base.OnOpen();
		base.xui.playerUI.windowManager.OpenIfNotOpen("windowpaging", false, false, true);
		XUiC_WindowSelector childByType = base.xui.FindWindowGroupByName("windowpaging").GetChildByType<XUiC_WindowSelector>();
		if (childByType == null)
		{
			return;
		}
		childByType.SetSelected("creative2");
	}

	// Token: 0x06006246 RID: 25158 RVA: 0x0027DB98 File Offset: 0x0027BD98
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
	}
}
