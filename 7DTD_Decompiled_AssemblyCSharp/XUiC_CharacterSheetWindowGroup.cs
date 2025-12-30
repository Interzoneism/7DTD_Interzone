using System;
using UnityEngine.Scripting;

// Token: 0x02000C33 RID: 3123
[Preserve]
public class XUiC_CharacterSheetWindowGroup : XUiController
{
	// Token: 0x0600601D RID: 24605 RVA: 0x00270C39 File Offset: 0x0026EE39
	public override void Init()
	{
		base.Init();
		this.buffList = base.GetChildByType<XUiC_ActiveBuffList>();
	}

	// Token: 0x0600601E RID: 24606 RVA: 0x00270C50 File Offset: 0x0026EE50
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.buffList != null)
		{
			this.buffList.setFirstEntry = true;
		}
		base.xui.playerUI.windowManager.OpenIfNotOpen("windowpaging", false, false, true);
		XUiC_WindowSelector childByType = base.xui.FindWindowGroupByName("windowpaging").GetChildByType<XUiC_WindowSelector>();
		if (childByType != null)
		{
			childByType.SetSelected("character");
		}
		if (base.xui.PlayerEquipment != null)
		{
			base.xui.PlayerEquipment.IsOpen = true;
		}
	}

	// Token: 0x0600601F RID: 24607 RVA: 0x00270CD6 File Offset: 0x0026EED6
	public override void OnClose()
	{
		base.OnClose();
		base.xui.playerUI.windowManager.CloseIfOpen("windowpaging");
		if (base.xui.PlayerEquipment != null)
		{
			base.xui.PlayerEquipment.IsOpen = false;
		}
	}

	// Token: 0x0400488B RID: 18571
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ActiveBuffList buffList;
}
