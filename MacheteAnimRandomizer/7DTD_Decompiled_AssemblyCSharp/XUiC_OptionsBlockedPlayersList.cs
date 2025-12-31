using System;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000D45 RID: 3397
[Preserve]
public class XUiC_OptionsBlockedPlayersList : XUiController
{
	// Token: 0x06006A00 RID: 27136 RVA: 0x002B1280 File Offset: 0x002AF480
	public override void Init()
	{
		base.Init();
		XUiC_OptionsBlockedPlayersList.ID = base.WindowGroup.ID;
		base.WindowGroup.openWindowOnEsc = XUiC_OptionsMenu.ID;
		this.blockedPlayersList = base.GetChildByType<XUiC_BlockedPlayersList>();
		(base.GetChildById("btnBack") as XUiC_SimpleButton).OnPressed += this.BtnBack_OnPressed;
	}

	// Token: 0x06006A01 RID: 27137 RVA: 0x002B12E0 File Offset: 0x002AF4E0
	public override void OnClose()
	{
		base.OnClose();
		BlockedPlayerList instance = BlockedPlayerList.Instance;
		if (instance == null)
		{
			return;
		}
		instance.MarkForWrite();
	}

	// Token: 0x06006A02 RID: 27138 RVA: 0x002A30F5 File Offset: 0x002A12F5
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPressed(XUiController _sender, int _mouseButton)
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsMenu.ID, true, false, true);
	}

	// Token: 0x04004FDE RID: 20446
	public static string ID = "";

	// Token: 0x04004FDF RID: 20447
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_BlockedPlayersList blockedPlayersList;
}
