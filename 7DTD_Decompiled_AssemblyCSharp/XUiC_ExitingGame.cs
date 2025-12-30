using System;
using UnityEngine.Scripting;

// Token: 0x02000CB2 RID: 3250
[Preserve]
public class XUiC_ExitingGame : XUiController
{
	// Token: 0x06006485 RID: 25733 RVA: 0x0028BCAC File Offset: 0x00289EAC
	public override void Init()
	{
		base.Init();
		XUiC_ExitingGame.ID = base.WindowGroup.ID;
	}

	// Token: 0x06006486 RID: 25734 RVA: 0x0028BCC4 File Offset: 0x00289EC4
	public override void OnOpen()
	{
		base.OnOpen();
		base.ViewComponent.IsVisible = true;
		((XUiV_Window)this.viewComponent).ForceVisible(1f);
	}

	// Token: 0x06006487 RID: 25735 RVA: 0x0028BCED File Offset: 0x00289EED
	public override void Cleanup()
	{
		base.Cleanup();
	}

	// Token: 0x04004BD5 RID: 19413
	public static string ID = "";
}
