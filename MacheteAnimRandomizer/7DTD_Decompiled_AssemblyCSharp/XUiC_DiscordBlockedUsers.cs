using System;
using UnityEngine.Scripting;

// Token: 0x020002EF RID: 751
[Preserve]
public class XUiC_DiscordBlockedUsers : XUiController
{
	// Token: 0x06001566 RID: 5478 RVA: 0x0007E4F0 File Offset: 0x0007C6F0
	public override void Init()
	{
		base.Init();
		XUiC_DiscordBlockedUsers.ID = base.WindowGroup.ID;
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnBack") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += delegate(XUiController _, int _)
			{
				base.xui.playerUI.windowManager.Close(this.windowGroup, false);
				base.xui.playerUI.windowManager.Open(this.windowGroup.openWindowOnEsc, true, false, true);
			};
		}
	}

	// Token: 0x04000DB3 RID: 3507
	public static string ID = "";
}
